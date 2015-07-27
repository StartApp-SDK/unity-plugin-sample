using UnityEngine;
using UnityEditor;

public class ImportTexture : AssetPostprocessor {

	void Start() {
		EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.Android);
		UnityEditor.PlayerSettings.MTRendering = false;
	}

	void OnPreprocessTexture() {
		TextureImporter importer = assetImporter as TextureImporter;
		importer.textureType  = TextureImporterType.Advanced;
		importer.textureFormat = TextureImporterFormat.RGBA32;
		importer.isReadable = true;

		Object asset = AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Texture2D));
		if (asset) {
			EditorUtility.SetDirty(asset);
		} else {
			importer.textureType  = TextureImporterType.Advanced;
		}
	}
}
