using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullAccess : MonoBehaviour
{
    [SerializeField] private string privacyURL, termsURL;
    public GameObject[] Highlight;
    public GameObject weekly, freeTrial;
    public Toggle toggle;
    [SerializeField] private Button restoreBtn;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button privacyBtn, termsBtn;

    private int highlightIndex = 0; // Track current selection

    void Start()
    {
        ChoosePackage(highlightIndex);
        restoreBtn.onClick.AddListener(() => RestorePurchases());
	privacyBtn.onClick.AddListener(() => OpenURL(privacyURL));
	termsBtn.onClick.AddListener(() => OpenURL(termsURL));
        nextButton.onClick.AddListener(NextPackage);
        freeTrial.SetActive(false);

        // Track paywall view with AppsFlyer
        if (AnalyticsManager.Instance != null)
        {
            AnalyticsManager.Instance.TrackPaywallView("full_access_paywall", "app_flow");
        }
    }

    private void RestorePurchases()
    {
        // Track restore attempt with AppsFlyer
        if (AnalyticsManager.Instance != null)
        {
            AnalyticsManager.Instance.LogEvent(AnalyticsManager.EventNames.RESTORE_PURCHASE,
                new Dictionary<string, string> { { "source", "full_access_panel" } });
        }

        IAPManager.Instance.RestorePurchases();
    }

    private void OpenURL(string url)
    {
        // Track URL opening with AppsFlyer
        if (AnalyticsManager.Instance != null)
        {
            string urlType = url.Contains("privacy") ? "privacy_policy" : "terms_of_service";
            AnalyticsManager.Instance.TrackPaywallInteraction("url_open", urlType,
                new Dictionary<string, string> { { "url", url }, { "source", "full_access_panel" } });
        }

        Application.OpenURL(url);
    }

    public void ChoosePackage(int i)
    {
        for (int j = 0; j < Highlight.Length; j++)
        {
            Highlight[j].SetActive(false);
        }
        Highlight[i].SetActive(true);

        // Handle UI visibility based on selected index
        if (i == 1) // Weekly
        {
            weekly.SetActive(true);
            freeTrial.SetActive(false);
        }
        else if (i == 2) // Free Trial
        {
            weekly.SetActive(false);
            freeTrial.SetActive(true);
        }
    }

    private void Update()
    {
        if(toggle.isOn)
        {
            weekly.SetActive(false);
            freeTrial.SetActive(true);
        }
        else
        {
            weekly.SetActive(true);
            freeTrial.SetActive(false);
        }
    }

    public void NextPackage()
    {
        // Process purchase based on the current selection

        Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventJoinGroup,
            Firebase.Analytics.FirebaseAnalytics.ParameterGroupID, "paywall");
// #if UNITY_IOS && !UNITY_EDITOR

// #endif

        // Track paywall interaction with AppsFlyer
        if (AnalyticsManager.Instance != null)
        {
            AnalyticsManager.Instance.TrackPaywallInteraction("button_click", "next_package_button");
        }

#if UNITY_ANDROID
        MenuPanelController.Instance.ActivePanel(6);
#endif

        ProcessPurchase();

        // Cycle to the next package
        highlightIndex++;

        if (highlightIndex > 2)
        {
            highlightIndex = 0;
        }

        ChoosePackage(highlightIndex);
    }

    private void ProcessPurchase()
    {
        switch (highlightIndex)
        {
            case 0:
                Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventJoinGroup,
                    Firebase.Analytics.FirebaseAnalytics.ParameterGroupID, "yearly_access_successful");
        // #if UNITY_IOS && !UNITY_EDITOR

        // #endif
                // Track subscription attempt with AppsFlyer
                if (AnalyticsManager.Instance != null)
                {
                    AnalyticsManager.Instance.LogEvent(AnalyticsManager.EventNames.SUBSCRIPTION_ATTEMPT,
                        new Dictionary<string, string> { { "subscription_type", "yearly" }, { "source", "full_access_panel" } });
                }
                Debug.Log("Subscription purchase triggered.");
                IAPManager.Instance.BuyYearlySubscription();
                break;
            case 1:
                Debug.Log($"Weekly subs");
                // Track weekly subscription attempt with AppsFlyer
                if (AnalyticsManager.Instance != null)
                {
                    AnalyticsManager.Instance.LogEvent(AnalyticsManager.EventNames.SUBSCRIPTION_ATTEMPT,
                        new Dictionary<string, string> { { "subscription_type", "weekly" }, { "source", "full_access_panel" } });
                }
                break;
            case 2:
            Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventJoinGroup,
                    Firebase.Analytics.FirebaseAnalytics.ParameterGroupID, "free_access_successful");
        // #if UNITY_IOS && !UNITY_EDITOR

        // #endif
                // Track free trial attempt with AppsFlyer
                if (AnalyticsManager.Instance != null)
                {
                    AnalyticsManager.Instance.LogEvent(AnalyticsManager.EventNames.SUBSCRIPTION_ATTEMPT,
                        new Dictionary<string, string> { { "subscription_type", "free_trial" }, { "source", "full_access_panel" } });
                }
                Debug.Log("Subscription purchase triggered.");
                IAPManager.Instance.BuyFreeTrialSubscription();
                break;
        }
    }
}
