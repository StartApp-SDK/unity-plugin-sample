using UnityEngine;
using System.Collections;
using StartApp;

public class StartAppAd : MonoBehaviour {

	bool bannerIsShown = false;

	void Start() {
		StartAppWrapper.loadAd();
	}

	void OnGUI () {
		/* Determine buttons size */
		int buttonHeight = Screen.height / 5;
		Rect logoRect = new Rect(Screen.width / 4, buttonHeight / 3, Screen.width / 2, buttonHeight / 3);
		Rect bannerButtonRect = new Rect(0, buttonHeight, Screen.width, buttonHeight);
		Rect interstitialAdButtonRect = new Rect(0, 2 * buttonHeight, Screen.width, buttonHeight);
		Rect interstitialAdWithListenerButtonRect = new Rect(0, 3 * buttonHeight, Screen.width, buttonHeight);
		
		/* Change text size and logo size according to screen orientation */
		GUIStyle guiStyle = new GUIStyle(GUI.skin.button);
		GUIStyle guiStyle2 = new GUIStyle (GUI.skin.button);

		if (Screen.orientation == ScreenOrientation.Portrait) {
			guiStyle.fontSize = Screen.width / 12;
			guiStyle2.fontSize = Screen.width / 23;
		} else {
			logoRect = new Rect(Screen.width / 3, buttonHeight / 3, Screen.width / 3, buttonHeight / 3);
			guiStyle.fontSize = Screen.height / 12;
			guiStyle2.fontSize = Screen.height / 23;
		}

		drawLogo(logoRect);

		/* STARTAPP ADS */
		addBannerButton(bannerButtonRect, guiStyle);
		addInterstitialButton(interstitialAdButtonRect, guiStyle);
		addInterstitialWithListenerButton(interstitialAdWithListenerButtonRect, guiStyle2);
	}

	public void addBannerButton(Rect bannerButtonRect, GUIStyle guiStyle) {
		if (GUI.Button (bannerButtonRect, "Show/Hide Banner", guiStyle)) {
			if (bannerIsShown) {
				StartAppWrapper.removeBanner ();
			} else {
				StartAppWrapper.addBanner (StartAppWrapper.BannerType.AUTOMATIC, StartAppWrapper.BannerPosition.BOTTOM);
			}
			bannerIsShown = !bannerIsShown;
		}
	}

	public void addInterstitialButton(Rect interstitialAdButtonRect, GUIStyle guiStyle) {
		if (GUI.Button(interstitialAdButtonRect, "Show Interstitial Ad", guiStyle)) {
			StartAppWrapper.showAd();
			StartAppWrapper.loadAd();
		}
	}

	public void addInterstitialWithListenerButton(Rect interstitialAdWithListenerButtonRect, GUIStyle guiStyle) {
		if (GUI.Button (interstitialAdWithListenerButtonRect, "Show Interstitial Ad Using Callbacks", guiStyle)) {
			StartAppWrapper.AdDisplayListener adDisplayListener = new AdDisplayListenerImplementation ();
			StartAppWrapper.AdEventListener adEventListener = new AdEventListenerImplementation ();
			StartAppWrapper.showAd (adDisplayListener);
			StartAppWrapper.loadAd (adEventListener);
		}
	}

	public void drawLogo(Rect logoRect) {
		Texture2D logo = Resources.Load ("StartAppLogo") as Texture2D;
		GUI.DrawTexture (logoRect, logo);
	}

	#if (!(UNITY_2_6 || UNITY_2_6_1 || UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1))
	/* AdDisplayListener callbacks */
	public class AdDisplayListenerImplementation : StartAppWrapper.AdDisplayListener {
		public AdDisplayListenerImplementation() {}
		
		public void adHidden() {
			Debug.Log ("Ad hidden");
		}
		
		public void adDisplayed() {
			Debug.Log ("Ad displayed");
		}
		
		public void adClicked() {
			Debug.Log ("Ad clicked");
		}
	}

	/* AdEventListener callbacks */
	public class AdEventListenerImplementation : StartAppWrapper.AdEventListener {
		public AdEventListenerImplementation() {}
		
		public void onReceiveAd() {
			Debug.Log ("Ad received");
		}
		
		public void onFailedToReceiveAd() {
			Debug.Log ("Ad failed to receive");
		}
	}
	#endif
}