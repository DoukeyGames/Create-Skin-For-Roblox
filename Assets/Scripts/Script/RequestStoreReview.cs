using System;
using UnityEngine;
using UnityEngine.iOS;


public class RequestStoreReview : MonoBehaviour
{
    private const string ReviewRequestedKey = "ReviewRequested";

    private void Start() => RequestReview();

    void RequestReview()
    {
        if (PlayerPrefs.GetInt(ReviewRequestedKey, 0) == 0)
        {
            bool popupShown = Device.RequestStoreReview();
            if (popupShown)
            {
                PlayerPrefs.SetInt(ReviewRequestedKey, 1);
                PlayerPrefs.Save();
            }
            else
                Debug.Log("iOS version is too low or StoreKit framework was not linked.");
        }
    }
}
