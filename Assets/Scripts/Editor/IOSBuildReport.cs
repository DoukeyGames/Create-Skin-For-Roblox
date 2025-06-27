#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine; // Added for Debug.Log
using System.IO;
using System.Collections.Generic;

public class IOSBuildReport {
    // Set the IDFA request description:
    const string k_TrackingDescription = "Your data will be used to provide you a better and personalized ad experience.";

    // AppsFlyer Dev Key
    const string k_AppsFlyerDevKey = "qZkSwZ7wLyeaEMxvFJm5J5";

    // iOS App ID
    const string k_AppStoreAppID = "id6741786715";

    [PostProcessBuild(0)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToXcode) {
        if (buildTarget == BuildTarget.iOS) {
            AddPListValues(pathToXcode);
            UpdateCapabilities(pathToXcode);
        }
    }

    // Implement a function to read and write values to the plist file:
    static void AddPListValues(string pathToXcode) {
        // Retrieve the plist file from the Xcode project directory:
        string plistPath = pathToXcode + "/Info.plist";
        PlistDocument plistObj = new PlistDocument();

        // Read the values from the plist file:
        plistObj.ReadFromString(File.ReadAllText(plistPath));

        // Set values from the root object:
        PlistElementDict plistRoot = plistObj.root;

        // Set the description key-value in the plist:
        plistRoot.SetString("NSUserTrackingUsageDescription", k_TrackingDescription);

        // Add AppsFlyer specific settings
        plistRoot.SetString("AppsFlyerDevKey", k_AppsFlyerDevKey);
        plistRoot.SetString("appleAppID", k_AppStoreAppID);

        // Enable SKAdNetwork for iOS 14+ attribution
        PlistElementArray skAdNetworkItems = plistRoot.CreateArray("SKAdNetworkItems");

        // Add AppsFlyer's SKAdNetwork ID
        PlistElementDict skAdNetworkItem = skAdNetworkItems.AddDict();
        skAdNetworkItem.SetString("SKAdNetworkIdentifier", "su67r6k2v3.skadnetwork");

        // Save changes to the plist:
        File.WriteAllText(plistPath, plistObj.WriteToString());

        Debug.Log("AppsFlyer: Added required keys to Info.plist");
    }

    static void UpdateCapabilities(string pathToXcode) {
        string projectPath = PBXProject.GetPBXProjectPath(pathToXcode);
        PBXProject project = new PBXProject();
        project.ReadFromFile(projectPath);

        // Get the main target GUID
#if UNITY_2019_3_OR_NEWER
        string targetGuid = project.GetUnityMainTargetGuid();
#else
        string targetGuid = project.TargetGuidByName(PBXProject.GetUnityTargetName());
#endif

        // Add required frameworks for AppsFlyer
        project.AddFrameworkToProject(targetGuid, "AdSupport.framework", false);
        project.AddFrameworkToProject(targetGuid, "AppTrackingTransparency.framework", false);
        project.AddFrameworkToProject(targetGuid, "iAd.framework", false);

        // Save the changes
        project.WriteToFile(projectPath);

        Debug.Log("AppsFlyer: Added required frameworks to Xcode project");
    }
}
#endif