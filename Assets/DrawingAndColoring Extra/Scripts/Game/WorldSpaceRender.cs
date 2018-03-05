using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Logic
{
	public class WorldSpaceRender : MonoBehaviour {

		// Use this for initialization
		void Start () {
			GetComponent<Canvas> ().renderMode = RenderMode.WorldSpace;	
		}
	}
}