#if UNITY_IOS

using UnityEngine;
using System.Runtime.InteropServices;
using System;

namespace StartApp
{
    public class InterstitialAdiOS : InterstitialAd, IDisposable
    {
        bool mDisposed;
        readonly GameObject mGameObject = new GameObject();

        static InterstitialAdiOS()
        {
            AdSdkiOS.ImplInstance.Setup();
        }

        public InterstitialAdiOS()
        {
            mGameObject.name = mGameObject.GetInstanceID().ToString();
            mGameObject.AddComponent<ListenerComponent>().Parent = this;
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
            sta_removeAdObject(mGameObject.name);
            mDisposed = true;
        }

        ~InterstitialAdiOS()
        {
            Dispose(false);
        }

        public override void LoadAd(AdType mode)
        {
            if (mode == AdType.Rewarded)
            {
                sta_loadRewardedVideoAd(mGameObject.name);
            }
            else if (mode == AdType.Video)
            {
                sta_loadVideoAd(mGameObject.name);
            }
            else
            {
                sta_loadAd(mGameObject.name);
            }
        }

        public override bool ShowAd(string tag)
        {
            if (tag == null)
            {
                sta_showAd(mGameObject.name);
                return true;
            }

            sta_showAdWithTag(mGameObject.name, tag);
            return true;
        }

        public override bool IsReady()
        {
            return sta_isReady(mGameObject.name);
        }

        class ListenerComponent : MonoBehaviour
        {
            public InterstitialAdiOS Parent { get; set; }

            void OnDidLoadAd()
            {
                Parent.OnRaiseAdLoaded();
            }

            void OnFailedLoadAd(string error)
            {
                Parent.OnRaiseAdLoadingFailed(error);
            }

            void OnDidShowAd()
            {
                Parent.OnRaiseAdShown();
            }

            void OnDidCloseAd()
            {
                Parent.OnRaiseAdClosed();
            }

            void OnDidClickAd()
            {
                Parent.OnRaiseAdClicked();
            }

            void OnDidCompleteVideo()
            {
                Parent.OnRaiseAdVideoCompleted();
            }
        }

        [DllImport("__Internal")]
        static extern void sta_loadAd(string gameObjectName);

        [DllImport("__Internal")]
        static extern void sta_showAd(string gameObjectName);

        [DllImport("__Internal")]
        static extern void sta_showAdWithTag(string gameObjectName, string tag);

        [DllImport("__Internal")]
        static extern void sta_loadRewardedVideoAd(string gameObjectName);

        [DllImport("__Internal")]
        static extern void sta_loadVideoAd(string gameObjectName);

        [DllImport("__Internal")]
        static extern bool sta_isReady(string gameObjectName);

        [DllImport("__Internal")]
        static extern void sta_removeAdObject(string gameObjectName);
    }
}

#endif