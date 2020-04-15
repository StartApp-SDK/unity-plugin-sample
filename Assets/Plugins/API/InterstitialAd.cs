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

using System;

namespace StartApp
{
    public abstract class InterstitialAd
    {
        public enum AdType
        {
            Automatic,
            FullScreen,
            OfferWall,
            Video,
            Rewarded
        }

        public event EventHandler RaiseAdLoaded;
        public event EventHandler<MessageArgs> RaiseAdLoadingFailed;
        public event EventHandler RaiseAdShown;
        public event EventHandler RaiseAdClosed;
        public event EventHandler RaiseAdClicked;
        public event EventHandler RaiseAdVideoCompleted;

        public abstract void LoadAd(AdType mode = AdType.Automatic);
        public abstract bool ShowAd(string tag = null);
        public abstract bool IsReady();

        protected void OnRaiseAdLoaded()
        {
            EventHandler handler = RaiseAdLoaded;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        protected void OnRaiseAdLoadingFailed(string error)
        {
            EventHandler<MessageArgs> handler = RaiseAdLoadingFailed;
            if (handler != null)
            {
                handler(this, new MessageArgs(error));
            }
        }

        protected void OnRaiseAdShown()
        {
            EventHandler handler = RaiseAdShown;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        protected void OnRaiseAdClosed()
        {
            EventHandler handler = RaiseAdClosed;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        protected void OnRaiseAdClicked()
        {
            EventHandler handler = RaiseAdClicked;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        protected void OnRaiseAdVideoCompleted()
        {
            EventHandler handler = RaiseAdVideoCompleted;
            if (handler != null)
            {
                handler(this, null);
            }
        }
    }
}
