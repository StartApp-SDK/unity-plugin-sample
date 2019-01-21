using UnityEngine;
using UnityEngine.SceneManagement;
using StartApp;

public class Banners : MonoBehaviour
{
    BannerAd mTopBanner;
    BannerAd mBottomBanner;

    void Start()
    {
        mTopBanner = AdSdk.Instance.CreateBanner();
        mTopBanner.ShowInPosition(BannerAd.BannerPosition.Top);

        mBottomBanner = AdSdk.Instance.CreateBanner();
        mBottomBanner.ShowInPosition(BannerAd.BannerPosition.Bottom);
    }

    void OnGUI()
    {
        int buttonHeight = Screen.height / 5;
        Rect backRect = new Rect(0, 2 * buttonHeight, Screen.width, buttonHeight);
        GUIStyle guiStyle = new GUIStyle(GUI.skin.button);

        if (Screen.orientation == ScreenOrientation.Portrait)
        {
            guiStyle.fontSize = Screen.width / 12;
        }
        else
        {
            guiStyle.fontSize = Screen.height / 12;
        }

        if (GUI.Button(backRect, "Back", guiStyle) || Input.GetKeyUp(KeyCode.Escape))
        {
            mTopBanner.Hide();
            mBottomBanner.Hide();
            SceneManager.LoadScene("StartApp", LoadSceneMode.Single);
        }
    }
}
