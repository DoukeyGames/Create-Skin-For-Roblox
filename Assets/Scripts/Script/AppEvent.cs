using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;

public class AppEvent : MonoBehaviour
{
    private void Start()
    {
        #if UNITY_IOS
        string appId = "id6738957228"; // Your iOS App Store ID
#else
        string appId = ""; // Leave blank for Android
#endif
        string devKey = "qZkSwZ7wLyeaEMxvFJm5J5";

        // Enable debug logging to help diagnose issues
        AppsFlyer.setIsDebug(true);

        // Initialize the SDK first
        AppsFlyer.initSDK(devKey, appId, this);
        AppsFlyer.startSDK();

        // Track app open event
        Dictionary<string, string> appOpenParams = new Dictionary<string, string>
        {
            { "app_version", Application.version }
        };
        AppsFlyer.sendEvent(AnalyticsManager.EventNames.APP_OPEN, appOpenParams);

        // Track login event
        AppsFlyer.sendEvent(AFInAppEvents.LOGIN, null);

        // Log ATT status for debugging
        #if UNITY_IOS
        CheckATTStatus();
        #endif
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // App is going to background
            Dictionary<string, string> backgroundParams = new Dictionary<string, string>
            {
                { "session_duration", Time.time.ToString() }
            };

            if (AnalyticsManager.Instance != null)
            {
                AnalyticsManager.Instance.TrackAppLifecycle(AnalyticsManager.EventNames.APP_BACKGROUND, backgroundParams);
            }
            else
            {
                AppsFlyer.sendEvent(AnalyticsManager.EventNames.APP_BACKGROUND, backgroundParams);
            }
        }
        else
        {
            // App is returning from background (resuming)
            Dictionary<string, string> resumeParams = new Dictionary<string, string>
            {
                { "from_background", "true" },
                { "app_version", Application.version }
            };

            if (AnalyticsManager.Instance != null)
            {
                AnalyticsManager.Instance.TrackAppLifecycle(AnalyticsManager.EventNames.APP_FOREGROUND, resumeParams);
                // Also track as app open for continuity
                AnalyticsManager.Instance.LogEvent(AnalyticsManager.EventNames.APP_OPEN, resumeParams);
            }
            else
            {
                AppsFlyer.sendEvent(AnalyticsManager.EventNames.APP_FOREGROUND, resumeParams);
                AppsFlyer.sendEvent(AnalyticsManager.EventNames.APP_OPEN, resumeParams);
            }
        }
    }

    #if UNITY_IOS
    private void CheckATTStatus()
    {
        // Check and log the current ATT status
        var status = Unity.Advertisement.IosSupport.ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
        Debug.Log("AppsFlyer: Current ATT status: " + status);
    }
    #endif

    public void onConversionDataSuccess(string conversionData)
    {
        Debug.Log("AppsFlyer conversion data: " + conversionData);
    }

    public void onConversionDataFail(string error)
    {
        Debug.Log("AppsFlyer conversion data error: " + error);

        // Track error with analytics manager
        AnalyticsManager.Instance.TrackError("conversion_data", error);
    }

    public void onAppOpenAttribution(string attributionData)
    {
        Debug.Log("AppsFlyer attribution: " + attributionData);
    }

    public void onAppOpenAttributionFailure(string error)
    {
        Debug.Log("AppsFlyer attribution error: " + error);

        // Track error with analytics manager
        AnalyticsManager.Instance.TrackError("attribution", error);
    }
}
