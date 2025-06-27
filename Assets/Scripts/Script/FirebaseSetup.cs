using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseSetup : MonoBehaviour
{
    private void Awake()
    {
        // Firebase Analytics tracking
        Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLogin);
        Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventJoinGroup,
            Firebase.Analytics.FirebaseAnalytics.ParameterGroupID, "app_started");

        // AppsFlyer tracking - ensure AnalyticsManager is available
        if (AnalyticsManager.Instance != null)
        {
            // Track login event with AppsFlyer
            AnalyticsManager.Instance.LogEvent(AnalyticsManager.EventNames.LOGIN);

            // Track app started event with AppsFlyer
            Dictionary<string, string> appStartedParams = new Dictionary<string, string>
            {
                { "source", "firebase_setup" },
                { "app_version", Application.version }
            };
            AnalyticsManager.Instance.LogEvent(AnalyticsManager.EventNames.APP_STARTED, appStartedParams);
        }
        else
        {
            Debug.LogWarning("AnalyticsManager not available during FirebaseSetup. AppsFlyer events will be tracked later.");
        }
    }
}
