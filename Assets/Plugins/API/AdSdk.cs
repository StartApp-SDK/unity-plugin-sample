using System;

namespace StartApp
{
    public abstract class AdSdk
    {
        public static string WrapperVersion = "1.0.0";

        private static AdSdk sInstance;
        public static AdSdk Instance
        {
            get
            {
                if (sInstance == null)
                {
                    #if UNITY_IOS
                    sInstance = new AdSdkiOS();
                    #elif UNITY_ANDROID
                    sInstance = new AdSdkAndroid();
                    #endif
                }
                return sInstance;
            }
        }

        public abstract InterstitialAd CreateInterstitial();
        public abstract BannerAd CreateBanner();
        public abstract void SetUserConsent(string consentType, bool enabled, long timestamp);
        public abstract void ShowSplash(SplashConfig config = null);
        public abstract void DisableReturnAds();
        public abstract bool OnBackPressed();

        public InterstitialAd ShowDefaultAd(InterstitialAd.AdType mode = InterstitialAd.AdType.Automatic, string tag = null)
        {
            var ad = CreateInterstitial();
            ad.RaiseAdLoaded += (sender, e) => ad.ShowAd(tag);
            ad.LoadAd(mode);
            return ad;
        }

        public BannerAd ShowDefaultBanner(BannerAd.BannerPosition position = BannerAd.BannerPosition.Bottom, string tag = null)
        {
            var banner = CreateBanner();
            banner.ShowInPosition(position, tag);
            return banner;
        }
    }

    public class MessageArgs : EventArgs
    {
        readonly string mMessage;

        public MessageArgs(string msg)
        {
            mMessage = msg;
        }

        public string Message
        {
            get { return mMessage; }
        }
    }
}
