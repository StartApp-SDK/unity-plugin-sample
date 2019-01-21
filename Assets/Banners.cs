using UnityEngine;
using UnityEngine.SceneManagement;
using StartApp;

public class Banners : MonoBehaviour
{
    BannerAd topBanner;
    BannerAd bottomBanner;

    void Start()
    {
        topBanner = AdSdk.Instance.CreateBanner();
        topBanner.ShowInPosition(BannerAd.BannerPosition.Top);

        bottomBanner = AdSdk.Instance.CreateBanner();
        bottomBanner.ShowInPosition(BannerAd.BannerPosition.Bottom);
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

		if (GUI.Button(backRect, "Back", guiStyle) || Input.GetKeyUp (KeyCode.Escape))
        {
            topBanner.Hide();
            bottomBanner.Hide();
			SceneManager.LoadScene("StartApp", LoadSceneMode.Single);
		}
	}
}
