using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabPaywall : TabBase
{
    private TabButton selectedTab;
    private bool isTabSelected = false;
    [SerializeField] private Button nextBtn, privacyBtn, tocBtn, restoreBtn;
    [SerializeField] private GameObject weekly, freeTrial;
    [SerializeField] private TMP_Text freeTrialText, trialSafetyText;

    [SerializeField] private Toggle toggle;

    private const string _privacyTextUrl = "https://gravloom.com/privacy-policy-2/";
    private const string _permOfConditionUrl = "https://gravloom.com/terms-of-use-2/";

    private void Start()
    {
        nextBtn.interactable = false;
        nextBtn.onClick.AddListener(OnNextButtonClick);
        privacyBtn.onClick.AddListener(() => OpenURL(_privacyTextUrl));
        tocBtn.onClick.AddListener(() => OpenURL(_permOfConditionUrl));
        restoreBtn.onClick.AddListener(RestorePurchases);

        SetDefaultSelection();

        toggle.onValueChanged.AddListener(OnToggleChanged);

        if (AnalyticsManager.Instance != null)
        {
            AnalyticsManager.Instance.TrackPaywallView("tab_paywall", "app_flow");
        }
    }

    private void SetDefaultSelection()
    {
        selectedTab = toggle.isOn ? tabButtons[1] : tabButtons[0];
        isTabSelected = true;
        ResetTabs();
    }

    private void OnToggleChanged(bool isOn)
    {
        // Only change selection when the toggle is changed
        selectedTab = isOn ? tabButtons[1] : tabButtons[0];
        ResetTabs();

        // Track subscription selection with AppsFlyer
        if (AnalyticsManager.Instance != null)
        {
            string subscriptionType = isOn ? "free_trial" : "yearly_intro";
            AnalyticsManager.Instance.TrackPaywallInteraction("toggle_change", "subscription_toggle",
                new Dictionary<string, string> { { "selected_type", subscriptionType } });
        }
    }

    public override void Subscribe(TabButton button) {}

    public override void OnTabEnter(TabButton button) => ResetTabs();

    public override void OnTabExit(TabButton button) => ResetTabs();

    public override void OnTabSelected(TabButton button)
    {
        selectedTab = button;
        isTabSelected = true;
        ResetTabs();

        // Track tab selection with AppsFlyer
        if (AnalyticsManager.Instance != null)
        {
            int selectedIndex = tabButtons.IndexOf(button);
            string subscriptionType = selectedIndex == 0 ? "yearly_intro" : "free_trial";
            AnalyticsManager.Instance.TrackSubscriptionSelection(subscriptionType);
        }
    }

    public void ResetTabs()
    {
        foreach (TabButton button in tabButtons)
        {
            bool isSelected = button == selectedTab;
            button.SetImage(isSelected);
            nextBtn.interactable = true;
        }
    }

    private void Update()
    {
        // Only update UI elements, no selection logic here
        if (toggle.isOn)
        {
            weekly.SetActive(false);
            freeTrial.SetActive(true);
            freeTrialText.text = "Start No-Commitment Trial";
            trialSafetyText.text = "No Payment Now";
        }
        else
        {
            weekly.SetActive(true);
            freeTrial.SetActive(false);
            freeTrialText.text = "CONTINUE";
            trialSafetyText.text = "CANCEL ANYTIME";
        }
    }

    private void RestorePurchases()
    {
        // Track restore attempt with AppsFlyer
        if (AnalyticsManager.Instance != null)
        {
            AnalyticsManager.Instance.LogEvent(AnalyticsManager.EventNames.RESTORE_PURCHASE,
                new Dictionary<string, string> { { "source", "tab_paywall" } });
        }

        IAPManager.Instance.RestorePurchases();
    }

    private void OnNextButtonClick()
    {
        nextBtn.interactable = false;

        // Track next button click with AppsFlyer
        if (AnalyticsManager.Instance != null)
        {
            AnalyticsManager.Instance.TrackPaywallInteraction("button_click", "next_button");
        }

        if (isTabSelected && selectedTab != null)
        {
            int selectedIndex = tabButtons.IndexOf(selectedTab);
            if (selectedIndex == 0)
            {
                // Track subscription attempt with AppsFlyer
                if (AnalyticsManager.Instance != null)
                {
                    AnalyticsManager.Instance.LogEvent(AnalyticsManager.EventNames.SUBSCRIPTION_ATTEMPT,
                        new Dictionary<string, string> { { "subscription_type", "yearly_intro" } });
                }
                IAPManager.Instance.BuyYealyIntroductSubscription();
            }
            else if (selectedIndex == 1)
            {
                // Track subscription attempt with AppsFlyer
                if (AnalyticsManager.Instance != null)
                {
                    AnalyticsManager.Instance.LogEvent(AnalyticsManager.EventNames.SUBSCRIPTION_ATTEMPT,
                        new Dictionary<string, string> { { "subscription_type", "free_trial" } });
                }
                IAPManager.Instance.BuyFreeTrialSubscription();
            }
            else if (selectedIndex == 2)
            {
                // Track subscription attempt with AppsFlyer
                if (AnalyticsManager.Instance != null)
                {
                    AnalyticsManager.Instance.LogEvent(AnalyticsManager.EventNames.SUBSCRIPTION_ATTEMPT,
                        new Dictionary<string, string> { { "subscription_type", "yearly" } });
                }
                IAPManager.Instance.BuyYearlySubscription();
            }
        }
    }

    private void OpenURL(string url)
    {
        if (AnalyticsManager.Instance != null)
        {
            string urlType = url.Contains("privacy") ? "privacy_policy" : "terms_of_service";
            AnalyticsManager.Instance.TrackPaywallInteraction("url_open", urlType,
                new Dictionary<string, string> { { "url", url } });
        }

        Application.OpenURL(url);
    }
}
