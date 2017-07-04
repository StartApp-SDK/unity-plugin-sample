using UnityEngine;
using System.Collections;
using System;

namespace StartApp {
	
	public class StartAppWrapper : MonoBehaviour {
		#if UNITY_ANDROID
		private static string accountId;
		private static string applicationId;
		private static bool enableReturnAds = true;
		private static bool isAccountIdUsed = false;

		private static AndroidJavaObject jAppId = null;
		private static AndroidJavaObject jAccId = null;
		private static AndroidJavaObject jEnableReturnAds = null;
		private static AndroidJavaClass unityClass;
		private static AndroidJavaObject currentActivity;
		private static AndroidJavaObject wrapper;
		
		#if (!(UNITY_2_6 || UNITY_2_6_1 || UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1))
		// Unity 4.2 or newer
		public interface AdEventListener {
			void onReceiveAd();
			void onFailedToReceiveAd();
		}
		
		public interface AdDisplayListener {
			void adHidden();
			void adDisplayed();
			void adClicked();
		}
		
		public interface VideoListener {
			void onVideoCompleted();
		}
		
		public enum AdMode {
			AUTOMATIC = 1,
			FULLPAGE,
			OFFERWALL,
			REWARDED_VIDEO,
			[Obsolete]
			OVERLAY
		}
		
		/* Implementation of Splash Config for Unity */
		public class SplashConfig {
			
			public enum Theme {
				DEEP_BLUE = 1,
				SKY,
				ASHEN_SKY,
				BLAZE,
				GLOOMY,
				OCEAN
			};
			
			public enum Orientation {
				PORTRAIT = 1,
				LANDSCAPE,
				AUTO
			};
			
			private AndroidJavaObject javaSplashConfig = null;
			
			public SplashConfig() {
				init();
				javaSplashConfig = new AndroidJavaObject("com.startapp.android.publish.ads.splash.SplashConfig");
			}
			
			public AndroidJavaObject getJavaSplashConfig() {
				return javaSplashConfig;
			}
			
			public SplashConfig setAppName(string appName) {
				AndroidJavaObject appNameJava = new AndroidJavaObject("java.lang.String", appName);
				wrapper.Call<AndroidJavaObject>("setAppName", getJavaSplashConfig(), appNameJava);
				return this;
			}
			
			public SplashConfig setTheme(Theme theme) {
				int themeIndex = (int)theme;
				AndroidJavaObject themeIndexInteger = new AndroidJavaObject("java.lang.Integer", themeIndex);
				wrapper.Call<AndroidJavaObject>("setTheme", getJavaSplashConfig(), themeIndexInteger);
				return this;
			}
			
			public SplashConfig setLogo(string fileName) {
				byte[] logoByteArray = null;
				Texture2D logoTexture = Resources.Load (fileName) as Texture2D;
				if (logoTexture != null) {
					logoByteArray = logoTexture.EncodeToPNG ();
				}
				wrapper.Call<AndroidJavaObject>("setLogo", getJavaSplashConfig(), logoByteArray);
				return this;
			}
			
			public SplashConfig setOrientation(Orientation orientation) {
				int orientationIndex = (int)orientation;
				AndroidJavaObject orientationIndexInteger = new AndroidJavaObject("java.lang.Integer", orientationIndex);
				wrapper.Call<AndroidJavaObject>("setOrientation", getJavaSplashConfig(), orientationIndexInteger);
				return this;
			}
		}
		
		/* Implementation of Ad Event Listener for Unity */
		private class ImplementationAdEventListener : AndroidJavaProxy {
			private AdEventListener listener = null;
			
			public ImplementationAdEventListener(AdEventListener listener) : base("com.startapp.android.publish.adsCommon.adListeners.AdEventListener") {
				this.listener = listener;
			}
			
			void onReceiveAd(AndroidJavaObject ad) {
				if (listener != null){
					listener.onReceiveAd();
				}
			}
			
			void onFailedToReceiveAd(AndroidJavaObject ad) {
				if (listener != null){
					listener.onFailedToReceiveAd();
				}
			}
			
			int hashCode() {
				return listener.GetHashCode();
			}
			
			bool equals(AndroidJavaObject o) {
				int otherHash = o.Call<int>("hashCode");
				return otherHash == listener.GetHashCode();
			}
			
			// Without this we get null when printing 
			String toString() {
                return "ImplementationAdEventListener: " + hashCode();
            }
		}
		
		/* Implementation of Ad Display Listener for Unity */
		private class ImplementationAdDisplayListener : AndroidJavaProxy {
			private AdDisplayListener listener = null;
			
			public ImplementationAdDisplayListener(AdDisplayListener listener) : base("com.startapp.android.publish.adsCommon.adListeners.AdDisplayListener"){
				this.listener = listener;
			}
			
			void adHidden(AndroidJavaObject ad) {
				if (listener != null){
					listener.adHidden();
				}
			}
			
			void adDisplayed(AndroidJavaObject ad) {
				if (listener != null){
					listener.adDisplayed();
				}
			}
			
			void adClicked(AndroidJavaObject ad) {
				if (listener != null){
					listener.adClicked();
				}
			}
			
			int hashCode() {
				return listener.GetHashCode();
			}
			
			bool equals(AndroidJavaObject o) {
				int otherHash = o.Call<int>("hashCode");
				return otherHash == listener.GetHashCode();
			}
			
			// Without this we get null when printing 
			String toString() {
                return "ImplementationAdDisplayListener: " + hashCode();
            }
		}
		
		/* Implementation of Ad Display Listener for Unity */
		private class OnBackPressedAdDisplayListener : AndroidJavaProxy {
			private string gameObjectName = null;
			private bool clicked = false;
			
			public OnBackPressedAdDisplayListener(string gameObjectName) : base("com.startapp.android.publish.adsCommon.adListeners.AdDisplayListener") {
				this.gameObjectName = gameObjectName;
			}
			
			void adHidden(AndroidJavaObject ad) {
				if (!clicked) {
					init();
					Application.Quit();
				}
			}
			
			void adDisplayed(AndroidJavaObject ad) {
				
			}
			
			void adClicked(AndroidJavaObject ad) {
				clicked = true;
			}
			
			void adNotDisplayed(AndroidJavaObject ad) {
				
			}
		}
		
		/* Implementation of Video Listener for Unity */
		private class ImplementationVideoListener : AndroidJavaProxy {
			private VideoListener listener = null;
			
			public ImplementationVideoListener(VideoListener listener) : base("com.startapp.android.publish.adsCommon.VideoListener"){
				this.listener = listener;
			}
			
			void onVideoCompleted() {
				if (listener != null){
					listener.onVideoCompleted();
				}
			}
		}
		
		public static void setVideoListener(VideoListener listener) {
			init ();
			wrapper.Call ("setVideoListener", new ImplementationVideoListener(listener));
		}
		
		public static void loadAd(AdEventListener listener) {
			loadAd (AdMode.AUTOMATIC, listener);
		}
		
		public static void loadAd(AdMode adMode) {
			loadAd (adMode, null);
		}
		
		public static void loadAd(AdMode adMode, AdEventListener listener) {
			loadAd (adMode, listener, false);
		}
		
		private static void loadAd(AdMode adMode, AdEventListener listener, bool is3D) {
			init();
			int adModeIndex = (int)adMode;
			AndroidJavaObject adModeIndexInteger = new AndroidJavaObject("java.lang.Integer", adModeIndex);
			AndroidJavaObject is3DBoolean = new AndroidJavaObject("java.lang.Boolean", is3D);
			if (listener == null) {
				wrapper.Call ("loadAd", adModeIndexInteger);
				return;
			}
			wrapper.Call ("loadAd", adModeIndexInteger, new ImplementationAdEventListener (listener));
		}
		
		public static bool showAd(AdDisplayListener listener) {
			init();
			return wrapper.Call<bool>("showAd", new object[] {new ImplementationAdDisplayListener(listener)});
		}

        public static bool showAd(String adTag) {
            init();
            AndroidJavaObject objTag = new AndroidJavaObject("java.lang.String", adTag);
            return wrapper.Call<bool>("showAd", objTag);
        }

        public static bool showAd(String adTag, AdDisplayListener listener) {
            if (adTag == null) {
                return showAd(listener);
            }
            
            init();
            AndroidJavaObject objTag = new AndroidJavaObject("java.lang.String", adTag);
			return wrapper.Call<bool>("showAd", new object[] {adTag, new ImplementationAdDisplayListener(listener)});
        }
		
		public static bool onBackPressed(string gameObjectName) {
			init();
			return wrapper.Call<bool>("onBackPressed", new object[] {new OnBackPressedAdDisplayListener(gameObjectName)});
		}
		
		public static void showSplash() {
			init ();
			wrapper.Call ("showSplash");
		}
		
		public static void showSplash(SplashConfig splashConfig) {
			init ();
			wrapper.Call ("showSplash", splashConfig.getJavaSplashConfig());
		}
		
		#else
		// Unity 4.1 or older - no listener
		public static bool onBackPressed(string gameObjectName) {
			init();
			return wrapper.Call<bool>("onBackPressed");
		}
		#endif

		public enum BannerPosition {
			BOTTOM,
			TOP
		};
		
		public enum BannerType {
			AUTOMATIC,
			STANDARD,
			THREED
		};

		public static bool checkIfBannerExists(BannerPosition bannerPosition) {
			AndroidJavaObject objPosition = getBannerPositionObject (bannerPosition);
			return wrapper.Call<bool> ("checkIfBannerExists", objPosition);
		}

		private static AndroidJavaObject getBannerPositionObject(BannerPosition bannerPosition) {
			int pos = 1;
			switch(bannerPosition) {
			case BannerPosition.BOTTOM:
				pos = 1;
				break;
			case BannerPosition.TOP:
				pos = 2;
				break;
			}
			AndroidJavaObject objPosition = new AndroidJavaObject("java.lang.Integer", pos);
			return objPosition;
		}

		private static AndroidJavaObject getBannerTypeObject(BannerType bannerType) {
			int type = 1;
			// Select type
			switch(bannerType) {
			case BannerType.AUTOMATIC:
				type = 1;
				break;
			case BannerType.STANDARD:
				type = 2;
				break;
			case BannerType.THREED:
				type = 3;
				break;
			}
			AndroidJavaObject objType = new AndroidJavaObject("java.lang.Integer", type);
			return objType;
		}
		
		public static void addBanner() {
			addBanner(BannerType.AUTOMATIC, BannerPosition.BOTTOM);
		}
		
		public static void addBanner(BannerType bannerType, BannerPosition bannerPosition) {
			addBanner(bannerType, bannerPosition, null);
		}

        public static void addBanner(BannerType bannerType, BannerPosition bannerPosition, String adTag) {
			init();
			AndroidJavaObject objPosition = getBannerPositionObject (bannerPosition);
			AndroidJavaObject objType = getBannerTypeObject (bannerType);

            if (adTag == null) {
                wrapper.Call("addBanner", new []{ objType, objPosition });
            } else {
                AndroidJavaObject objTag = new AndroidJavaObject("java.lang.String", adTag);
			    wrapper.Call("addBanner", new []{ objType, objPosition, objTag });
            }
		}
		
		public static void removeBanner() {
			removeBanner(BannerPosition.BOTTOM);
		}
		
		public static void removeBanner(BannerPosition bannerPosition) {
			init();
			AndroidJavaObject objPosition = getBannerPositionObject(bannerPosition);
			wrapper.Call("removeBanner", objPosition);
		}
		
		public static void disableReturnAds() {
			enableReturnAds = false;
		}

		public static void loadAd() {
			init ();
			wrapper.Call("loadAd");
		}
		
		public static bool showAd() {
			init();
			return wrapper.Call<bool>("showAd");
		}
		
		
		/* Initialization */
		public static void init() {	
			if (wrapper == null) {
				initWrapper();
				initSdk();
			}
		}

		public static void init(string appId, bool enableReturnAds) {	
			if (wrapper == null) {
				initWrapper();
				initSdk(appId, enableReturnAds);
			}
		}

		public static void init(string accId, string appId, bool enableReturnAds) {	
			if (wrapper == null) {
				initWrapper();
				initSdk(accId, appId, enableReturnAds);
			}
		}
		
		private static void initWrapper() {			
			unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
			currentActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
			wrapper = new AndroidJavaObject("com.startapp.android.unity.InAppWrapper", currentActivity);
		}

		private static void initSdk() {
			if (!readDataFromTextFile()) {
				throw new System.ArgumentException("Error in initializing Application ID, Account ID or Return Ads. Please verify your StartAppData.txt file.");
			}
			if (isAccountIdUsed) { 
				initSdk (accountId, applicationId, enableReturnAds);
			} else {
				initSdk (applicationId, enableReturnAds);
			}
		}

		private static void initSdk(string appId, bool returnAds) {
			initSdk (null, appId, returnAds);
		}

		private static void initSdk(string accId, string appId, bool returnAds) {
			jAppId = new AndroidJavaObject("java.lang.String", appId);
			jEnableReturnAds = new AndroidJavaObject("java.lang.Boolean", returnAds);
			if (accId == null) {
				wrapper.Call("init", jAppId, jEnableReturnAds);
			} else {
				jAccId = new AndroidJavaObject ("java.lang.String", accId);
				wrapper.Call ("init", jAccId, jAppId, jEnableReturnAds);
			}
		}
		
		private static bool readDataFromTextFile() {
			bool result = false;
			int assigned = 0;
			
			TextAsset data = (TextAsset)Resources.Load("StartAppData");
			string userData = data.ToString();
			
			string[] splitData = userData.Split('\n');
			string[] singleData;
			
			for (int i = 0; i < splitData.Length; i++) {
				singleData = splitData[i].Split('=');
				if (singleData[0].ToLower().CompareTo("applicationid") == 0) {
					assigned++;
					applicationId = singleData[1].ToString().Trim();
				}
				
				// Account ID is optional
				if (singleData[0].ToLower().CompareTo("accountid") == 0 || singleData[0].ToLower().CompareTo("developerid") == 0) {
					isAccountIdUsed = true;
					accountId = singleData[1].ToString().Trim();
				}
				
				if (singleData[0].ToLower().CompareTo("returnads") == 0) {
					if (singleData[1].ToLower().Equals("false")) {
						assigned++;
						disableReturnAds();
					}
				}
			}
			
			removeSpecialCharacters ();
			if ((enableReturnAds && assigned == 1) || (!enableReturnAds && assigned == 2)) {
				Debug.Log ("Initialization successful");
				Debug.Log ("Application ID: " + applicationId);
				if (isAccountIdUsed) {
					Debug.Log ("Account ID: " + accountId);
				}
				if (enableReturnAds) {
					Debug.Log ("Return ads are enabled");
				}
				result = true;
			}
			return result;
		}
		
		private static void removeSpecialCharacters() {
			if (applicationId != null && applicationId.IndexOf ("\"") != -1) {
				applicationId = applicationId.Replace("\"", "");
			}
			
			if (isAccountIdUsed && accountId != null && accountId.IndexOf ("\"") != -1) {
				accountId = accountId.Replace("\"", "");
			}
		}
		#endif
	}
}