using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using StartApp;

public class StartAppAd : MonoBehaviour
{
	GUIStyle guiStyle;
	Rect showFullscreenButton;
	Rect showOfferwallButton;
	Rect showRewardedVideoButton;
	Rect showBannersButton;
	Rect showPersonalizedAdsButton;
	bool startAppInitialized;

	void Start()
    {
		initStartAppSdkAccordingToConsent(() => {
			startAppInitialized = true;
		});

		Debug.Log("StartAppSDK start initializing");
	}

	void OnGUI()
    {
		if (!startAppInitialized)
        {
			return;
		}

		initializeButtons();

		/* STARTAPP ADS */
		addShowFullscreenButton(showFullscreenButton);
		addShowOfferwallButton(showOfferwallButton);
		addShowRewardedVideoButton(showRewardedVideoButton);
		addShowBannersButton(showBannersButton);
		addShowPersonalizedAdsButton(showPersonalizedAdsButton);
	}

	void showGdprDialog(Action callback)
    {
		ModalDialog.Instance().Choice(
			() => {
				if (callback != null)
                {
					callback();
				}
				writePersonalizedAdsConsent(true);
			},
			() => {
				if (callback != null)
                {
					callback();
				}
				writePersonalizedAdsConsent(false);
			});
	}

	void writePersonalizedAdsConsent(bool isGranted)
    {
		Debug.Log("StartAppSDK setUserConsent: " + isGranted);

        AdSdk.Instance.SetUserConsent(
                            "pas",
                            isGranted,
                            (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);

        PlayerPrefs.SetInt("gdpr_dialog_was_shown", 1);
		PlayerPrefs.Save();
	}

	void initStartAppSdkAccordingToConsent(Action callback)
    {
		if (PlayerPrefs.HasKey("gdpr_dialog_was_shown"))
        {
            AdSdk.Instance.ShowSplash();
            callback();
			return;
		}

        showGdprDialog(callback);
	}

    private InterstitialAd loadInterstitial()
    {
        var ad = AdSdk.Instance.CreateInterstitial();

        ad.RaiseAdLoaded += (sender, e) => {
            Debug.Log("Ad loaded");
            ad.ShowAd();
        };

        ad.RaiseAdLoadingFailed += (sender, e) => {
            Debug.Log(string.Format("Error {0}", e.Message));
        };

        ad.RaiseAdShown += (sender, e) => Debug.Log("Ad shown");
        ad.RaiseAdClosed += (sender, e) => Debug.Log("Ad closed");
        ad.RaiseAdClicked += (sender, e) => Debug.Log("Ad clicked");

        return ad;
    }

    public void addShowFullscreenButton(Rect showFullscreenButton)
    {
		if (GUI.Button(showFullscreenButton, "Show Fullscreen", guiStyle))
        {
            var ad = loadInterstitial();
            ad.LoadAd(InterstitialAd.AdType.FullScreen);
		}
	}

	public void addShowOfferwallButton(Rect showOfferwallButton)
    {
		if (GUI.Button(showOfferwallButton, "Show Offerwall", guiStyle))
        {
            var ad = loadInterstitial();
            ad.LoadAd(InterstitialAd.AdType.OfferWall);
        }
	}

	public void addShowRewardedVideoButton(Rect showRewardedVideoButton)
    {
		if (GUI.Button(showRewardedVideoButton, "Show Rewarded Video", guiStyle))
        {
            var ad = loadInterstitial();
            ad.RaiseAdVideoCompleted += (sender, e) => Debug.Log("Ad video completed");
            ad.LoadAd(InterstitialAd.AdType.Rewarded);
        }
	}

	public void addShowBannersButton(Rect showBannersButton)
    {
		if (GUI.Button(showBannersButton, "Show Banners", guiStyle))
        {
			SceneManager.LoadScene("Banners", LoadSceneMode.Single);
		}
	}

	public void addShowPersonalizedAdsButton(Rect showPersonalizedAdsButton)
    {
		if (GUI.Button(showPersonalizedAdsButton, "Personalized Ads Setting", guiStyle))
        {
			startAppInitialized = false;
			showGdprDialog(() => {
				startAppInitialized = true;
			});
		}
	}

	public void initializeButtons()
    {
		/* Determine buttons size */
		int buttonHeight = Screen.height / 7;
		showFullscreenButton = new Rect(0, buttonHeight, Screen.width, buttonHeight);
		showOfferwallButton = new Rect(0, 2 * buttonHeight, Screen.width, buttonHeight);
		showRewardedVideoButton = new Rect(0, 3 * buttonHeight, Screen.width, buttonHeight);
		showBannersButton = new Rect(0, 4 * buttonHeight, Screen.width, buttonHeight);
		showPersonalizedAdsButton = new Rect(0, 5 * buttonHeight, Screen.width, buttonHeight);
		
		/* Change text size and logo size according to screen orientation */
		guiStyle = new GUIStyle(GUI.skin.button);
		if (Screen.orientation == ScreenOrientation.Portrait)
        {
			guiStyle.fontSize = Screen.width / 14;
		}
        else
        {
			guiStyle.fontSize = Screen.height / 14;
		}
	}
}
