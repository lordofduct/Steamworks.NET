using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SteamworksLocationHelper : MonoBehaviour
{

    //The guid of this specific class file. If imported via unityPackage correctly, this should always match.
    private const string GUID_SELF = "b97dd3981e8a37740a5c1a892404905b";
    //The guid of the base Steamworks.NET folder. If imported via unityPackage correctly, this should always match.
    private const string GUID_BASE = "db67b2c6826d4dc45bdf61ca5945a706";

    private static string _baseDirectory;

    static SteamworksLocationHelper()
    {
        _baseDirectory = AssetDatabase.GUIDToAssetPath(GUID_BASE);
    }

    /// <summary>
    /// Directory in which Steamworks.NET is placed.
    /// </summary>
    public static string BaseDirectory
    {
        get
        {
            return _baseDirectory;
        }
    }

    public static string PluginsDirectory
    {
        get
        {
            return _baseDirectory + @"/Plugins";
        }
    }

    public static string PluginX86Directory
    {
        get
        {
            return _baseDirectory + @"/Plugins/x86";
        }
    }

    public static string PluginX86_64Directory
    {
        get
        {
            return _baseDirectory + @"/Plugins/x64";
        }
    }

    public static string RedistPath
    {
        get
        {
            return _baseDirectory + "/Plugins/Steamworks.NET/redist";
        }
    }


}
