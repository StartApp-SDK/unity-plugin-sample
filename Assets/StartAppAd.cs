using UnityEngine;
using System.Collections;
using StartApp;

public class StartAppAd : MonoBehaviour {

	StartAppWrapper.AdEventListener adEventListener;
	StartAppWrapper.VideoListener videoListener;
	GUIStyle guiStyle;
	Rect showFullscreenButton;
	Rect showOfferwallButton;
	Rect showRewardedVideoButton;
	Rect showBannersButton;

	void Start() {
		StartAppWrapper.showSplash ();
		adEventListener = new AdEventListenerImplementation ();
		videoListener = new VideoListenerImplementation ();
		StartAppWrapper.setVideoListener (videoListener);
	}

	void OnGUI () {
		initializeButtons ();

		/* STARTAPP ADS */
		addShowFullscreenButton(showFullscreenButton);
		addShowOfferwallButton(showOfferwallButton);
		addShowRewardedVideoButton(showRewardedVideoButton);
		addShowBannersButton(showBannersButton);
	}

	public void addShowFullscreenButton(Rect showFullscreenButton) {
		if (GUI.Button (showFullscreenButton, "Show Fullscreen", guiStyle)) {
			StartAppWrapper.loadAd(StartAppWrapper.AdMode.FULLPAGE, adEventListener);
		}
	}

	public void addShowOfferwallButton(Rect showOfferwallButton) {
		if (GUI.Button(showOfferwallButton, "Show Offerwall", guiStyle)) {
			StartAppWrapper.loadAd(StartAppWrapper.AdMode.OFFERWALL, adEventListener);
		}
	}

	public void addShowRewardedVideoButton(Rect showRewardedVideoButton) {
		if (GUI.Button (showRewardedVideoButton, "Show Rewarded Video", guiStyle)) {
			StartAppWrapper.loadAd(StartAppWrapper.AdMode.REWARDED_VIDEO, adEventListener);
		}
	}

	public void addShowBannersButton(Rect showBannersButton) {
		if (GUI.Button (showBannersButton, "Show Banners", guiStyle)) {
			Application.LoadLevel(1);
		}
	}

	public void initializeButtons() {
		/* Determine buttons size */
		int buttonHeight = Screen.height / 6;
		Rect logoRect = new Rect(Screen.width / 4, buttonHeight / 3, Screen.width / 2, buttonHeight / 3);
		showFullscreenButton = new Rect(0, buttonHeight, Screen.width, buttonHeight);
		showOfferwallButton = new Rect(0, 2 * buttonHeight, Screen.width, buttonHeight);
		showRewardedVideoButton = new Rect(0, 3 * buttonHeight, Screen.width, buttonHeight);
		showBannersButton = new Rect(0, 4 * buttonHeight, Screen.width, buttonHeight);
		
		/* Change text size and logo size according to screen orientation */
		guiStyle = new GUIStyle(GUI.skin.button);
		if (Screen.orientation == ScreenOrientation.Portrait) {
			guiStyle.fontSize = Screen.width / 12;
		} else {
			logoRect = new Rect(Screen.width / 3, buttonHeight / 3, Screen.width / 3, buttonHeight / 3);
			guiStyle.fontSize = Screen.height / 12;
		}

		drawLogo(logoRect);
	}

	public void drawLogo(Rect logoRect) {
		Texture2D logo = Resources.Load ("StartAppLogo") as Texture2D;
		GUI.DrawTexture (logoRect, logo);
	}

	/* AdEventListener callbacks */
	public class AdEventListenerImplementation : StartAppWrapper.AdEventListener {
		StartAppWrapper.AdDisplayListener adDisplayListener = new AdDisplayListenerImplementation ();

		public void onReceiveAd() {
			Debug.Log ("Ad received");
			StartAppWrapper.showAd(adDisplayListener);
		}
		
		public void onFailedToReceiveAd() {
			Debug.Log ("Ad failed to receive");
		}
	}

	public class AdDisplayListenerImplementation : StartAppWrapper.AdDisplayListener {
		public void adHidden() {
			Debug.Log ("Ad Hidden");
		}

		public void adDisplayed() {
			Debug.Log ("Ad Displayed");
		}

		public void adClicked() {
			Debug.Log ("Ad Clicked");
		}
	}

	/* VideoListener callback */
	public class VideoListenerImplementation : StartAppWrapper.VideoListener {
		public void onVideoCompleted() {
			Debug.Log ("Rewarded Video Completed");
		}
	}
}