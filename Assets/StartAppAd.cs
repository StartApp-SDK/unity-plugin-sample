using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using StartApp;

public class StartAppAd : MonoBehaviour {

	StartAppWrapper.AdEventListener adEventListener;
	StartAppWrapper.VideoListener videoListener;
	GUIStyle guiStyle;
	Rect showFullscreenButton;
	Rect showOfferwallButton;
	Rect showRewardedVideoButton;
	Rect showBannersButton;
	Rect showPersonalizedAdsButton;
	bool startAppInitialized = false;

	void Start() {
		initStartAppSdkAccordingToConsent(() => {
			startAppInitialized = true;
		});

		Debug.Log("StartAppSDK start initializing");
	}

	void OnGUI () {
		if (!startAppInitialized) {
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

	void showGdprDialog(Action callback) {
		ModalDialog.Instance().Choice(
			() => {
				if (callback != null) {
					callback();
				}
				writePersonalizedAdsConsent(true);
			},
			() => {
				if (callback != null) {
					callback();
				}
				writePersonalizedAdsConsent(false);
			});
	}

	void writePersonalizedAdsConsent(bool isGranted) {
		Debug.Log("StartAppSDK setUserConsent: " + isGranted);

		StartAppWrapper.setUserConsent("pas",
                            (long)(DateTime.UtcNow.Subtract(new DateTime (1970, 1, 1))).TotalMilliseconds,                        
                            isGranted);

		PlayerPrefs.SetInt("gdpr_dialog_was_shown", 1);
		PlayerPrefs.Save();
	}

	void initStartAppSdk() {
		StartAppWrapper.init();
		adEventListener = new AdEventListenerImplementation();
		videoListener = new VideoListenerImplementation();
		StartAppWrapper.setVideoListener(videoListener);
	}

	void initStartAppSdkAccordingToConsent(Action callback) {
		if (PlayerPrefs.HasKey("gdpr_dialog_was_shown")) {
			initStartAppSdk();
			StartAppWrapper.showSplash();
			callback();
			return;
		}

		showGdprDialog(() => {
			initStartAppSdk();
			callback();
		});
	}

	public void addShowFullscreenButton(Rect showFullscreenButton) {
		if (GUI.Button(showFullscreenButton, "Show Fullscreen", guiStyle)) {
			StartAppWrapper.loadAd(StartAppWrapper.AdMode.FULLPAGE, adEventListener);
		}
	}

	public void addShowOfferwallButton(Rect showOfferwallButton) {
		if (GUI.Button(showOfferwallButton, "Show Offerwall", guiStyle)) {
			StartAppWrapper.loadAd(StartAppWrapper.AdMode.OFFERWALL, adEventListener);
		}
	}

	public void addShowRewardedVideoButton(Rect showRewardedVideoButton) {
		if (GUI.Button(showRewardedVideoButton, "Show Rewarded Video", guiStyle)) {
			StartAppWrapper.loadAd(StartAppWrapper.AdMode.REWARDED_VIDEO, adEventListener);
		}
	}

	public void addShowBannersButton(Rect showBannersButton) {
		if (GUI.Button(showBannersButton, "Show Banners", guiStyle)) {
			SceneManager.LoadScene("Banners", LoadSceneMode.Single);
		}
	}

	public void addShowPersonalizedAdsButton(Rect showPersonalizedAdsButton) {
		if (GUI.Button(showPersonalizedAdsButton, "Personalized Ads Setting", guiStyle)) {
			startAppInitialized = false;
			showGdprDialog(() => {
				startAppInitialized = true;
			});
		}
	}

	public void initializeButtons() {
		/* Determine buttons size */
		int buttonHeight = Screen.height / 7;
		showFullscreenButton = new Rect(0, buttonHeight, Screen.width, buttonHeight);
		showOfferwallButton = new Rect(0, 2 * buttonHeight, Screen.width, buttonHeight);
		showRewardedVideoButton = new Rect(0, 3 * buttonHeight, Screen.width, buttonHeight);
		showBannersButton = new Rect(0, 4 * buttonHeight, Screen.width, buttonHeight);
		showPersonalizedAdsButton = new Rect(0, 5 * buttonHeight, Screen.width, buttonHeight);
		
		/* Change text size and logo size according to screen orientation */
		guiStyle = new GUIStyle(GUI.skin.button);
		if (Screen.orientation == ScreenOrientation.Portrait) {
			guiStyle.fontSize = Screen.width / 14;
		} else {
			guiStyle.fontSize = Screen.height / 14;
		}
	}

	/* AdEventListener callbacks */
	public class AdEventListenerImplementation : StartAppWrapper.AdEventListener {
		StartAppWrapper.AdDisplayListener adDisplayListener = new AdDisplayListenerImplementation ();

		public void onReceiveAd() {
			Debug.Log("Ad received");
			StartAppWrapper.showAd(adDisplayListener);
		}
		
		public void onFailedToReceiveAd() {
			Debug.Log("Ad failed to receive");
		}
	}

	public class AdDisplayListenerImplementation: StartAppWrapper.AdDisplayListener {
		public void adHidden() {
			Debug.Log("Ad Hidden");
		}

		public void adDisplayed() {
			Debug.Log("Ad Displayed");
		}

		public void adClicked() {
			Debug.Log("Ad Clicked");
		}
	}

	/* VideoListener callback */
	public class VideoListenerImplementation: StartAppWrapper.VideoListener {
		public void onVideoCompleted() {
			Debug.Log("Rewarded Video Completed");
		}
	}
}
