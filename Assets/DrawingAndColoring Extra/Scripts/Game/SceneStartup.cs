using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using IndieStudio.DrawingAndColoring.Utility;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Logic
{
	public class SceneStartup : MonoBehaviour
	{
			// Use this for initialization
			void Start ()
			{
					ShowAd ();
			}

			public void ShowAd ()
			{
					if (SceneManager.GetActiveScene().name == "Album") {
						AdsManager.instance.ShowAdvertisment (AdPackage.AdEvent.Event.ON_LOAD_ALBUM_SCENE);
					} else if (SceneManager.GetActiveScene().name == "Game") {
						AdsManager.instance.ShowAdvertisment (AdPackage.AdEvent.Event.ON_LOAD_GAME_SCENE);
					} 
			}
			
			void OnDestroy ()
			{
					AdsManager.instance.HideAdvertisment ();
			}
	}
}
