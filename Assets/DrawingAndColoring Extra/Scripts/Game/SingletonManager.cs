using UnityEngine;
using System.Collections;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Logic
{
	public class SingletonManager : MonoBehaviour {

		public GameObject[] values;

		// Use this for initialization
		void Awake () {
			InitManagers ();
		}

		private void InitManagers ()
		{
			if (values == null) {
				return;
			}

			foreach (GameObject value in values) {
				if (GameObject.Find (value.name) == null) {
					GameObject temp = Instantiate (value, Vector3.zero, Quaternion.identity) as GameObject;
					temp.name = value.name;
				}
			}
		}
	}
}
