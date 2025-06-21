using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using UnityEditor.iOS.Xcode;
using System.IO;

public class AutoConfigureSKAN
{
    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            // Get plist
            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            // Get root
            PlistElementDict rootDict = plist.root;

            // Change value of CFBundleVersion in Xcode plist
            var key = "NSAdvertisingAttributionReportEndpoint ";
            string value = "https://appsflyer-skadnetwork.com/";
            rootDict.SetString(key, value);
            Debug.Log("Auto configure SKAN");
            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}   
