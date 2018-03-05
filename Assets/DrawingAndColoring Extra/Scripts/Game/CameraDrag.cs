using UnityEngine;
using System.Collections;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Logic
{
	public class CameraDrag : MonoBehaviour
	{
		public Camera targetCamera;//the target camera
		private Touch touch;//the sceen touch
		private Vector3 currentTouchPosition = Vector3.zero;//the current touch position
		private Vector3 previousTouchPosition;//the previous touch position
		private Vector3 offset = Vector3.zero;//the offset between the touch and the scene camera
		private Vector3 firstTouchPosition;// the first touch postion on the screen
		private Vector3 targetCameraPosition = Vector3.zero;//scene camera position
		private Vector2 yClamp, xClamp;//use to limit values
		[Range(0,30)]
		public float speed = 10f;//lerp speed
		private float maxSpeed;// max lerp speed
		private const float releaseFactor = 5;//touch relase factor or multiplier
		private float xDistance, yDistance;// horizontal and vertical distance between first touch and current touch
		private float xMirrorPosition, yMirrorPosition;//a postion to be lerped by the camera
		private float initialOrthographicSize;//the initial orthographic size of the camera
		private float difference;//the difference between initial orthographic size and the current one 
		public static bool touchBegan;//wehter the touch began on the screen or not
		public static bool isRunning;//whether the script is running or not
		public static bool isCameraDraging;//whether the camera is draging or not
		private bool rightClickBegan;//whether the user clicked on the right mouse button or not
		public GameManager gameManager;//the game manager reference

		// Use this for initialization
		void Start ()
		{
			//Setting up references
			rightClickBegan = touchBegan = isRunning = false;
			if (gameManager == null) {
				gameManager = GameObject.FindObjectOfType<GameManager> ();
			}
			maxSpeed = speed;
			isCameraDraging = false;
			initialOrthographicSize = targetCamera.orthographicSize;
		}

		// Update is called once per frame
		void Update ()
		{
			if (!GameManager.pointerInDrawArea) {
				return;
			}

			if (!Application.isMobilePlatform) {
				if (Input.GetMouseButtonDown (1)) {
					isRunning = true;
					rightClickBegan = true;
					TouchBegan (Input.mousePosition);
				}
			}

			if (!isRunning) {
				return;
			}

			if (Application.isMobilePlatform) {
				OnSceeenTouch ();
			} else {

				if (Input.GetMouseButtonDown (0)) {
					TouchBegan (Input.mousePosition);
				} else if (Input.GetMouseButtonUp (0) || Input.GetMouseButtonUp (1)) {
					if (rightClickBegan) {
						TouchRelease ();
						if (gameManager.currentTool.feature != Tool.ToolFeature.Hand) {
							isRunning = false;
						}
					} else if (touchBegan) {
						TouchRelease ();
					}
				}
				
				if (touchBegan || rightClickBegan) {
					TouchMoved (Input.mousePosition);
				}
			}
		}

		void LateUpdate ()
		{
			//Get the offset between the initial orthographic and the current orthographic size
			difference = initialOrthographicSize - CameraZoom.currentOrthographicSize;
			if (difference > 0) {
				xClamp.x = -difference * targetCamera.aspect;
				xClamp.y = difference * targetCamera.aspect;
			
				yClamp.x = -difference;
				yClamp.y = difference;
			} else {
				xClamp.x = difference * targetCamera.aspect;
				xClamp.y = -difference * targetCamera.aspect;

				yClamp.x = difference;
				yClamp.y = -difference;
			}

			LerpToTouchPosition ();
		}

		/// <summary>
		/// On sceeen touch event.
		/// </summary>
		private void OnSceeenTouch ()
		{
			if (Input.touchCount != 1 || CameraZoom.isCameraZooming) {
				return;
			}

			touch = Input.GetTouch (0);

			if (touch.phase == TouchPhase.Began) {
				TouchBegan (touch.position);
			} else if (touch.phase == TouchPhase.Moved) {
				if (CameraZoom.zoomStartedBefore) {
					CameraZoom.zoomStartedBefore = false;
					TouchBegan (touch.position);
					return;
				}
				TouchMoved (touch.position);
							
			} else  if (touch.phase == TouchPhase.Ended) {
				if (!CameraZoom.zoomStartedBefore && touchBegan) {
					TouchRelease ();
				}
			}
		}

		/// <summary>
		/// On Touch release.
		/// </summary>
		private void TouchRelease ()
		{
			speed = maxSpeed / 2.0f;
			currentTouchPosition.x = currentTouchPosition.x + (currentTouchPosition.x - previousTouchPosition.x) * releaseFactor;
			currentTouchPosition.y = currentTouchPosition.y + (currentTouchPosition.y - previousTouchPosition.y) * releaseFactor;
			
			//reset flags
			isCameraDraging = false;
			touchBegan = false;
			rightClickBegan = false;
		}

		/// <summary>
		/// On Touch began event.
		/// </summary>
		/// <param name="screenTouchPosition">Screen touch position.</param>
		private void TouchBegan (Vector3 screenTouchPosition)
		{
			touchBegan = true;
			speed = maxSpeed; 
			targetCameraPosition = targetCamera.transform.position;
			currentTouchPosition = (Camera.main.ScreenToWorldPoint (screenTouchPosition) * (1.0f/Camera.main.orthographicSize)) *targetCamera.orthographicSize;
			previousTouchPosition = currentTouchPosition;
			firstTouchPosition = currentTouchPosition;

			offset = currentTouchPosition - targetCameraPosition;
		}

		/// <summary>
		/// Touch moved event.
		/// </summary>
		/// <param name="screenTouchPosition">Screen touch position.</param>
		private void TouchMoved (Vector3 screenTouchPosition)
		{
			isCameraDraging = true;
			previousTouchPosition = currentTouchPosition;
			currentTouchPosition = (Camera.main.ScreenToWorldPoint (screenTouchPosition) * (1.0f/Camera.main.orthographicSize)) *targetCamera.orthographicSize;
		}

		/// <summary>
		/// Lerp the target camera to the touch position
		/// </summary>
		private void LerpToTouchPosition ()
		{
			targetCameraPosition = targetCamera.transform.position;
	              
			xDistance = currentTouchPosition.x - firstTouchPosition.x;//the x distance between the first touch and the current touch
			yDistance = currentTouchPosition.y - firstTouchPosition.y;//the y distance between the first touch and the current touch

			xMirrorPosition = firstTouchPosition.x - (offset.x + xDistance);//the mirror or the opposite x position of the (offset.x + xDistance)
			yMirrorPosition = firstTouchPosition.y - (offset.y + yDistance);//the mirror or the opposite y position of the (offset.y + yDistance)

			targetCameraPosition.x = Mathf.Lerp (targetCameraPosition.x, xMirrorPosition, speed * Time.smoothDeltaTime);
			targetCameraPosition.y = Mathf.Lerp (targetCameraPosition.y, yMirrorPosition, speed * Time.smoothDeltaTime);

			targetCameraPosition.x = Mathf.Clamp (targetCameraPosition.x, xClamp.x, xClamp.y);			
			targetCameraPosition.y = Mathf.Clamp (targetCameraPosition.y, yClamp.x, yClamp.y);

			if (targetCameraPosition.x == xClamp.x || targetCameraPosition.x == xClamp.y) {
				firstTouchPosition.x = currentTouchPosition.x;
				offset.x = currentTouchPosition.x - targetCameraPosition.x;
			}
			
			if (targetCameraPosition.y == yClamp.x || targetCameraPosition.y == yClamp.y) {
				firstTouchPosition.y = currentTouchPosition.y;
				offset.y = currentTouchPosition.y - targetCameraPosition.y;
			}

			targetCamera.transform.position = targetCameraPosition;
		}

		public Vector3 GetTargetPosition(){
			return targetCameraPosition;
		}
	}
}