using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using AppsFlyerSDK;

public class SettingController : MonoBehaviour
{

    public string shareMessage;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void Share()
    {
        shareMessage = "Hey Check Out this New Roblox Game";

        // Track share attempt
        AnalyticsManager.Instance.LogEvent(AnalyticsManager.EventNames.SHARE,
            new Dictionary<string, string> { { "method", "native_share" } });

        StartCoroutine( TakeScreenshotAndShare() );
    }

    public void RestorePurchase()
    {
        // Track restore purchase attempt
        AnalyticsManager.Instance.LogEvent(AnalyticsManager.EventNames.RESTORE_PURCHASE);

        // Call the actual restore purchase method from IAPManager
        if (IAPManager.Instance != null)
        {
            IAPManager.Instance.RestorePurchases();
        }
    }

    public void Youtube()
    {
        // Track social link click
        AnalyticsManager.Instance.TrackFeatureUsed("social_link",
            new Dictionary<string, string> { { "platform", "youtube" } });

        Application.OpenURL("https://www.youtube.com/@Chaseroony");
    }

    public void Tiktok()
    {
        // Track social link click
        AnalyticsManager.Instance.TrackFeatureUsed("social_link",
            new Dictionary<string, string> { { "platform", "tiktok" } });

        Application.OpenURL("https://www.tiktok.com/@sofiatube07");
    }

    public void Discord()
    {
        // Track social link click
        AnalyticsManager.Instance.TrackFeatureUsed("social_link",
            new Dictionary<string, string> { { "platform", "discord" } });

        Application.OpenURL("https://discord.gg/BjaJeSrB");
    }

    public void Facebook()
    {
        // Track social link click
        AnalyticsManager.Instance.TrackFeatureUsed("social_link",
            new Dictionary<string, string> { { "platform", "facebook" } });
        Application.OpenURL("https://www.facebook.com/sofiatube07");
    }



    private IEnumerator TakeScreenshotAndShare()
    {
        yield return new WaitForEndOfFrame();

        // Texture2D ss = new Texture2D( Screen.width, Screen.height, TextureFormat.RGB24, false );
        // ss.ReadPixels( new Rect( 0, 0, Screen.width, Screen.height ), 0, 0 );
        // ss.Apply();
        //
        // string filePath = Path.Combine( Application.temporaryCachePath, "shared img.png" );
        // File.WriteAllBytes( filePath, ss.EncodeToPNG() );
        //
        // // To avoid memory leaks
        // Destroy( ss );

        new NativeShare()//.AddFile( filePath )
            .SetText( shareMessage )
            .SetCallback( ( result, shareTarget ) => {
                Debug.Log( "Share result: " + result + ", selected app: " + shareTarget );

                // Track share result
                Dictionary<string, string> shareResultParams = new Dictionary<string, string>
                {
                    { "result", result.ToString() },
                    { "target_app", shareTarget ?? "unknown" }
                };
                AnalyticsManager.Instance.LogEvent(AnalyticsManager.EventNames.SHARE, shareResultParams);
            })
            .Share();

        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();
    }
}
