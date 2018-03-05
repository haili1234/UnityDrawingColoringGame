using UnityEngine;
using UnityEditor;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

public class PostProcessor : AssetPostprocessor
{
	private static readonly string googleMobileAdsPath = Application.dataPath + "/GoogleMobileAds";
	private static readonly  string chartBoosteAdsPath = Application.dataPath + "/Chartboost";
	private static readonly string googleMobileAdsDefine = "GOOGLE_MOBILE_ADS;";
	private static readonly string chartBoosteAdsDefine = "CHARTBOOST_ADS;";

	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		string defines = "";

		if (System.IO.Directory.Exists (googleMobileAdsPath)) {
			defines += googleMobileAdsDefine;
		}

		if (System.IO.Directory.Exists (chartBoosteAdsPath)) {
			defines += chartBoosteAdsDefine;
		}

		if (!string.IsNullOrEmpty (defines)) {
			PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.Android, defines);
			PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.iOS, defines);
		}
	}
}