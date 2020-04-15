/*
Copyright 2019 StartApp Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

#if UNITY_IOS

using UnityEngine;
using System.Runtime.InteropServices;
using System;

namespace StartApp
{
    public class BannerAdiOS : BannerAd, IDisposable
    {
        bool mDisposed;
        readonly GameObject mGameObject = new GameObject();

        static BannerAdiOS()
        {
            AdSdkiOS.ImplInstance.Setup();
        }

        public BannerAdiOS()
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
            sta_removeBannerObject(mGameObject.name);
            mDisposed = true;
        }

        ~BannerAdiOS()
        {
            Dispose(false);
        }

        public override void PreLoad()
        {
            sta_preloadBanner(mGameObject.name);
        }

        public override void ShowInPosition(BannerPosition position, string tag)
        {
            if (tag == null)
            {
                sta_addBanner(mGameObject.name, (int)position);
                return;
            }
            sta_addBannerWithTag(mGameObject.name, (int)position, tag);
        }

        public override void Hide()
        {
            sta_hideBanner(mGameObject.name);
        }

        public override bool IsShownInPosition(BannerPosition position)
        {
            return sta_isShownInPosition(mGameObject.name, (int)position);
        }

        class ListenerComponent : MonoBehaviour
        {
            public BannerAdiOS Parent { get; set; }

            void OnDidShowBanner()
            {
                Parent.OnRaiseBannerShown();
            }

            void OnFailedLoadBanner(string error)
            {
                Parent.OnRaiseBannerLoadingFailed(error);
            }

            void OnDidClickBanner()
            {
                Parent.OnRaiseBannerClicked();
            }
        }

        [DllImport("__Internal")]
        static extern void sta_addBanner(string gameObjectName, int position);

        [DllImport("__Internal")]
        static extern void sta_addBannerWithTag(string gameObjectName, int position, string tag);

        [DllImport("__Internal")]
        static extern void sta_preloadBanner(string gameObjectName);

        [DllImport("__Internal")]
        static extern void sta_hideBanner(string gameObjectName);

        [DllImport("__Internal")]
        static extern bool sta_isShownInPosition(string gameObjectName, int position);

        [DllImport("__Internal")]
        static extern void sta_removeBannerObject(string gameObjectName);
    }
}

#endif