using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Logic
{
	public class WebPrint : MonoBehaviour
	{
		/// <summary>
		/// Whether process is running or not.
		/// </summary>
		public static bool isRunning;

		/// <summary>
		/// The flash effect fade.
		/// </summary>
		public Animator flashEffect;

		/// <summary>
		/// The flash sound effect.
		/// </summary>
		public AudioClip flashSFX;

		/// <summary>
		/// The objects bet hide/show on screen capturing.
		/// </summary>
		public Transform[] objects;

		/// <summary>
		/// The logo on the bottom of the page.
		/// </summary>
		public Transform bottomLogo;


		void Start(){
			isRunning = false;
		}

		/// <summary>
		/// Print the screen.
		/// </summary>
		public void PrintScreen ()
		{
			#if(UNITY_WEBPLAYER || UNITY_WEBGL || UNITY_EDITOR)
				Debug.LogWarning("Print feature works only in the Web platform, check out the Manual.pdf to implement print feature...");
				StartCoroutine ("PrintScreenCoroutine");
			#endif
		}

		public IEnumerator PrintScreenCoroutine ()
		{
			isRunning = true;

			HideObjects ();
			if(bottomLogo!=null)
				bottomLogo.gameObject.SetActive (true);
			string imageName = "DrawingAndColoring-"+System.DateTime.Now.ToString ("yyyy-MM-dd_HH-mm-ss");

			//Capture screen shot
			yield return new WaitForEndOfFrame();
			Texture2D texture = new Texture2D(Screen.width, Screen.height);
			texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
			texture.Apply();

			flashEffect.SetTrigger ("Run");
			if(flashSFX !=null)
				AudioSource.PlayClipAtPoint (flashSFX, Vector3.zero, 1);
			yield return new WaitForSeconds (1);
			ShowObjects ();
			if(bottomLogo!=null)
				bottomLogo.gameObject.SetActive (false);

			Application.ExternalCall("PrintImage", System.Convert.ToBase64String(texture.EncodeToPNG()),imageName);
			isRunning = false;

		}

		/// <summary>
		/// Hide the objects.
		/// </summary>
		private void HideObjects ()
		{
			if (objects == null) {
				return;
			}

			foreach (Transform obj in objects) {
				if(obj!=null)
					obj.gameObject.SetActive (false);
			}
		}

		/// <summary>
		/// Show the objects.
		/// </summary>
		private void ShowObjects ()
		{
			if (objects == null) {
				return;
			}
			
			foreach (Transform obj in objects) {
				obj.gameObject.SetActive (true);
			}
		}
	}
}