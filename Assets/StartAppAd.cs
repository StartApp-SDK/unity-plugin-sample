using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using StartApp;

public class StartAppAd : MonoBehaviour
{
    GUIStyle mGuiStyle;
    Rect mShowFullscreenButton;
    Rect mShowOfferwallButton;
    Rect mShowRewardedVideoButton;
    Rect mShowBannersButton;
    Rect mShowPersonalizedAdsButton;
    bool mStartAppInitialized;

    void Start()
    {
        InitStartAppSdkAccordingToConsent(() =>
        {
            mStartAppInitialized = true;
        });

        Debug.Log("StartAppSDK start initializing");
    }

    void OnGUI()
    {
        if (!mStartAppInitialized)
        {
            return;
        }

        InitializeButtons();

        /* STARTAPP ADS */
        AddShowFullscreenButton(mShowFullscreenButton);
        AddShowOfferwallButton(mShowOfferwallButton);
        AddShowRewardedVideoButton(mShowRewardedVideoButton);
        AddShowBannersButton(mShowBannersButton);
        AddShowPersonalizedAdsButton(mShowPersonalizedAdsButton);
    }

    void ShowGdprDialog(Action callback)
    {
        ModalDialog.Instance().Choice(
            () =>
            {
                if (callback != null)
                {
                    callback();
                }
                WritePersonalizedAdsConsent(true);
            },
            () =>
            {
                if (callback != null)
                {
                    callback();
                }
                WritePersonalizedAdsConsent(false);
            });
    }

    void WritePersonalizedAdsConsent(bool isGranted)
    {
        Debug.Log("StartAppSDK setUserConsent: " + isGranted);

        AdSdk.Instance.SetUserConsent(
                            "pas",
                            isGranted,
                            (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);

        PlayerPrefs.SetInt("gdpr_dialog_was_shown", 1);
        PlayerPrefs.Save();
    }

    void InitStartAppSdkAccordingToConsent(Action callback)
    {
        if (PlayerPrefs.HasKey("gdpr_dialog_was_shown"))
        {
            AdSdk.Instance.ShowSplash();
            callback();
            return;
        }

        ShowGdprDialog(callback);
    }

    private InterstitialAd LoadInterstitial()
    {
        var ad = AdSdk.Instance.CreateInterstitial();

        ad.RaiseAdLoaded += (sender, e) =>
        {
            Debug.Log("Ad loaded");
            ad.ShowAd();
        };

        ad.RaiseAdLoadingFailed += (sender, e) =>
        {
            Debug.Log(string.Format("Error {0}", e.Message));
        };

        ad.RaiseAdShown += (sender, e) => Debug.Log("Ad shown");
        ad.RaiseAdClosed += (sender, e) => Debug.Log("Ad closed");
        ad.RaiseAdClicked += (sender, e) => Debug.Log("Ad clicked");

        return ad;
    }

    public void AddShowFullscreenButton(Rect showFullscreenButton)
    {
        if (GUI.Button(showFullscreenButton, "Show Fullscreen", mGuiStyle))
        {
            var ad = LoadInterstitial();
            ad.LoadAd(InterstitialAd.AdType.FullScreen);
        }
    }

    public void AddShowOfferwallButton(Rect showOfferwallButton)
    {
        if (GUI.Button(showOfferwallButton, "Show Offerwall", mGuiStyle))
        {
            var ad = LoadInterstitial();
            ad.LoadAd(InterstitialAd.AdType.OfferWall);
        }
    }

    public void AddShowRewardedVideoButton(Rect showRewardedVideoButton)
    {
        if (GUI.Button(showRewardedVideoButton, "Show Rewarded Video", mGuiStyle))
        {
            var ad = LoadInterstitial();
            ad.RaiseAdVideoCompleted += (sender, e) => Debug.Log("Ad video completed");
            ad.LoadAd(InterstitialAd.AdType.Rewarded);
        }
    }

    public void AddShowBannersButton(Rect showBannersButton)
    {
        if (GUI.Button(showBannersButton, "Show Banners", mGuiStyle))
        {
            SceneManager.LoadScene("Banners", LoadSceneMode.Single);
        }
    }

    public void AddShowPersonalizedAdsButton(Rect showPersonalizedAdsButton)
    {
        if (GUI.Button(showPersonalizedAdsButton, "Personalized Ads Setting", mGuiStyle))
        {
            mStartAppInitialized = false;
            ShowGdprDialog(() =>
            {
                mStartAppInitialized = true;
            });
        }
    }

    public void InitializeButtons()
    {
        /* Determine buttons size */
        int buttonHeight = Screen.height / 7;
        mShowFullscreenButton = new Rect(0, buttonHeight, Screen.width, buttonHeight);
        mShowOfferwallButton = new Rect(0, 2 * buttonHeight, Screen.width, buttonHeight);
        mShowRewardedVideoButton = new Rect(0, 3 * buttonHeight, Screen.width, buttonHeight);
        mShowBannersButton = new Rect(0, 4 * buttonHeight, Screen.width, buttonHeight);
        mShowPersonalizedAdsButton = new Rect(0, 5 * buttonHeight, Screen.width, buttonHeight);

        /* Change text size and logo size according to screen orientation */
        mGuiStyle = new GUIStyle(GUI.skin.button);
        if (Screen.orientation == ScreenOrientation.Portrait)
        {
            mGuiStyle.fontSize = Screen.width / 14;
        }
        else
        {
            mGuiStyle.fontSize = Screen.height / 14;
        }
    }
}
