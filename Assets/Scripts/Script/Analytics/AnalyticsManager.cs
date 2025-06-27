using System;
using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;

/// <summary>
/// Centralized manager for all analytics tracking in the application.
/// Implements the singleton pattern for easy access from anywhere.
/// </summary>
public class AnalyticsManager : MonoBehaviour
{
    #region Singleton Implementation
    
    private static AnalyticsManager _instance;
    
    public static AnalyticsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("AnalyticsManager");
                _instance = go.AddComponent<AnalyticsManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeAnalytics();
    }
    
    #endregion

    #region AppsFlyer Standard Event Names
    
    // Standard AppsFlyer event names for consistent tracking
    public static class EventNames
    {
        // App Lifecycle Events
        public const string APP_OPEN = "af_app_open";
        public const string APP_UPDATE = "af_app_update";
        public const string APP_BACKGROUND = "af_app_background";
        public const string APP_FOREGROUND = "af_app_foreground";
        public const string APP_STARTED = "af_app_started";

        // User Events
        public const string LOGIN = AFInAppEvents.LOGIN;
        public const string REGISTRATION = AFInAppEvents.COMPLETE_REGISTRATION;

        // Content Events
        public const string CONTENT_VIEW = AFInAppEvents.CONTENT_VIEW;
        public const string SCREEN_VIEW = "af_screen_view";
        public const string SKIN_DOWNLOAD = "skin_download";
        public const string SKIN_CUSTOMIZATION = "skin_customization";
        public const string SKIN_PREVIEW = "af_skin_preview";
        public const string SKIN_SHARE = "af_skin_share";

        // Navigation Events
        public const string CATALOG_VIEW = "af_catalog_view";
        public const string CATALOG_SWITCH = "af_catalog_switch";

        // Purchase Events
        public const string PURCHASE = AFInAppEvents.PURCHASE;
        public const string SUBSCRIPTION = "af_subscription";
        public const string SUBSCRIPTION_RENEWAL = "af_subscription_renewal";
        public const string TRIAL_START = "af_start_trial";
        public const string RESTORE_PURCHASE = "af_restore_purchase";

        // Paywall Events
        public const string PAYWALL_VIEW = "af_paywall_view";
        public const string PAYWALL_INTERACTION = "af_paywall_interaction";
        public const string SUBSCRIPTION_SELECTION = "af_subscription_selection";
        public const string SUBSCRIPTION_ATTEMPT = "af_subscription_attempt";

        // Feature Usage Events
        public const string FEATURE_USED = "af_feature_used";
        public const string SHARE = "af_share";
        public const string ONBOARDING_COMPLETE = "af_onboarding_complete";

        // Error Events
        public const string ERROR = "af_error";
    }
    
    #endregion

    #region Initialization
    
    private void InitializeAnalytics()
    {
        Debug.Log("Initializing AnalyticsManager");
        
        // AppsFlyer is initialized in AppEvent.cs
        // This class will only handle event tracking
    }
    
    #endregion

    #region Event Tracking Methods
    
    /// <summary>
    /// Tracks a screen view event
    /// </summary>
    /// <param name="screenName">Name of the screen being viewed</param>
    public void TrackScreenView(string screenName)
    {
        try
        {
            Dictionary<string, string> eventValues = new Dictionary<string, string>
            {
                { "af_screen_name", screenName }
            };
            
            LogEvent(EventNames.SCREEN_VIEW, eventValues);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error tracking screen view: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Tracks a content view event (viewing a specific item)
    /// </summary>
    /// <param name="contentId">ID of the content</param>
    /// <param name="contentType">Type of content (e.g., "skin", "outfit")</param>
    /// <param name="contentName">Name of the content</param>
    public void TrackContentView(string contentId, string contentType, string contentName)
    {
        try
        {
            Dictionary<string, string> eventValues = new Dictionary<string, string>
            {
                { "af_content_id", contentId },
                { "af_content_type", contentType },
                { "af_content_name", contentName }
            };
            
            LogEvent(EventNames.CONTENT_VIEW, eventValues);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error tracking content view: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Tracks a skin download event
    /// </summary>
    /// <param name="skinName">Name of the skin</param>
    /// <param name="skinType">Type of skin (e.g., "shirt", "pants", "full")</param>
    public void TrackSkinDownload(string skinName, string skinType)
    {
        try
        {
            Dictionary<string, string> eventValues = new Dictionary<string, string>
            {
                { "skin_name", skinName },
                { "skin_type", skinType }
            };
            
            LogEvent(EventNames.SKIN_DOWNLOAD, eventValues);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error tracking skin download: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Tracks a skin customization event
    /// </summary>
    /// <param name="skinType">Type of skin being customized</param>
    /// <param name="customizationType">Type of customization (e.g., "color", "pattern")</param>
    public void TrackSkinCustomization(string skinType, string customizationType)
    {
        try
        {
            Dictionary<string, string> eventValues = new Dictionary<string, string>
            {
                { "skin_type", skinType },
                { "customization_type", customizationType }
            };
            
            LogEvent(EventNames.SKIN_CUSTOMIZATION, eventValues);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error tracking skin customization: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Tracks a purchase event
    /// </summary>
    /// <param name="price">Price of the purchase</param>
    /// <param name="currency">Currency code (e.g., "USD")</param>
    /// <param name="contentId">ID of the purchased content</param>
    /// <param name="contentType">Type of the purchased content</param>
    public void TrackPurchase(float price, string currency, string contentId, string contentType = "subscription")
    {
        try
        {
            Dictionary<string, string> eventValues = new Dictionary<string, string>
            {
                { "af_revenue", price.ToString() },
                { "af_currency", currency },
                { "af_content_id", contentId },
                { "af_content_type", contentType }
            };
            
            LogEvent(EventNames.PURCHASE, eventValues);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error tracking purchase: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Tracks a subscription event
    /// </summary>
    /// <param name="price">Price of the subscription</param>
    /// <param name="currency">Currency code</param>
    /// <param name="subscriptionId">ID of the subscription</param>
    /// <param name="isRenewal">Whether this is a renewal</param>
    /// <param name="isTrial">Whether this is a trial</param>
    public void TrackSubscription(float price, string currency, string subscriptionId, bool isRenewal = false, bool isTrial = false)
    {
        try
        {
            string eventName = isTrial ? EventNames.TRIAL_START : 
                              isRenewal ? EventNames.SUBSCRIPTION_RENEWAL : 
                                         EventNames.SUBSCRIPTION;
            
            Dictionary<string, string> eventValues = new Dictionary<string, string>
            {
                { "af_revenue", price.ToString() },
                { "af_currency", currency },
                { "af_content_id", subscriptionId }
            };
            
            LogEvent(eventName, eventValues);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error tracking subscription: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Tracks a feature usage event
    /// </summary>
    /// <param name="featureName">Name of the feature</param>
    /// <param name="additionalParams">Any additional parameters</param>
    public void TrackFeatureUsed(string featureName, Dictionary<string, string> additionalParams = null)
    {
        try
        {
            Dictionary<string, string> eventValues = new Dictionary<string, string>
            {
                { "feature_name", featureName }
            };

            if (additionalParams != null)
            {
                foreach (var param in additionalParams)
                {
                    eventValues[param.Key] = param.Value;
                }
            }

            LogEvent(EventNames.FEATURE_USED, eventValues);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error tracking feature usage: {ex.Message}");
        }
    }

    /// <summary>
    /// Tracks paywall view events
    /// </summary>
    /// <param name="paywallName">Name of the paywall</param>
    /// <param name="source">Source that triggered the paywall</param>
    public void TrackPaywallView(string paywallName, string source = null)
    {
        try
        {
            Dictionary<string, string> eventValues = new Dictionary<string, string>
            {
                { "paywall_name", paywallName }
            };

            if (!string.IsNullOrEmpty(source))
            {
                eventValues["source"] = source;
            }

            LogEvent(EventNames.PAYWALL_VIEW, eventValues);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error tracking paywall view: {ex.Message}");
        }
    }

    /// <summary>
    /// Tracks paywall interactions
    /// </summary>
    /// <param name="interactionType">Type of interaction (e.g., "button_click", "selection")</param>
    /// <param name="elementName">Name of the UI element</param>
    /// <param name="additionalParams">Any additional parameters</param>
    public void TrackPaywallInteraction(string interactionType, string elementName, Dictionary<string, string> additionalParams = null)
    {
        try
        {
            Dictionary<string, string> eventValues = new Dictionary<string, string>
            {
                { "interaction_type", interactionType },
                { "element_name", elementName }
            };

            if (additionalParams != null)
            {
                foreach (var param in additionalParams)
                {
                    eventValues[param.Key] = param.Value;
                }
            }

            LogEvent(EventNames.PAYWALL_INTERACTION, eventValues);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error tracking paywall interaction: {ex.Message}");
        }
    }

    /// <summary>
    /// Tracks subscription selection events
    /// </summary>
    /// <param name="subscriptionType">Type of subscription selected</param>
    /// <param name="price">Price of the subscription</param>
    /// <param name="currency">Currency code</param>
    public void TrackSubscriptionSelection(string subscriptionType, float price = 0f, string currency = "USD")
    {
        try
        {
            Dictionary<string, string> eventValues = new Dictionary<string, string>
            {
                { "subscription_type", subscriptionType },
                { "af_revenue", price.ToString() },
                { "af_currency", currency }
            };

            LogEvent(EventNames.SUBSCRIPTION_SELECTION, eventValues);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error tracking subscription selection: {ex.Message}");
        }
    }

    /// <summary>
    /// Tracks catalog view and navigation events
    /// </summary>
    /// <param name="catalogType">Type of catalog (e.g., "shirt", "pants", "tshirt", "fullbody")</param>
    /// <param name="catalogIndex">Index of the catalog</param>
    public void TrackCatalogView(string catalogType, int catalogIndex = -1)
    {
        try
        {
            Dictionary<string, string> eventValues = new Dictionary<string, string>
            {
                { "catalog_type", catalogType }
            };

            if (catalogIndex >= 0)
            {
                eventValues["catalog_index"] = catalogIndex.ToString();
            }

            LogEvent(EventNames.CATALOG_VIEW, eventValues);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error tracking catalog view: {ex.Message}");
        }
    }

    /// <summary>
    /// Tracks app lifecycle events
    /// </summary>
    /// <param name="lifecycleEvent">Type of lifecycle event</param>
    /// <param name="additionalParams">Any additional parameters</param>
    public void TrackAppLifecycle(string lifecycleEvent, Dictionary<string, string> additionalParams = null)
    {
        try
        {
            Dictionary<string, string> eventValues = new Dictionary<string, string>
            {
                { "lifecycle_event", lifecycleEvent }
            };

            if (additionalParams != null)
            {
                foreach (var param in additionalParams)
                {
                    eventValues[param.Key] = param.Value;
                }
            }

            LogEvent(lifecycleEvent, eventValues);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error tracking app lifecycle: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Tracks an error event
    /// </summary>
    /// <param name="errorType">Type of error</param>
    /// <param name="errorMessage">Error message</param>
    public void TrackError(string errorType, string errorMessage)
    {
        try
        {
            Dictionary<string, string> eventValues = new Dictionary<string, string>
            {
                { "error_type", errorType },
                { "error_message", errorMessage }
            };
            
            LogEvent(EventNames.ERROR, eventValues);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error tracking error event: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Generic method to log any event to AppsFlyer
    /// </summary>
    /// <param name="eventName">Name of the event</param>
    /// <param name="eventValues">Dictionary of event parameters</param>
    public void LogEvent(string eventName, Dictionary<string, string> eventValues = null)
    {
        try
        {
            // Log for debugging
            if (eventValues != null)
            {
                string paramString = string.Join(", ", Array.ConvertAll(
                    System.Linq.Enumerable.ToArray(eventValues), 
                    item => item.Key + ": " + item.Value));
                
                Debug.Log($"Sending AppsFlyer event: {eventName} with params: {paramString}");
            }
            else
            {
                Debug.Log($"Sending AppsFlyer event: {eventName} with no params");
            }
            
            AppsFlyer.sendEvent(eventName, eventValues);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending AppsFlyer event {eventName}: {ex.Message}");
        }
    }
    
    #endregion
}
