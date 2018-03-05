using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

public class UnityAd : MonoBehaviour
{
	/// <summary>
	/// The android game id.
	/// </summary>
	public string androidGameID;

	/// <summary>
	/// The ios game id.
	/// </summary>
	public string iosGameID;

	/// <summary>
	/// Enable test mode or not.
	/// </summary>
	public bool testMode;

	// Use this for initialization
	void Start ()
	{
		//initialize unity ad
		#if UNITY_ADS
			#if UNITY_ANDROID
				Advertisement.Initialize (androidGameID, testMode);
			#elif UNITY_IPHONE
				Advertisement.Initialize(iosGameID, testMode);
			#endif
		#endif
	}

	/// <summary>
	/// Show the unity ad.
	/// </summary>
	public void ShowUnityAd(){
		StartCoroutine("UnityAdCoroutine");
	}

	/// <summary>
	/// Unity ad coroutine.
	/// </summary>
	/// <returns>The ad coroutine.</returns>
	private IEnumerator UnityAdCoroutine(){
		#if UNITY_ADS
		while (!Advertisement.IsReady())
		{
			yield return new WaitForSeconds(0.1f);
		}

		Advertisement.Show();
		#else
			yield return null;
		#endif
	}
}
