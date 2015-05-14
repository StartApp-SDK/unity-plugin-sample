using UnityEngine;
using System.Collections;
using StartApp;

public class StartAppBackPlugin : MonoBehaviour{

	void Start () {
	#if UNITY_ANDROID
		StartAppWrapper.loadAd();
	#endif
    }

	void Update () {
	#if UNITY_ANDROID
		if (Input.GetKeyUp (KeyCode.Escape)) {
			if (StartAppWrapper.onBackPressed(gameObject.name) == false) {
				exit();
			}
		}
	#endif
    }
	
	#if UNITY_ANDROID
	void exit() {
		Application.Quit ();
	}
	#endif

}