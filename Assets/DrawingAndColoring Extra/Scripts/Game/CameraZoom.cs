using UnityEngine;
using System.Collections;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Logic
{
	public class CameraZoom : MonoBehaviour
	{       
		public static bool zoomStartedBefore;//whether the zoom started before the drag or not
		public static bool isCameraZooming;//whether the camera is zooming in/out
		public Camera targetCamera;//the target Camera
		public Vector2 cameraScope = new Vector2 (2, 5);//camera scope size
		[Range(0,30)]
		public float speed = 5;//lerp speed
		private Vector2 firstTouchPosition = Vector2.zero;//first touch position
		private Vector2 secondTouchPosition = Vector2.zero;//second touch position 
		private bool twoTouchesOrMoreDetected;//if there are touch to
		public static bool isRunning;//whether the script is running or not
		public bool enableWheelZoom = true;//whether to enable wheel zoom or not
		public static float currentOrthographicSize;//the current orthographic size
		private static bool zoomInPress,zoomOutPress;//whether we have continuous zoom in/out
		private static float initialOrthographicSize;//the initial orthographic size
		private float currentDistance;//the current distance between the first touch and the second touch
		private float initialDistance;//the initial distance between the first touch and the second touch
		private float offset;//the offset between the current distance and the initial distance
		private static float factor = 0.01f;//offset multiply factor
		public GameManager gameManager;//the game manager reference

		// Use this for initialization
		void Start ()
		{
			//Setting up references
			isRunning = true;
			gameManager = GameManager.FindObjectOfType<GameManager> ();
			zoomInPress = zoomOutPress = false;
			zoomStartedBefore = false;
			isCameraZooming = false;
			currentOrthographicSize = targetCamera.orthographicSize;
			targetCamera.orthographicSize = targetCamera.orthographicSize;
			initialOrthographicSize = currentOrthographicSize;
		}
		
		// Update is called once per frame
		void Update ()
		{
			if (!isRunning || !GameManager.pointerInDrawArea) {
				return;
			}

			OnScreenTouches ();

			if (enableWheelZoom) {
				if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
					ZoomIn (1200);
				} else if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
					ZoomOut (1200);
				}
			}
		}

		void LateUpdate ()
		{
			LerpToSize ();

			if (zoomInPress) {
				ZoomIn(500);
			} else if (zoomOutPress) {
				ZoomOut(500);
			}
		}

		/// <summary>
		/// Handle screen touches
		/// </summary>
		private void OnScreenTouches ()
		{
			if (!(Input.touchCount > 1)) {
				twoTouchesOrMoreDetected = false;
				isCameraZooming = false;
				return;
			}

			zoomStartedBefore = true;
			isCameraZooming = true;

			firstTouchPosition = Input.GetTouch (0).position;//first touch position
			secondTouchPosition = Input.GetTouch (1).position;//second touch position
			       
			currentDistance = Vector2.Distance (firstTouchPosition, secondTouchPosition);

			if (!twoTouchesOrMoreDetected) {
				twoTouchesOrMoreDetected = true;
				initialDistance = currentDistance;
			}

			offset = initialDistance - currentDistance;

			if (offset != 0) {
				currentOrthographicSize += offset * factor;
				initialDistance = currentDistance;
			}
		}

		/// <summary>
		/// Reset the zoom.
		/// </summary>
		public static void ResetZoom(){
			currentOrthographicSize = initialOrthographicSize;
		}

		/// <summary>
		/// Raise the zoom in press event.
		/// </summary>
		public void OnZoomInPress(){
			zoomInPress = true;
			if (gameManager.currentTool.feature == Tool.ToolFeature.Hand) {
				CameraDrag.isRunning = false;
			}
		}

		/// <summary>
		/// Raise the zoom in press release event.
		/// </summary>
		public void OnZoomInPressRelease(){
			zoomInPress = false;
			if (gameManager.currentTool.feature == Tool.ToolFeature.Hand) {
				CameraDrag.isRunning = true;
			}
		}

		/// <summary>
		/// Raise the zoom out press event.
		/// </summary>
		public void OnZoomOutPress(){
			zoomOutPress = true;
			CameraDrag.isRunning = false;
		}

		/// <summary>
		/// Raise the zoom out press release event.
		/// </summary>
		public void OnZoomOutPressRelease(){
			zoomOutPress = false;
			if (gameManager.currentTool.feature == Tool.ToolFeature.Hand) {
				CameraDrag.isRunning = true;
			}
		}

		/// <summary>
		/// Zooms the in by a value.
		/// </summary>
		/// <param name="value">Value.</param>
		public static void ZoomIn(float value){
			currentOrthographicSize -= value * factor * Time.deltaTime;
		}

		/// <summary>
		/// Zooms the out by a value.
		/// </summary>
		/// <param name="value">Value.</param>
		public static void ZoomOut(float value){
			currentOrthographicSize += value * factor * Time.deltaTime;
		}

		/// <summary>
		/// Lerp the target camera to a size
		/// </summary>
		private void LerpToSize ()
		{
			currentOrthographicSize = Mathf.Clamp (currentOrthographicSize, cameraScope.x, cameraScope.y);
			targetCamera.orthographicSize = Mathf.Lerp (targetCamera.orthographicSize, currentOrthographicSize, speed * Time.smoothDeltaTime);
		}
	}
}