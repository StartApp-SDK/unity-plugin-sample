/*
Copyright 2019 StartApp Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

#if UNITY_ANDROID

using System;
using UnityEngine;

namespace StartApp
{
    public class AdSdkAndroid : AdSdk
    {
        public static AdSdkAndroid ImplInstance
        {
            get
            {
                return (AdSdkAndroid)Instance;
            }
        }

        static readonly string EXIT_AD_TAG = "exit_ad";

        string mAccountId;
        string mApplicationId;
        bool mEnableReturnAds = true;
        bool mIsAccountIdUsed;
        bool mWasSplashShown;

        public AndroidJavaObject Activity { get; private set; }

        public void Setup()
        {
            if (!string.IsNullOrEmpty(mApplicationId))
            {
                return;
            }

            if (!ReadDataFromTextFile())
            {
                throw new ArgumentException("Error in initializing Application ID, Account ID or Return Ads. Please verify your StartAppData.txt file.");
            }

            if (mIsAccountIdUsed)
            {
                Setup(mAccountId, mApplicationId, mEnableReturnAds);
            }
            else
            {
                Setup(mApplicationId, mEnableReturnAds);
            }
        }

        void Setup(string appId, bool returnAds)
        {
            Setup(null, appId, returnAds);
        }

        void Setup(string accId, string appId, bool returnAds)
        {
            var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            Activity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");

            var jAppId = new AndroidJavaObject("java.lang.String", appId);
            var sdk = new AndroidJavaClass("com.startapp.sdk.adsbase.StartAppSDK");

            var wrapperName = new AndroidJavaObject("java.lang.String", "Unity");
            var wrapperVer = new AndroidJavaObject("java.lang.String", WrapperVersion);
            sdk.CallStatic("addWrapper", Activity, wrapperName, wrapperVer);

            var ad = new AndroidJavaClass("com.startapp.sdk.adsbase.StartAppAd");
            ad.CallStatic("disableSplash");

            if (string.IsNullOrEmpty(accId))
            {
                Debug.Log("App ID: " + appId);
                Debug.Log("Enable Return Ads : " + mEnableReturnAds);
                sdk.CallStatic("init", Activity, jAppId, returnAds);
            }
            else
            {
                Debug.Log("Account ID: " + accId);
                Debug.Log("App ID: " + appId);
                Debug.Log("Enable Return Ads : " + mEnableReturnAds);
                AndroidJavaObject jAccId = new AndroidJavaObject("java.lang.String", accId);
                sdk.CallStatic("init", Activity, jAccId, jAppId, returnAds);
            }
        }

        public override InterstitialAd CreateInterstitial()
        {
            return new InterstitialAdAndroid();
        }

        public override BannerAd CreateBanner()
        {
            return new BannerAdAndroid();
        }

        public override void DisableReturnAds()
        {
            Setup();
            mEnableReturnAds = false;
        }

        public override void SetUserConsent(string consentType, bool enabled, long timestamp)
        {
            Setup();
            AndroidJavaObject objConsentType = new AndroidJavaObject("java.lang.String", consentType);
            var sdk = new AndroidJavaClass("com.startapp.sdk.adsbase.StartAppSDK");
            sdk.CallStatic("setUserConsent", Activity, objConsentType, timestamp, enabled);
        }

        public override void ShowSplash(SplashConfig config)
        {
            if (mWasSplashShown)
            {
                return;
            }

            Setup();

            var jad = new AndroidJavaClass("com.startapp.sdk.adsbase.StartAppAd");
            if (config == null)
            {
                jad.CallStatic("showSplash", Activity, null);
                mWasSplashShown = true;
                return;
            }

            var jconfig = new AndroidJavaObject("com.startapp.sdk.ads.splash.SplashConfig");

            if (config.AppName != null)
            {
                var jappName = new AndroidJavaObject("java.lang.String", config.AppName);
                jconfig.Call<AndroidJavaObject>("setAppName", jappName);
            }

            if (config.LogoFileName != null)
            {
                byte[] logoByteArray = null;
                var logoTexture = Resources.Load(config.LogoFileName) as Texture2D;
                if (logoTexture != null)
                {
                    logoByteArray = logoTexture.EncodeToPNG();
                }
                jconfig.Call<AndroidJavaObject>("setLogo", logoByteArray);
            }

            int themeIndex = (int)config.TemplateTheme + 1; // +1 for Android because the theme enum starts from 1
            var themeClass = new AndroidJavaClass("com.startapp.sdk.ads.splash.SplashConfig$Theme");
            jconfig.Call<AndroidJavaObject>("setTheme", themeClass.CallStatic<AndroidJavaObject>("getByIndex", themeIndex));

            int orientationIndex = (int)config.ScreenOrientation + 1; // +1 for Android because the orientation enum starts from 1
            var orientationClass = new AndroidJavaClass("com.startapp.sdk.ads.splash.SplashConfig$Orientation");
            jconfig.Call<AndroidJavaObject>("setOrientation", orientationClass.CallStatic<AndroidJavaObject>("getByIndex", orientationIndex));

            jad.CallStatic("showSplash", Activity, null, jconfig);
            mWasSplashShown = true;
        }

        public override bool OnBackPressed()
        {
            var backAd = new InterstitialAdAndroid();
            bool clicked = false;

            backAd.RaiseAdClosed += (sender, e) => {
                if (!clicked)
                {
                    Application.Quit();
                }
            };

            backAd.RaiseAdClicked += (sender, e) => clicked = true;

            return backAd.ShowAd(EXIT_AD_TAG);
        }

        bool ReadDataFromTextFile()
        {
            bool result = false;
            int assigned = 0;

            var data = Resources.Load("StartAppDataAndroid") as TextAsset;
            string userData = data.ToString();

            string[] splitData = userData.Split('\n');
            string[] singleData;

            for (int i = 0; i < splitData.Length; i++)
            {
                singleData = splitData[i].Split('=');
                if (singleData[0].ToLower().CompareTo("applicationid") == 0)
                {
                    assigned++;
                    mApplicationId = singleData[1].Trim();
                }

                // Account ID is optional
                if (singleData[0].ToLower().CompareTo("accountid") == 0 || singleData[0].ToLower().CompareTo("developerid") == 0)
                {
                    mIsAccountIdUsed = true;
                    mAccountId = singleData[1].Trim();
                }

                if (singleData[0].ToLower().CompareTo("returnads") == 0)
                {
                    if (singleData[1].ToLower().Equals("false"))
                    {
                        assigned++;
                        DisableReturnAds();
                    }
                }
            }

            RemoveSpecialCharacters();
            if ((mEnableReturnAds && assigned == 1) || (!mEnableReturnAds && assigned == 2))
            {
                Debug.Log("Initialization successful");
                Debug.Log("Application ID: " + mApplicationId);
                if (mIsAccountIdUsed)
                {
                    Debug.Log("Account ID: " + mAccountId);
                }
                if (mEnableReturnAds)
                {
                    Debug.Log("Return ads are enabled");
                }
                result = true;
            }
            return result;
        }

        void RemoveSpecialCharacters()
        {
            if (mApplicationId != null && mApplicationId.IndexOf("\"") != -1)
            {
                mApplicationId = mApplicationId.Replace("\"", "");
            }

            if (mIsAccountIdUsed && mAccountId != null && mAccountId.IndexOf("\"") != -1)
            {
                mAccountId = mAccountId.Replace("\"", "");
            }
        }
    }
}

#endif