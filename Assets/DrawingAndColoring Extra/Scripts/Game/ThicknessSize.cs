using UnityEngine;
using UnityEngine.UI;
using System.Collections;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Logic
{
	[DisallowMultipleComponent]
	public class ThicknessSize : MonoBehaviour {

		[Range(0.025f,5)]
		/// <summary>
		/// The thickness value.
		/// </summary>
		public float value = 0.025f;

		/// <summary>
		/// Enable ThicknessSize selection.
		/// </summary>
		public void EnableSelection(){
			Color imgColor = GetComponent<Image> ().color;
			imgColor.a = 0.5f;
			GetComponent<Image> ().color = imgColor;
		}

		/// <summary>
		/// Disable ThicknessSize selection.
		/// </summary>
		public void DisableSelection(){
			Color imgColor = GetComponent<Image> ().color;
			imgColor.a = 1;
			GetComponent<Image> ().color = imgColor;
		}
	}
}
