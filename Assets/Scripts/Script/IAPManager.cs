using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using AppsFlyerSDK;
using UnityEngine.Serialization;

public class IAPManager : MonoBehaviour, IDetailedStoreListener
{
    public static IAPManager Instance { get; private set; }
    private IStoreController m_StoreController;
    private IExtensionProvider m_StoreExtensionProvider;
    [SerializeField] private string yearlyProductId;
    [SerializeField] private string yearlyIntroductoryOfferProductId;
    [SerializeField] private string freeTrialProductId;
    [SerializeField] private int activeIndx, inactiveIndx;

    private void Awake() => Instance = this;

    private void Start()
    {
        Debug.Log("Initializing IAP...");
        InitializePurchasing();
        CheckAndSetPanel();
    }

    private void InitializePurchasing()
    {
        if (IsInitialized()) return;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(yearlyProductId, ProductType.Subscription);
        builder.AddProduct(yearlyIntroductoryOfferProductId, ProductType.Subscription);
        builder.AddProduct(freeTrialProductId, ProductType.Subscription);

        UnityPurchasing.Initialize(this, builder);
    }

    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void BuyYearlySubscription()
    {
        if (!IsInitialized())
        {
            Debug.LogError("IAP not initialized. Try again later.");
            return;
        }
        BuyProductID(yearlyProductId);
    }
    
    public void BuyYealyIntroductSubscription()
    {
        if (!IsInitialized())
        {
            Debug.LogError("IAP not initialized. Try again later.");
            return;
        }
        BuyProductID(yearlyIntroductoryOfferProductId);
    }

    public void BuyFreeTrialSubscription()
    {
        if (!IsInitialized())
        {
            Debug.LogError("IAP not initialized. Try again later.");
            return;
        }
        BuyProductID(freeTrialProductId);
    }

    private void BuyProductID(string productId)
    {
        Product product = m_StoreController.products.WithID(productId);

        if (product != null && product.availableToPurchase)
        {
            Debug.Log($"Purchasing product: {product.definition.id}");

            // Track purchase initiation
            Dictionary<string, string> purchaseParams = new Dictionary<string, string>
            {
                { "product_id", productId },
                { "price", product.metadata.localizedPrice.ToString() },
                { "currency", product.metadata.isoCurrencyCode }
            };
            AnalyticsManager.Instance.LogEvent("purchase_initiated", purchaseParams);

            m_StoreController.InitiatePurchase(product);
        }
        else
        {
            Debug.LogError($"Buy {product.definition.id} FAIL. Product not found or not available.");

            // Track purchase failure
            Dictionary<string, string> errorParams = new Dictionary<string, string>
            {
                { "product_id", productId },
                { "error", "Product not found or not available" }
            };
            AnalyticsManager.Instance.TrackError("purchase_initiation", "Product not found or not available");
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("IAP Initialized Successfully");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
        CheckSubscriptionStatus();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        OnInitializeFailed(error, null);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";
        if (message != null)
            errorMessage += $" More details: {message}";
        Debug.Log(errorMessage);
    }

    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            Debug.LogError("Cannot restore purchases, IAP not initialized.");
            AnalyticsManager.Instance.TrackError("restore_purchase", "IAP not initialized");
            return;
        }

        // Track restore attempt
        AnalyticsManager.Instance.LogEvent(AnalyticsManager.EventNames.RESTORE_PURCHASE);

        var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
        apple.RestoreTransactions((result, message) => {
            Debug.Log($"Restore Purchases result: {result}");

            // Track restore result
            Dictionary<string, string> restoreParams = new Dictionary<string, string>
            {
                { "success", result.ToString() }
            };
            if (!string.IsNullOrEmpty(message))
            {
                restoreParams.Add("message", message);
            }
            AnalyticsManager.Instance.LogEvent(AnalyticsManager.EventNames.RESTORE_PURCHASE, restoreParams);

            PlayerPrefs.SetInt("Purchased", 0); // Reset subscription state
            CheckAndSetPanel();
            if (result) CheckSubscriptionStatus();
        });
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log($"Purchase Completed: {args.purchasedProduct.definition.id}");

        // Track successful purchase with AppsFlyer
        Product product = args.purchasedProduct;
        string productId = product.definition.id;
        decimal price = product.metadata.localizedPrice;
        string currency = product.metadata.isoCurrencyCode;

        // Track through AnalyticsManager
        AnalyticsManager.Instance.TrackPurchase((float)price, currency, productId);

        // Also track as subscription if it's a subscription product
        if (productId == yearlyProductId)
        {
            AnalyticsManager.Instance.TrackSubscription((float)price, currency, productId);
        }
        else if (productId == freeTrialProductId)
        {
            AnalyticsManager.Instance.TrackSubscription(0f, currency, productId, false, true);
        }
        else if (productId == yearlyIntroductoryOfferProductId)
        {
            AnalyticsManager.Instance.TrackSubscription((float)price, currency, productId);
        }

        // Direct AppsFlyer tracking for redundancy
        Dictionary<string, string> purchaseParams = new Dictionary<string, string>
        {
            { "af_revenue", price.ToString() },
            { "af_currency", currency },
            { "af_content_id", productId },
            { "af_quantity", "1" }
        };
        AppsFlyer.sendEvent(AFInAppEvents.PURCHASE, purchaseParams);

        PlayerPrefs.SetInt("Purchased", 1);
        CheckAndSetPanel();
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");

        // Track purchase failure
        Dictionary<string, string> failureParams = new Dictionary<string, string>
        {
            { "product_id", product.definition.id },
            { "failure_reason", failureReason.ToString() }
        };
        AnalyticsManager.Instance.TrackError("purchase_failed", failureReason.ToString());
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log($"Purchase failed - Product: '{product.definition.id}'," +
            $" Purchase failure reason: {failureDescription.reason}," +
            $" Purchase failure details: {failureDescription.message}");

        // Track detailed purchase failure
        Dictionary<string, string> failureParams = new Dictionary<string, string>
        {
            { "product_id", product.definition.id },
            { "failure_reason", failureDescription.reason.ToString() },
            { "failure_message", failureDescription.message }
        };
        AnalyticsManager.Instance.TrackError("purchase_failed_detailed",
            $"{failureDescription.reason}: {failureDescription.message}");
    }

    private void CheckSubscriptionStatus()
    {
        if (!IsInitialized())
        {
            Debug.LogError("Cannot check subscription status, IAP not initialized.");
            return;
        }

        var yearlyProduct = m_StoreController.products.WithID(yearlyProductId);
        var freeTrialProduct = m_StoreController.products.WithID(freeTrialProductId);
        var monthlyProduct = m_StoreController.products.WithID(yearlyIntroductoryOfferProductId);
        bool isSubscribed = IsSubscribedTo(yearlyProduct) || IsSubscribedTo(freeTrialProduct) || IsSubscribedTo(monthlyProduct);

        Debug.Log(isSubscribed ? "User is subscribed." : "User is NOT subscribed.");
        PlayerPrefs.SetInt("Purchased", isSubscribed ? 1 : 0);
        CheckAndSetPanel();
    }

    private bool IsSubscribedTo(Product subscription)
    {
        if (subscription?.receipt == null) return false;

        var subscriptionManager = new SubscriptionManager(subscription, null);
        var info = subscriptionManager.getSubscriptionInfo();

        return info.isSubscribed() == Result.True;
    }

    private void CheckAndSetPanel()
    {
        int purchased = PlayerPrefs.GetInt("Purchased", 0);
        ScreenManager.Instance.ActivePanel(purchased == 1 ? activeIndx : inactiveIndx);
    }
}
