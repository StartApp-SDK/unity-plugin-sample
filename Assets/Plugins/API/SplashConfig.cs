namespace StartApp
{
    public class SplashConfig
    {
        public enum Theme
        {
            DeepBlue,
            Sky,
            AshenSky,
            Blaze,
            Gloomy,
            Ocean
        }

        public enum Orientation
        {
            Portrait,
            Landscape,
            Auto
        }

        public SplashConfig()
        {
            TemplateTheme = Theme.DeepBlue;
            AppName = null;
            LogoFileName = null;
            ScreenOrientation = Orientation.Auto;
        }

        public Theme TemplateTheme { get; set; }
        public string AppName { get; set; }
        public string LogoFileName { get; set; }
        public Orientation ScreenOrientation { get; set; }
    }
}
