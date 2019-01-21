#if UNITY_ANDROID

using UnityEngine;
using System;

namespace StartApp
{
    public class BannerAdAndroid : BannerAd, IDisposable
    {
        static readonly int ID_LAYOUT = 11;
        static readonly int ID_UNITY = 12;
        static readonly int MATCH_PARENT;
        static readonly int WRAP_CONTENT;
        static readonly int CENTER_HORIZONTAL;
        static readonly int ALIGN_PARENT_BOTTOM;
        static readonly int ALIGN_PARENT_TOP;
        static readonly int VISIBLE;

        AndroidJavaObject mBanner;
        BannerPosition mCurrentPosition;
        bool mDisposed;

        static BannerAdAndroid()
        {
            AdSdkAndroid.ImplInstance.Setup();

            var viewParams = new AndroidJavaClass("android.view.ViewGroup$LayoutParams");
            MATCH_PARENT = viewParams.GetStatic<int>("MATCH_PARENT");
            WRAP_CONTENT = viewParams.GetStatic<int>("WRAP_CONTENT");

            var relativeLayout = new AndroidJavaClass("android.widget.RelativeLayout");
            CENTER_HORIZONTAL = relativeLayout.GetStatic<int>("CENTER_HORIZONTAL");
            ALIGN_PARENT_BOTTOM = relativeLayout.GetStatic<int>("ALIGN_PARENT_BOTTOM");
            ALIGN_PARENT_TOP = relativeLayout.GetStatic<int>("ALIGN_PARENT_TOP");

            var viewClass = new AndroidJavaClass("android.view.View");
            VISIBLE = viewClass.GetStatic<int>("VISIBLE");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (mDisposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
            }
            AdSdkAndroid.ImplInstance.Activity.Call("runOnUiThread", new AndroidJavaRunnable(RemoveBannerImplementation));
            mDisposed = true;
        }

        ~BannerAdAndroid()
        {
            Dispose(false);
        }

        public override void PreLoad()
        {
            AdSdkAndroid.ImplInstance.Activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                mBanner.Call("loadAd");
            }));
        }

        public override void ShowInPosition(BannerPosition position, string tag)
        {
            var jContent = new AndroidJavaObject("java.lang.String", "content");
            var jId = new AndroidJavaObject("java.lang.String", "id");
            var jPackage = new AndroidJavaObject("java.lang.String", "android");

            AdSdkAndroid.ImplInstance.Activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                var layout = AdSdkAndroid.ImplInstance.Activity.Call<AndroidJavaObject>("findViewById", ID_LAYOUT);

                if (layout == null)
                {
                    var res = AdSdkAndroid.ImplInstance.Activity.Call<AndroidJavaObject>("getResources");
                    var id = res.Call<int>("getIdentifier", jContent, jId, jPackage);
                    var contentParent = AdSdkAndroid.ImplInstance.Activity.Call<AndroidJavaObject>("findViewById", id);
                    var unityView = contentParent.Call<AndroidJavaObject>("getChildAt", 0);
                    unityView.Call("setId", ID_UNITY);
                    contentParent.Call("removeView", unityView);

                    layout = new AndroidJavaObject("android.widget.RelativeLayout", AdSdkAndroid.ImplInstance.Activity);
                    layout.Call("setId", ID_LAYOUT);
                    contentParent.Call("addView", layout);

                    var layoutParams = new AndroidJavaObject("android.widget.RelativeLayout$LayoutParams", MATCH_PARENT, MATCH_PARENT);
                    layout.Call("addView", unityView, layoutParams);
                }

                var bannerParams = new AndroidJavaObject("android.widget.RelativeLayout$LayoutParams", WRAP_CONTENT, WRAP_CONTENT);
                bannerParams.Call("addRule", CENTER_HORIZONTAL);
                bannerParams.Call("addRule", GetRuleFromBannerPosition(position));

                if (mBanner == null)
                {
                    mBanner = new AndroidJavaObject("com.startapp.android.publish.ads.banner.bannerstandard.BannerStandard", AdSdkAndroid.ImplInstance.Activity);
                    if (tag != null)
                    {
                        mBanner.Call("setAdTag", tag);
                    }
                    mBanner.Call("setBannerListener", new ImplementationBannerListener(this));

                    layout.Call("addView", mBanner, bannerParams);
                }
                else
                {
                    mBanner.Call("setLayoutParams", bannerParams);
                    mBanner.Call("showBanner");
                }

                mCurrentPosition = position;
            }));
        }

        public override void Hide()
        {
            AdSdkAndroid.ImplInstance.Activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                mBanner.Call("hideBanner");
            }));
        }

        public override bool IsShownInPosition(BannerPosition position)
        {
            return IsVisible() && mCurrentPosition == position;
        }

        bool IsVisible()
        {
            return mBanner != null && mBanner.Call<int>("getVisibility") == VISIBLE;
        }

        public void RemoveBannerImplementation()
        {
            if (mBanner == null)
            {
                return;
            }

            var layout = AdSdkAndroid.ImplInstance.Activity.Call<AndroidJavaObject>("findViewById", ID_LAYOUT);

            if (layout != null)
            {
                layout.Call("removeView", mBanner);
                mBanner = null;
            }
        }

        static int GetRuleFromBannerPosition(BannerPosition position)
        {
            return position == BannerPosition.Top ? ALIGN_PARENT_TOP : ALIGN_PARENT_BOTTOM;
        }

        class ImplementationBannerListener : AndroidJavaProxy
        {
            readonly BannerAdAndroid mParent;

            public ImplementationBannerListener(BannerAdAndroid parent) : base("com.startapp.android.publish.ads.banner.BannerListener")
            {
                mParent = parent;
            }

            void onReceiveAd(AndroidJavaObject banner)
            {
                mParent.OnRaiseBannerShown();
            }

            void onFailedToReceiveAd(AndroidJavaObject banner)
            {
                var errorMessage = banner.Call<AndroidJavaObject>("getErrorMessage");
                mParent.OnRaiseBannerLoadingFailed(errorMessage.Call<string>("toString"));
            }

            void onClick(AndroidJavaObject banner)
            {
                mParent.OnRaiseBannerClicked();
            }
        }
    }
}

#endif