using System;

namespace StartApp
{
    public abstract class BannerAd
    {
        public enum BannerPosition
        {
            Top = 1,
            Bottom
        }

        public event EventHandler RaiseBannerShown;
        public event EventHandler<MessageArgs> RaiseBannerLoadingFailed;
        public event EventHandler RaiseBannerClicked;

        public abstract void PreLoad();
        public abstract void ShowInPosition(BannerPosition position = BannerPosition.Bottom, string tag = null);
        public abstract void Hide();
        public abstract bool IsShownInPosition(BannerPosition position);


        protected void OnRaiseBannerShown()
        {
            EventHandler handler = RaiseBannerShown;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        protected void OnRaiseBannerLoadingFailed(string error)
        {
            EventHandler<MessageArgs> handler = RaiseBannerLoadingFailed;
            if (handler != null)
            {
                handler(this, new MessageArgs(error));
            }
        }

        protected void OnRaiseBannerClicked()
        {
            EventHandler handler = RaiseBannerClicked;
            if (handler != null)
            {
                handler(this, null);
            }
        }
    }
}
