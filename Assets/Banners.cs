using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using StartApp;

public class Banners : MonoBehaviour {

	void Start () {
		StartAppWrapper.addBanner (StartAppWrapper.BannerType.STANDARD, StartAppWrapper.BannerPosition.BOTTOM);
		StartAppWrapper.addBanner (StartAppWrapper.BannerType.THREED, StartAppWrapper.BannerPosition.TOP);
	}

	void OnGUI () {
		int buttonHeight = Screen.height / 5;
		Rect backRect = new Rect(0, 2 * buttonHeight, Screen.width, buttonHeight);
		GUIStyle guiStyle = new GUIStyle(GUI.skin.button);
		if (Screen.orientation == ScreenOrientation.Portrait) {
			guiStyle.fontSize = Screen.width / 12;
		} else {
			guiStyle.fontSize = Screen.height / 12;
		}
		if (GUI.Button(backRect, "Back", guiStyle) || Input.GetKeyUp (KeyCode.Escape)) {
			StartAppWrapper.removeBanner(StartAppWrapper.BannerPosition.TOP);
			StartAppWrapper.removeBanner (StartAppWrapper.BannerPosition.BOTTOM);
			SceneManager.LoadScene("StartApp", LoadSceneMode.Single);
		}
	}
}
