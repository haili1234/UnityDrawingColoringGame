using UnityEngine;
using System.Collections;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Logic
{
	[DisallowMultipleComponent]
	public class AudioSources : MonoBehaviour
	{

		/// <summary>
		/// The loading canvas instance.
		/// </summary>
		public static AudioSources audioSourcesInstance;
	
		// Use this for initialization
		void Awake ()
		{
			if (audioSourcesInstance == null) {
				audioSourcesInstance = this;
				DontDestroyOnLoad (gameObject);
			} else {
				Destroy (gameObject);
			}
		}
	}
}
