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
