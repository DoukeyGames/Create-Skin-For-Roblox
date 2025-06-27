using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_IOS
// Include the IosSupport namespace if running on iOS:
using Unity.Advertisement.IosSupport;
#endif

public class IOSATTService : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_IOS
        // Check the user's consent status and log it
        var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
        Debug.Log("ATT Status at startup: " + status);

        // If the status is undetermined, display the request:
        if(status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED) {
            Debug.Log("Requesting ATT permission...");
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        } else if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED) {
            Debug.Log("ATT permission already granted");
        } else if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.DENIED) {
            Debug.Log("ATT permission denied by user");
        } else if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.RESTRICTED) {
            Debug.Log("ATT permission restricted (parental controls)");
        }
#endif

        // Add a small delay before loading the next scene to ensure ATT dialog is shown
        Invoke("LoadNextScene", 0.5f);
    }

    private void LoadNextScene()
    {
#if UNITY_IOS
        // Log the status again after potential user interaction
        var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
        Debug.Log("ATT Status after potential prompt: " + status);
#endif
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}