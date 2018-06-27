using UnityEngine;
using UnityEditor;

public class SteamworksMenu
{

#if UNITY_5 || UNITY_2017 || UNITY_2017_1_OR_NEWER

    [MenuItem("Steamworks.NET/Set Platform Settings")]
    public static void CallSetPlatformSettings()
    {
        RedistInstall.SetPlatformSettings();
    }

    [MenuItem("Steamworks.NET/Add Steamworks Define Symbols")]
    public static void CallSetScriptingDefineSymbols()
    {
        RedistInstall.SetScriptingDefineSymbols();
    }

#endif // UNITY_5 || UNITY_2017 || UNITY_2017_1_OR_NEWER

}
