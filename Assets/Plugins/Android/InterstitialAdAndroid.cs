#if UNITY_ANDROID

using UnityEngine;

namespace StartApp
{
    public class InterstitialAdAndroid : InterstitialAd
    {
        readonly AndroidJavaObject mjStartAppAd;

        static InterstitialAdAndroid()
        {
            AdSdkAndroid.ImplInstance.Setup();
        }

        public InterstitialAdAndroid()
        {
            mjStartAppAd = new AndroidJavaObject("com.startapp.android.publish.adsCommon.StartAppAd", AdSdkAndroid.ImplInstance.Activity);
            mjStartAppAd.Call("setVideoListener", new ImplementationVideoListener(this));
        }

        public override void LoadAd(AdType mode)
        {
            mjStartAppAd.Call("loadAd", GetJAdType(mode), new ImplementationAdEventListener(this));
        }

        public override bool ShowAd(string tag)
        {
            if (tag == null)
            {
                return mjStartAppAd.Call<bool>("showAd", new ImplementationAdDisplayListener(this));
            }

            var objTag = new AndroidJavaObject("java.lang.String", tag);
            return mjStartAppAd.Call<bool>("showAd", objTag, new ImplementationAdDisplayListener(this));
        }

        public override bool IsReady()
        {
            return mjStartAppAd.Call<bool>("isReady");
        }

        class ImplementationAdEventListener : AndroidJavaProxy
        {
            readonly InterstitialAdAndroid mParent;

            public ImplementationAdEventListener(InterstitialAdAndroid parent) : base("com.startapp.android.publish.adsCommon.adListeners.AdEventListener")
            {
                mParent = parent;
            }

            void onReceiveAd(AndroidJavaObject ad)
            {
                mParent.OnRaiseAdLoaded();
            }

            void onFailedToReceiveAd(AndroidJavaObject ad)
            {
                var errorMessage = ad.Call<AndroidJavaObject>("getErrorMessage");
                mParent.OnRaiseAdLoadingFailed(errorMessage.Call<string>("toString"));
            }
        }

        class ImplementationAdDisplayListener : AndroidJavaProxy
        {
            readonly InterstitialAdAndroid mParent;

            public ImplementationAdDisplayListener(InterstitialAdAndroid parent) : base("com.startapp.android.publish.adsCommon.adListeners.AdDisplayListener")
            {
                mParent = parent;
            }

            void adHidden(AndroidJavaObject ad)
            {
                mParent.OnRaiseAdClosed();
            }

            void adDisplayed(AndroidJavaObject ad)
            {
                mParent.OnRaiseAdShown();
            }

            void adClicked(AndroidJavaObject ad)
            {
                mParent.OnRaiseAdClicked();
            }
        }

        class ImplementationVideoListener : AndroidJavaProxy
        {
            readonly InterstitialAdAndroid mParent;

            public ImplementationVideoListener(InterstitialAdAndroid parent) : base("com.startapp.android.publish.adsCommon.VideoListener")
            {
                mParent = parent;
            }

            void onVideoCompleted()
            {
                mParent.OnRaiseAdVideoCompleted();
            }
        }

        static AndroidJavaObject GetJAdType(AdType adMode)
        {
            var jModeClass = new AndroidJavaClass("com.startapp.android.publish.adsCommon.StartAppAd$AdMode");
            switch (adMode)
            {
                case AdType.Automatic: return jModeClass.GetStatic<AndroidJavaObject>("AUTOMATIC");
                case AdType.FullScreen: return jModeClass.GetStatic<AndroidJavaObject>("FULLPAGE");
                case AdType.OfferWall: return jModeClass.GetStatic<AndroidJavaObject>("OFFERWALL");
                case AdType.Rewarded: return jModeClass.GetStatic<AndroidJavaObject>("REWARDED_VIDEO");
                case AdType.Video: return jModeClass.GetStatic<AndroidJavaObject>("VIDEO");
            }

            return jModeClass.GetStatic<AndroidJavaObject>("AUTOMATIC");
        }
    }
}

#endif