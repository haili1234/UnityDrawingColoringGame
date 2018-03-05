using UnityEngine;
using System.Collections;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Logic
{
	public class CursorZoomCamera: MonoBehaviour
	{
		/// <summary>
		/// The cursor reference.
		/// </summary>
		private Transform cursor;

		/// <summary>
		/// The lerp speed.
		/// </summary>
		public float speed = 10;

		/// <summary>
		/// A temp vector.
		/// </summary>
		private Vector3 tempVector;

		/// <summary>
		/// The new position.
		/// </summary>
		private Vector2 newPosition;

		/// <summary>
		/// The middle camera reference.
		/// </summary>
		private Camera middleCam;

		/// <summary>
		/// The main camera reference.
		/// </summary>
		private Camera mainCam;

		void Start ()
		{
			cursor = GameObject.Find ("Cursor").transform;
			middleCam = GameObject.Find ("MiddleCamera").GetComponent<Camera> ();
			mainCam = GameObject.Find ("MainCamera").GetComponent<Camera> ();
		}

		// Update is called once per frame
		void Update ()
		{
			//set up the new postion
			newPosition.x = cursor.transform.position.x * middleCam.orthographicSize / mainCam.orthographicSize;
			newPosition.x += middleCam.transform.position.x;
			newPosition.y = cursor.transform.position.y * middleCam.orthographicSize / mainCam.orthographicSize;
			newPosition.y += middleCam.transform.position.y;

			tempVector = transform.position;
			tempVector.x = GetValue (transform.position.x, newPosition.x);
			tempVector.y = GetValue (transform.position.y, newPosition.y);

			//apply new postion
			transform.position = tempVector;
		}

		private float GetValue (float currentValue, float targetValue)
		{
			return Mathf.Lerp (currentValue, targetValue, speed * Time.deltaTime);
		}
	}
}