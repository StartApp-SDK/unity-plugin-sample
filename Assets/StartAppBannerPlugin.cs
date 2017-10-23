using UnityEngine;
using System.Collections;
using StartApp;

public class StartAppBannerPlugin : MonoBehaviour {

	string currentOrientation = "Unknown";

	void Start () {
		if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft) {
			currentOrientation = "Landscape";
		}
		else if (Input.deviceOrientation == DeviceOrientation.Portrait) {
			currentOrientation = "Portrait";
		}
	}

	void Update () {
		if (hasOrientationChanged()) {
			reloadStartAppBanners();
		}
	}

	private bool hasOrientationChanged() {
		bool switchedToLandscape = (Screen.orientation.ToString().Equals("Landscape") && currentOrientation.Equals("Portrait"));
		if (switchedToLandscape) {
			currentOrientation = "Landscape";
			return true;
		}
		bool switchedToPortrait = (Screen.orientation.ToString().Equals("Portrait") && currentOrientation.Equals("Landscape"));
		if (switchedToPortrait) {
			currentOrientation = "Portrait";
			return true;
		}
		return false;
	}

	private void reloadStartAppBanners() {
		if (StartAppWrapper.checkIfBannerExists(StartAppWrapper.BannerPosition.BOTTOM)) {
			StartAppWrapper.removeBanner(StartAppWrapper.BannerPosition.BOTTOM);
			StartAppWrapper.addBanner(StartAppWrapper.BannerType.AUTOMATIC, StartAppWrapper.BannerPosition.BOTTOM);
		}
		
		if (StartAppWrapper.checkIfBannerExists(StartAppWrapper.BannerPosition.TOP)) {
			StartAppWrapper.removeBanner(StartAppWrapper.BannerPosition.TOP);
			StartAppWrapper.addBanner(StartAppWrapper.BannerType.AUTOMATIC, StartAppWrapper.BannerPosition.TOP);
		}
	}
}
