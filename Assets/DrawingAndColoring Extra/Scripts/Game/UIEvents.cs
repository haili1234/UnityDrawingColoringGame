using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using IndieStudio.DrawingAndColoring.Utility;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Logic
{
	/// <summary>
	/// User interface events for (buttons,sliders,...etc).
	/// </summary>
	[DisallowMultipleComponent]
	public class UIEvents : MonoBehaviour
	{
		public void ResetZoom(){
			CameraZoom.ResetZoom ();
		}

		public void PointerButtonEvent(Pointer pointer){
			if (pointer == null) {
				return;
			}
			if (pointer.group != null) {
				ScrollSlider scrollSlider = GameObject.FindObjectOfType (typeof(ScrollSlider)) as ScrollSlider;
				if (scrollSlider != null) {
					scrollSlider.DisableCurrentPointer ();
					FindObjectOfType<ScrollSlider> ().currentGroupIndex = pointer.group.Index;
					scrollSlider.GoToCurrentGroup ();
				}
			}
		}

		public void PrintClickEvent(){
			GameObject.FindObjectOfType<WebPrint> ().PrintScreen ();
		}

		public void UndoClickEvent ()
		{
			History history = GameObject.FindObjectOfType<History> ();
			if (history != null) {
				history.UnDo ();
			}
		}

		public void RedoClickEvent ()
		{
			History history = GameObject.FindObjectOfType<History> ();
			if (history != null) {
				GameManager.interactable = false;
				history.Redo ();
			}
		}

		public void AlbumShapeEvent (TableShape tableShape)
		{
			if (tableShape == null) {
				return;
			}

			TableShape.selectedShape = tableShape;
			LoadGameScene ();
		}

		public void ThicknessSizeEvent (ThicknessSize thicknessSize)
		{
			if (thicknessSize == null) {
				return;
			}

			GameManager gameManager = GameObject.FindObjectOfType<GameManager> ();

			if (gameManager.currentTool == null) {
				return;
			}

			if (!(gameManager.currentTool.feature == Tool.ToolFeature.Line)) {
				return;
			}

			gameManager.currentThickness = thicknessSize;
			gameManager.ChangeThicknessSizeColor ();
		}

		public void ShowTrashConfirmDialog ()
		{
			AdsManager.instance.ShowAdvertisment (AdPackage.AdEvent.Event.ON_SHOW_TRASH_DIALOG);
			DisableGameManager ();
			GameObject.Find ("TrashConfirmDialog").GetComponent<ConfirmDialog> ().Show ();
		}

		public void TrashConfirmDialogEvent (GameObject value)
		{
			if (value == null) {
				return;
			}
			
			if (value.name.Equals ("YesButton")) {
				Debug.Log ("Trash Confirm Dialog : Yes button clicked");
				GameObject.FindObjectOfType<GameManager> ().CleanCurrentShapeScreen ();

			} else if (value.name.Equals ("NoButton")) {
				Debug.Log ("Trash Confirm Dialog : No button clicked");
			}
			value.GetComponentInParent<ConfirmDialog> ().Hide ();
			EnableGameManager ();
			AdsManager.instance.ShowAdvertisment (AdPackage.AdEvent.Event.ON_LOAD_GAME_SCENE);
		}

		public void ToolClickEvent (Tool tool)
		{
			if (tool == null) {
				return;
			}
			
			GameManager gameManager = GameObject.FindObjectOfType<GameManager> ();
			
			if (tool.useAsToolContent) {//like an eraser
				gameManager.currentToolContent = tool.GetComponent<ToolContent> ();
			}
			
			if (tool.useAsCursor) {
				//Set the tool as cursor
				gameManager.currentCursorSprite = tool.GetComponent<Image> ().sprite;
			}
			
			gameManager.currentTool.DisableSelection ();

			tool.EnableSelection ();
			gameManager.HideToolContents (gameManager.currentTool);
			gameManager.currentTool = tool;
			gameManager.LoadCurrentToolContents ();

			if (tool.contents.Count != 0) {
				//Select current content of the tool
				if(tool.selectedContentIndex >=0 && tool.selectedContentIndex < tool.contents.Count)
					gameManager.SelectToolContent (tool.contents [tool.selectedContentIndex].GetComponent<ToolContent>());
			}

			if (tool.feature == Tool.ToolFeature.Hand) {
				CameraDrag.isRunning = true;
			} else {
				CameraDrag.isRunning = false;
			}
		}

		public void ToolContentClickEvent (ToolContent content)
		{
			if (content == null) {
				return;
			}

			GameManager gameManager = GameObject.FindObjectOfType<GameManager> ();
			gameManager.SelectToolContent (content);
		}

		public void NextButtonClickEvent ()
		{
			GameManager gameManager = GameObject.FindObjectOfType<GameManager> ();
			gameManager.NextShape ();
		}

		public void PreviousButtonClickEvent ()
		{
			GameManager gameManager = GameObject.FindObjectOfType<GameManager> ();
			gameManager.PreviousShape ();
		}

		public void OnPointerEnterDrawArea(){
			GameManager.pointerInDrawArea = true;
		}

		public void OnPointerExitDrawArea(){
			GameManager.pointerInDrawArea = false;
		}

		public void DisableGameManager ()
		{
			if (!GameManager.clickDownOnDrawArea) {
				GameManager.interactable = false;
			}
		}
		
		public void EnableGameManager ()
		{
			GameManager.interactable = true;
		}

		public void OnDrawAreaClickDown ()
		{
			GameManager.clickDownOnDrawArea = true;
		}

		public void OnDrawAreaClickUp ()
		{
			GameManager.clickDownOnDrawArea = false;
		}

		public void ChangeCursorToArrow ()
		{
			GameManager gameManager = GameObject.FindObjectOfType<GameManager> ();
			if (gameManager != null)
				gameManager.ChangeCursorToArrow ();
		}

		public void ChangeCursorToCurrentSprite ()
		{
			GameManager gameManager = GameObject.FindObjectOfType<GameManager> ();
			if (gameManager != null)
				gameManager.ChangeCursorToCurrentSprite ();
		}

		public void LoadAlbumScene ()
		{
			StartCoroutine (LoadSceneAsync("Album"));
		}

		public void LoadGameScene ()
		{
			StartCoroutine (LoadSceneAsync("Game"));
		}

		IEnumerator LoadSceneAsync (string sceneName)
		{
			if (!string.IsNullOrEmpty (sceneName)) {
				#if UNITY_PRO_LICENSE
				AsyncOperation async = Application.LoadLevelAsync (sceneName);
				while (!async.isDone) {
					yield return 0;
				}
				#else
				Application.LoadLevel (sceneName);
				yield return 0;
				#endif
			}
		}
	}
}
