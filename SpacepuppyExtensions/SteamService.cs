﻿#if UNITY_ANDROID || UNITY_IOS || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_WSA || UNITY_PS4 || UNITY_WII || UNITY_XBOXONE
#define DISABLESTEAMWORKS
#endif

#if STEAMWORKS && !DISABLESTEAMWORKS

using UnityEngine;
using System.Collections.Generic;

using Steamworks;

namespace com.spacepuppy
{

    public class SteamService : ServiceComponent<SteamService>
    {

        // Once you get a Steam AppID assigned by Valve, you need to replace AppId_t.Invalid with it and
        // remove steam_appid.txt from the game depot. eg: "(AppId_t)480" or "new AppId_t(480)".
        // See the Valve documentation for more information: https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
        public static AppId_t APP_ID = AppId_t.Invalid;

        #region Fields

        private static bool s_everInialized;
        private bool _initialized;

        private SteamAPIWarningMessageHook_t _steamAPIWarningMessageHook;

        #endregion

        #region CONSTRUCTOR

        protected override void OnValidAwake()
        {
            base.OnValidAwake();

            if (s_everInialized)
            {
                // This is almost always an error.
                // The most common case where this happens is when SteamManager gets destroyed because of Application.Quit(),
                // and then some Steamworks code in some other OnDestroy gets called afterwards, creating a new SteamManager.
                // You should never call Steamworks functions in OnDestroy, always prefer OnDisable if possible.
                throw new System.Exception("Tried to Initialize the SteamAPI twice in one session!");
            }

            if (!Packsize.Test())
            {
                Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
            }

            if (!DllCheck.Test())
            {
                Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
            }

            try
            {
                // If Steam is not running or the game wasn't started through Steam, SteamAPI_RestartAppIfNecessary starts the
                // Steam client and also launches this game again if the User owns it. This can act as a rudimentary form of DRM.
                if (SteamAPI.RestartAppIfNecessary(APP_ID))
                {
                    Application.Quit();
                    return;
                }
            }
            catch (System.DllNotFoundException e)
            { // We catch this exception here, as it will be the first occurence of it.
                Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + e, this);

                Application.Quit();
                return;
            }

            // Initializes the Steamworks API.
            // If this returns false then this indicates one of the following conditions:
            // [*] The Steam client isn't running. A running Steam client is required to provide implementations of the various Steamworks interfaces.
            // [*] The Steam client couldn't determine the App ID of game. If you're running your application from the executable or debugger directly then you must have a [code-inline]steam_appid.txt[/code-inline] in your game directory next to the executable, with your app ID in it and nothing else. Steam will look for this file in the current working directory. If you are running your executable from a different directory you may need to relocate the [code-inline]steam_appid.txt[/code-inline] file.
            // [*] Your application is not running under the same OS user context as the Steam client, such as a different user or administration access level.
            // [*] Ensure that you own a license for the App ID on the currently active Steam account. Your game must show up in your Steam library.
            // [*] Your App ID is not completely set up, i.e. in [code-inline]Release State: Unavailable[/code-inline], or it's missing default packages.
            // Valve's documentation for this is located here:
            // https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
            _initialized = SteamAPI.Init();
            if (!_initialized)
            {
                Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);

                return;
            }

            s_everInialized = true;
        }

        #endregion

        #region Properties

        public bool Initialized
        {
            get { return _initialized; }
        }

        #endregion

        #region Methods

        // This should only ever get called on first load and after an Assembly reload, You should never Disable the Steamworks Manager yourself.
        protected override void Start()
        {
            base.Start();

            if (!_initialized) return;

            SteamClient.SetWarningMessageHook((severity, text) =>
            {
                if (text != null) Debug.LogWarning(text.ToString());
            });
        }

        // OnApplicationQuit gets called too early to shutdown the SteamAPI.
        // Because the SteamManager should be persistent and never disabled or destroyed we can shutdown the SteamAPI here.
        // Thus it is not recommended to perform any Steamworks work in other OnDestroy functions as the order of execution can not be garenteed upon Shutdown. Prefer OnDisable().
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (!_initialized) return;

            SteamAPI.Shutdown();
        }
        
        private void Update()
        {
            if (!_initialized) return;

            // Run Steam client callbacks
            SteamAPI.RunCallbacks();
        }

        #endregion

    }

}

#endif
