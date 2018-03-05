using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using IndieStudio.DrawingAndColoring.Utility;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Logic
{
	[DisallowMultipleComponent]
	public class ScrollSlider : MonoBehaviour
	{	
		/// <summary>
		/// The groups grid layout reference
		/// </summary>
		public GridLayoutGroup groupsGridLayout;

		/// <summary>
		/// The sscroll rect component
		/// </summary>
		private ScrollRect scrollRect;

		/// <summary>
		/// The scroll content
		/// </summary>
		public RectTransform scrollContent;

		/// <summary>
		/// The pointers grid layout.
		/// </summary>
		public GridLayoutGroup pointersGridLayout;

		/// <summary>
		/// The rect transform of this instance
		/// </summary>
		private RectTransform rectTransform;

		/// <summary>
		/// The next button.
		/// </summary>
		public Transform nextButton;

		/// <summary>
		/// The previous button.
		/// </summary>
		public Transform previousButton;

		/// <summary>
		/// The current group text.
		/// </summary>
		public Text currentGroupText;

		/// <summary>
		/// The pointer enabled sprite.
		/// </summary>
		public Sprite pointerEnabled;

		/// <summary>
		/// The pointer disabled sprite.
		/// </summary>
		public Sprite pointerDisabled;

		/// <summary>
		/// The lerp speed.
		/// </summary>
		[Range(5,100)]
		public float lerpSpeed = 8;

		/// <summary>
		/// The group width ratio.
		/// </summary>
		[Range(0.1f,5.0f)]
		public float groupWidthRatio = 1;

		/// <summary>
		/// The group height ratio.
		/// </summary>
		[Range(0.1f,5.0f)]
		public float groupHeightRatio = 1;

		/// <summary>
		/// The group spacing ratio.
		/// </summary>
		[Range(0,5.0f)]
		public float groupSpacingRatio = 0.3f;

		/// <summary>
		/// The pointers width ratio.
		/// </summary>
		[Range(0.01f,5.0f)]
		public float pointersWidthRatio = 0.036f;

		/// <summary>
		/// The pointers height ratio.
		/// </summary>
		[Range(0.01f,5.0f)]
		public float pointersHeightRatio = 1;

		/// <summary>
		/// The pointers spacing ratio.
		/// </summary>
		[Range(0,5.0f)]
		public float pointersSpacingRatio = 0.002f;

		/// <summary>
		/// A loop scroll.
		/// </summary>
		public bool loop = true;

		/// <summary>
		/// The groups list.
		/// </summary>
		[HideInInspector]
		public GameObject[] groups;

		/// <summary>
		/// The pointers list.
		/// </summary>
		[HideInInspector]
		public GameObject[] pointers;

		/// <summary>
		/// The holder of the callback on change group .
		/// </summary>
		public Transform callBackHolder;

		/// <summary>
		/// The callback on change group .
		/// </summary>
		public string changeGroupCallBack = "OnChangeGroup";

		/// <summary>
		/// The index of the current group.
		/// </summary>
		[HideInInspector]
		public int currentGroupIndex = 0;

		/// <summary>
		/// A temp color.
		/// </summary>
		private Color tempColor;

		/// <summary>
		/// A temp anchroed postion.
		/// </summary>
		private Vector3 tempAnchoredPostion;

		/// <summary>
		/// The group anchored position.
		/// </summary>
		private Vector3 groupAnchoredPosition;

		/// <summary>
		/// Whether lerping to the group or not
		/// </summary>
		private bool isLerping;

		/// <summary>
		/// The screen's initial aspect ratio.
		/// </summary>
		private float initialAspectRatio;

		/// <summary>
		///A temp lerp speed.
		/// </summary>
		private float tempLerpSpeed;

		void Start ()
		{
			Init();
		}

		public void Init(){
			//Setting up references and initial values
			pointers = CommonUtil.FindGameObjectsOfTag ("Pointer");
			groups = CommonUtil.FindGameObjectsOfTag ("Group");
			scrollRect = GetComponent<ScrollRect> ();
			rectTransform = GetComponent<RectTransform> (); 
			initialAspectRatio = Camera.main.aspect;
			CalculateGridLayoutsValues ();
			isLerping = true;

			if (groups == null) {
				Debug.LogWarning ("No groups found");
			} else if (groups.Length == 0) {
				Debug.LogWarning ("No groups found");
			}

			SnapToGroup ();
			GoToCurrentGroup ();
			tempLerpSpeed = 256;
		}

		void Update ()
		{
			if (Camera.main.aspect != initialAspectRatio) {
				CalculateGridLayoutsValues ();
				initialAspectRatio = Camera.main.aspect;
			}

			if (isLerping)
				SnapToGroup ();

			HandleInput ();
		}

		/// <summary>
		/// Calculate the grid layouts values such as Spacing,Cell Size,Constraint Count.
		/// </summary>
		private void CalculateGridLayoutsValues ()
		{
			if (groupsGridLayout != null){
				groupsGridLayout.spacing = new Vector2 (rectTransform.rect.width * groupSpacingRatio, 0);
				groupsGridLayout.cellSize = new Vector2 (rectTransform.rect.width * groupWidthRatio, rectTransform.rect.height * groupHeightRatio);
			}
			if (pointersGridLayout != null){
				pointersGridLayout.spacing = new Vector2 (pointersGridLayout.GetComponent<RectTransform> ().rect.width * pointersSpacingRatio, 0);
				pointersGridLayout.cellSize = new Vector2 (pointersGridLayout.GetComponent<RectTransform> ().rect.width * pointersWidthRatio, pointersGridLayout.GetComponent<RectTransform> ().rect.height * pointersHeightRatio);
			}
		}

		/// <summary>
		/// Handles the user input.
		/// </summary>
		private void HandleInput ()
		{
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				NextGroup ();
			} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				PreviousGroup ();
			}
		}

		/// <summary>
		/// Calcualte the group anchored position.
		/// </summary>
		/// <param name="group">Group.</param>
		public void CalcualteGroupAnchoredPosition (RectTransform group)
		{
			groupAnchoredPosition = (Vector2)scrollRect.transform.InverseTransformPoint (scrollContent.position) - (Vector2)scrollRect.transform.InverseTransformPoint (group.position) + new Vector2 (rectTransform.rect.width / 2.0f, 0);
		}

		/// <summary>
		/// Snaps to the group.
		/// </summary>
		public void SnapToGroup ()
		{
			if (groups == null) {
				return;
			}

			if (groups.Length == 0) {
				return;
			}

			Canvas.ForceUpdateCanvases ();

			tempAnchoredPostion.x = Mathf.Lerp (tempAnchoredPostion.x, groupAnchoredPosition.x, tempLerpSpeed * Time.smoothDeltaTime);
			tempAnchoredPostion.y = 0;
			scrollContent.anchoredPosition = tempAnchoredPostion;

			if (Vector2.Distance (tempAnchoredPostion, groupAnchoredPosition) <= 5) {
				DisableLerping ();
			}
		}

		/// <summary>
		/// Go to the current group.
		/// </summary>
		public void GoToCurrentGroup ()
		{
			if (groups == null) {
				return;
			}

			if (groups.Length == 0) {
				return;
			}

			tempLerpSpeed = lerpSpeed;

			DisableFarGroups (currentGroupIndex);

			if (groups.Length <= 1) {
				DisableNextButton ();
				DisablePreviousButton ();
			} else if (currentGroupIndex == 0) {
				DisablePreviousButton ();
				EnableNextButton ();
			} else if (currentGroupIndex == groups.Length - 1) {
				DisableNextButton ();
				EnablePreviousButton ();
			} else {
				EnableNextButton ();
				EnablePreviousButton ();
			}

			if (currentGroupText != null)
				currentGroupText.text = (currentGroupIndex + 1) + "/" + groups.Length;

			scrollRect.StopMovement ();
			CalcualteGroupAnchoredPosition (groups [currentGroupIndex].GetComponent<RectTransform> ());
			tempAnchoredPostion = scrollContent.anchoredPosition;
			EnableCurrentPointer ();
			EnableLerping ();
			if (callBackHolder != null && !string.IsNullOrEmpty(changeGroupCallBack)) {
				callBackHolder.SendMessage(changeGroupCallBack,currentGroupIndex,SendMessageOptions.DontRequireReceiver);
			}
		}

		/// <summary>
		/// Go to the next group.
		/// </summary>
		public void NextGroup ()
		{
			if (groups == null) {
				return;
			}

			if (groups.Length == 0) {
				return;
			}

			if (currentGroupIndex + 1 >= groups.Length) {
				if (loop) {
					DisableCurrentPointer ();
					currentGroupIndex = 0;
				}
			} else {
				DisableCurrentPointer ();
				currentGroupIndex += 1;
			}

			GoToCurrentGroup ();
		}

		/// <summary>
		/// Go to the previous group.
		/// </summary>
		public void PreviousGroup ()
		{
			if (groups == null) {
				return;
			}

			if (groups.Length == 0) {
				return;
			}

			if (currentGroupIndex - 1 < 0) {
				if (loop) {
					DisableCurrentPointer ();
					currentGroupIndex = groups.Length - 1;
				}
			} else {
				DisableCurrentPointer ();
				currentGroupIndex -= 1;
			}
			GoToCurrentGroup ();
		}


		/// <summary>
		/// Raises the drag begin event.
		/// </summary>
		public void OnDragBegin ()
		{
			DisableLerping ();
		}

		/// <summary>
		/// Raises the drag end event.
		/// </summary>
		public void OnDragEnd ()
		{
			if (groups == null) {
				return;
			}

			if (groups.Length == 0) {
				return;
			}

			if (scrollRect.velocity.x < -350) {
				//Scroll to the next
				NextGroup ();
			} else if (scrollRect.velocity.x > 350) {
				//Scroll to the previous
				PreviousGroup ();
			} else {
				//Scroll to the closest
				RectTransform closestGroup = groups [0].GetComponent<RectTransform> ();
				float dist1, dist2 = Mathf.Infinity;
				foreach (GameObject group in groups) {
					//The Horizontal Distance between the current group and levels panel
					dist1 = Mathf.Abs (group.transform.position.x - rectTransform.transform.position.x);

					if (dist1 < dist2) {
						closestGroup = group.GetComponent<RectTransform> ();
						//The Horizontal Distance between the closest group and levels panel
						dist2 = Mathf.Abs (closestGroup.transform.position.x - rectTransform.transform.position.x);
					}
				}

				DisableCurrentPointer ();
				currentGroupIndex = closestGroup.transform.GetComponent<Group> ().Index;
				GoToCurrentGroup ();
			}
		}

		/// <summary>
		/// Enable the current pointer.
		/// </summary>
		public void EnableCurrentPointer ()
		{
			if (pointers == null) {
				return;
			}

			if (pointers.Length == 0) {
				return;
			}

			if (currentGroupIndex >= 0 && currentGroupIndex < pointers.Length) {
				Color tempColor = pointers [currentGroupIndex].GetComponent<Image> ().color;
				tempColor.a = 1;
				pointers [currentGroupIndex].GetComponent<Image> ().color = tempColor;
				pointers [currentGroupIndex].GetComponent<Image> ().sprite = pointerEnabled;
			}
		}

		/// <summary>
		/// Disable the current pointer.
		/// </summary>
		public void DisableCurrentPointer ()
		{
			if (pointers == null) {
				return;
			}

			if (pointers.Length == 0) {
				return;
			}

			if (pointers == null) {
				return;
			}

			if (currentGroupIndex >= 0 && currentGroupIndex < pointers.Length) {
				Color tempColor = pointers [currentGroupIndex].GetComponent<Image> ().color;
				tempColor.a = 0.3f;
				pointers [currentGroupIndex].GetComponent<Image> ().color = tempColor;
				pointers [currentGroupIndex].GetComponent<Image> ().sprite = pointerDisabled;
			}
		}

		/// <summary>
		/// Enable the next button.
		/// </summary>
		public void EnableNextButton ()
		{
			if (nextButton != null) {
				nextButton.GetComponent<Button> ().interactable = true;
			}
		}

		/// <summary>
		/// Disables the next button.
		/// </summary>
		public void DisableNextButton ()
		{
			if (nextButton != null) {
				nextButton.GetComponent<Button> ().interactable = false;
			}
		}

		/// <summary>
		/// Enable the next button.
		/// </summary>
		public void EnablePreviousButton ()
		{
			if (previousButton != null) {
				previousButton.GetComponent<Button> ().interactable = true;
			}
		}

		/// <summary>
		/// Disables the next button.
		/// </summary>
		public void DisablePreviousButton ()
		{
			if (previousButton != null) {
				previousButton.GetComponent<Button> ().interactable = false;
			}
		}

		/// <summary>
		/// Disable lerping to the lerping.
		/// </summary>
		public void DisableLerping ()
		{
			isLerping = false;
		}

		/// <summary>
		/// Enable the lerping to the groupd.
		/// </summary>
		public void EnableLerping ()
		{
			isLerping = true;
		}

		/// <summary>
		/// Disable the far groups.
		/// </summary>
		private void DisableFarGroups (int groupIndex)
		{
			if (groups == null) {
				return;
			}

			if (!(groupIndex >= 0 && groupIndex < groups.Length)) {
				return;
			}

			for (int i = 0; i < groups.Length; i++) {
				if (i == groupIndex - 1 || i == groupIndex || i == groupIndex + 1) {
					CommonUtil.EnableChildern(groups [i].transform);
				} else {
					CommonUtil.DisableChildern(groups [i].transform);
				}
			}
		}
	}
}