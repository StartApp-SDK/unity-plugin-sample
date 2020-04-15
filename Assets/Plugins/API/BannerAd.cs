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
