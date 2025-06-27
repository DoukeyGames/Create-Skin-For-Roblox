using System;
using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;

public class PurchasesListener : Purchases.UpdatedCustomerInfoListener
{
    private const string YearlyProductID = "com.yearly.skins";
    private const string WeeklyProductID = "com.weekly.skins";
    [SerializeField] private int activeIndx, inactiveIndx;

    private Dictionary<string, Purchases.Package> availablePackages = new Dictionary<string, Purchases.Package>();

    private void Start()
    {
        var purchases = GetComponent<Purchases>();
        purchases.SetDebugLogsEnabled(true);

        purchases.GetOfferings((offerings, error) =>
        {
            if (error != null)
            {
                Debug.LogError("Error fetching offerings: " + error.Message);
                return;
            }

            if (offerings.Current != null)
            {
                foreach (var package in offerings.Current.AvailablePackages)
                {
                    availablePackages[package.Identifier] = package; // Correct way to reference package ID
                }
                Debug.Log("Available packages loaded.");
            }
        });
    }

    public void PurchaseYearlySubscription()
    {
        PurchaseProduct(YearlyProductID);
        // Track both with direct AppsFlyer call and through AnalyticsManager
        LogPurchaseEvent(99.99f, "USD", YearlyProductID);
        AnalyticsManager.Instance.TrackSubscription(99.99f, "USD", YearlyProductID);
    }

    public void PurchaseFreeTrialSubscription()
    {
        PurchaseProduct(WeeklyProductID);
        // Track as a trial subscription
        LogPurchaseEvent(0f, "USD", WeeklyProductID);
        AnalyticsManager.Instance.TrackSubscription(0f, "USD", WeeklyProductID, false, true);
    }

    private void PurchaseProduct(string productId)
    {
        if (availablePackages.TryGetValue(productId, out var package))
        {
            var purchases = GetComponent<Purchases>();
            purchases.PurchasePackage(package, (productIdentifier, customerInfo, userCancelled, error) =>
            {
                if (!userCancelled)
                {
                    if (error != null)
                    {
                        Debug.LogError("Purchase error: " + error.Message);
                    }
                    else
                    {
                        Debug.Log("Purchase successful! Updated Customer Info: " + customerInfo);
                        MenuPanelController.Instance.ActivePanel(activeIndx);
                    }
                }
                else
                {
                    Debug.Log("User cancelled the purchase.");
                }
            });
        }
        else
        {
            Debug.LogError($"Product {productId} not found in available offerings.");
        }
    }

    private void LogPurchaseEvent(float price, string currency, string contentId)
    {
        try
        {
            Dictionary<string, string> eventValues = new Dictionary<string, string>();
            eventValues.Add("af_revenue", price.ToString());
            eventValues.Add("af_currency", currency);
            eventValues.Add("af_content_id", contentId);
            eventValues.Add("af_content_type", "subscription");
            eventValues.Add("af_quantity", "1");

            // Log for debugging
            Debug.Log($"Sending AppsFlyer purchase event: af_purchase with revenue: {price} {currency}, content: {contentId}");

            AppsFlyer.sendEvent("af_purchase", eventValues);

            // Also try sending with standard event name for redundancy
            AppsFlyer.sendEvent(AFInAppEvents.PURCHASE, eventValues);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending AppsFlyer purchase event: {ex.Message}");
            AnalyticsManager.Instance.TrackError("purchase_event", ex.Message);
        }
    }

    public void RestoreClicked()
    {
        // Track restore attempt
        AnalyticsManager.Instance.LogEvent(AnalyticsManager.EventNames.RESTORE_PURCHASE);

        var purchases = GetComponent<Purchases>();
        purchases.RestorePurchases((customerInfo, error) =>
        {
            if (error != null)
            {
                Debug.LogError("Restore failed: " + error.Message);
                AnalyticsManager.Instance.TrackError("restore_purchase", error.Message);
            }
            else
            {
                Debug.Log("Purchases restored successfully. Updated Customer Info: " + customerInfo);

                // Track successful restore with subscription info if available
                Dictionary<string, string> restoreParams = new Dictionary<string, string>();
                if (customerInfo != null && customerInfo.ActiveSubscriptions != null)
                {
                    restoreParams.Add("active_subscriptions_count", customerInfo.ActiveSubscriptions.Count.ToString());
                }
                AnalyticsManager.Instance.LogEvent(AnalyticsManager.EventNames.RESTORE_PURCHASE, restoreParams);

                MenuPanelController.Instance.ActivePanel(inactiveIndx);
            }
        });
    }

    public override void CustomerInfoReceived(Purchases.CustomerInfo customerInfo)
    {
        Debug.Log("Customer Info Updated: " + customerInfo);
        // Handle subscription status updates here
    }
}
