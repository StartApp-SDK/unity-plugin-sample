using UnityEngine;
using System.Collections;
using System;

namespace StartApp {

	public class StartAppWrapper : MonoBehaviour {

	#if UNITY_ANDROID

		private static string developerId;
		private static string applicatonId;
	    
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

		/* Implementation of Splash Config for Unity */
		public class SplashConfig {

			public enum Theme {
				USER_DEFINED = 0,
				DEEP_BLUE,
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
				javaSplashConfig = new AndroidJavaObject("com.startapp.android.publish.splash.SplashConfig");
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
				Texture2D logoTexture = Resources.Load (fileName) as Texture2D;
				if (logoTexture == null) {
					throw new ArgumentException("File doesn't exist in Assets/Resources folder", fileName);
				}
				byte[] logoByteArray = logoTexture.EncodeToJPG ();
				string base64string = Convert.ToBase64String(logoByteArray);
				wrapper.Call<AndroidJavaObject>("setLogo", getJavaSplashConfig(), base64string);
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

			public ImplementationAdEventListener(AdEventListener listener) : base("com.startapp.android.publish.AdEventListener") {
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
				return GetHashCode ();
			}
		}

		/* Implementation of Ad Display Listener for Unity */
		private class ImplementationAdDisplayListener : AndroidJavaProxy {
			private AdDisplayListener listener = null;

			public ImplementationAdDisplayListener(AdDisplayListener listener) : base("com.startapp.android.publish.AdDisplayListener"){
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
		}

		/* Implementation of Ad Display Listener for Unity */
		private class OnBackPressedAdDisplayListener : AndroidJavaProxy {
			private string gameObjectName = null;
			private bool clicked = false;

			public OnBackPressedAdDisplayListener(string gameObjectName) : base("com.startapp.android.publish.AdDisplayListener"){
				this.gameObjectName = gameObjectName;
			}
			
			void adHidden(AndroidJavaObject ad) {
				Debug.Log("ad hidden - quitting application");
				if (!clicked) {
					init();
					wrapper.Call ("quit", gameObjectName);
				}
			}
			
			void adDisplayed(AndroidJavaObject ad) {

			}
			
			void adClicked(AndroidJavaObject ad) {
				clicked = true;
			}
		}
		
		public static void loadAd(AdEventListener listener) {
			init();
			wrapper.Call("loadAd", new []{new ImplementationAdEventListener(listener)});
		}
		
		public static bool showAd(AdDisplayListener listener) {
			init();
			return wrapper.Call<bool>("showAd", new object[] {new ImplementationAdDisplayListener(listener)});
		}
		
		public static bool onBackPressed(string gameObjectName) {
			init();
			return wrapper.Call<bool>("onBackPressed", new object[] {new OnBackPressedAdDisplayListener(gameObjectName)});
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

		public static void loadAd() {
			init ();
			wrapper.Call("loadAd");
		}
		
		public static bool showAd() {
			init();
			return wrapper.Call<bool>("showAd");
		}

		public static void showSplash() {
			init ();
			wrapper.Call ("showSplash");
		}
		
		public static void addBanner() {
			addBanner(BannerType.AUTOMATIC, BannerPosition.BOTTOM);
		}
		
		public static void addBanner(BannerType bannerType, BannerPosition position) {
			int pos = 1;
			int type = 1;
			// Select position
			switch(position){
				case BannerPosition.BOTTOM:
					pos = 1;
					break;
				case BannerPosition.TOP:
					pos = 2;
					break;
			}
			AndroidJavaObject objPosition = new AndroidJavaObject("java.lang.Integer", pos);
				
				
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
				
			init();
			wrapper.Call("addBanner", new []{ objType, objPosition });
		}
	
		public static void removeBanner() {
			removeBanner(BannerPosition.BOTTOM);
		}

		public static void removeBanner(BannerPosition position) {
			int pos = 1;
			// Select position
			switch(position){
			case BannerPosition.BOTTOM:
				pos = 1;
				break;
			case BannerPosition.TOP:
				pos = 2;
				break;
			}
			AndroidJavaObject objPosition = new AndroidJavaObject("java.lang.Integer", pos);

			init();
			wrapper.Call("removeBanner", objPosition);
		}
			
		public static void init() {	
			if (wrapper == null) {
				initWrapper (true);
			}
		}
		
		private static void initWrapper(bool enableReturnAds) {
			unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
			currentActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
			wrapper = new AndroidJavaObject("com.startapp.android.unity.InAppWrapper", currentActivity);
			
			if (!initUserData()){
				throw new System.ArgumentException("Error in initializing Application ID or Developer ID, Verify you initialized them in StartAppData in resources");
			}
			
			AndroidJavaObject jAppId = new AndroidJavaObject("java.lang.String", applicatonId);
			AndroidJavaObject jDevId = new AndroidJavaObject("java.lang.String", developerId);
			AndroidJavaObject jEnableReturnAds = new AndroidJavaObject("java.lang.Boolean", enableReturnAds);
			
			wrapper.Call("init", jDevId, jAppId, jEnableReturnAds);
		}

		private static bool initUserData() {
			bool result = false;
			int assigned = 0;
				
			TextAsset data = (TextAsset)Resources.Load("StartAppData");
			string userData = data.ToString();
				
			string[] splitData = userData.Split('\n');
			string[] singleData;
				
			for (int i = 0; i < splitData.Length; i++){
				singleData = splitData[i].Split('=');
				if (singleData[0].ToLower().CompareTo("applicationid") == 0){
					assigned++;
					applicatonId = singleData[1].ToString().Trim();
				}
					
				if (singleData[0].ToLower().CompareTo("developerid") == 0){
					assigned++;
					developerId = singleData[1].ToString().Trim();
				}
			}
				
			if (assigned == 2){
				result = true;
			}
			return result;
		}
		
	#elif UNITY_IPHONE
	    public interface AdEventListener {
	        void onReceiveAd();
	        void onFailedToReceiveAd();
	    }
	#endif
		
	}
}