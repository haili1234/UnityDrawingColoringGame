using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Logic
{
	public class History : MonoBehaviour
	{
		/// <summary>
		/// The pointer of the pool.
		/// </summary>
		private int pointer = -1;

		/// <summary>
		/// The pool (list of elements).
		/// </summary>
		private List<Element> pool = new List<Element> ();

		/// <summary>
		/// The undo button reference.
		/// </summary>
		public Button undoBtn;

		/// <summary>
		/// The redo button reference.
		/// </summary>
		public Button redoBtn;

		/// <summary>
		/// Undo Process
		/// </summary>
		public void UnDo ()
		{
			if (pointer < 0) {
				return;//skip
			}

			if (pool [pointer].type == Element.EType.Color) {
				Element element = getPreviousElementOf(pool [pointer],pointer - 1);

				if(element == null){
					pool [pointer].transform.GetComponent<ShapePart>().SetColor(Color.white);
					SetSortingOrder(pool [pointer].transform, 	pool [pointer].transform.GetComponent<ShapePart>().initialSortingOrder);
				}else{
					pool [pointer].transform.GetComponent<ShapePart>().SetColor(element.color);
					SetSortingOrder(pool [pointer].transform, element.sortingOrder);
				}
			} else if (pool [pointer].type == Element.EType.Object) {

				if(pool [pointer].transform.gameObject.activeSelf){
					pool [pointer].transform.gameObject.SetActive (false);
				}
				SetSortingOrder(pool [pointer].transform, pool [pointer].sortingOrder);
			}

			pointer--;
			CheckUnDoRedoButtonsStatus ();
		}

		/// <summary>
		/// Redo Process
		/// </summary>
		public void Redo ()
		{
			if (pointer+1 >= pool.Count) {
				return;//skip
			}

			pointer++;

			if (pool [pointer].type == Element.EType.Color) {
				pool [pointer].transform.GetComponent<ShapePart>().SetColor(pool [pointer].color);
				SetSortingOrder(pool [pointer].transform, pool [pointer].sortingOrder);
			} else if (pool [pointer].type == Element.EType.Object) {
				pool [pointer].transform.gameObject.SetActive (!pool [pointer].transform.gameObject.activeSelf);
				SetSortingOrder(pool [pointer].transform, pool [pointer].sortingOrder);
			}

			CheckUnDoRedoButtonsStatus ();
		}

		/// <summary>
		/// Add new Element to the pool.
		/// </summary>
		/// <param name="element">Element.</param>
		public void AddToPool (Element element)
		{
			if (transform == null) {
				return;
			}

			for(int i = pool.Count-1 ; i >= pointer +1 ;i--){
				if(pool[i].type == Element.EType.Object){
					Destroy(pool[i].transform.gameObject);
				}
				pool.RemoveAt(i);
			}

			pointer = pool.Count;
			pool.Add (element);
			CheckUnDoRedoButtonsStatus ();
		}
		
		/// <summary>
		/// Remove the last element.
		/// </summary>
		public void RemoveLastElement(){
			if (pool == null) {
				return;
			}

			if (pool.Count == 0) {
				return;
			}

			pool.RemoveAt (pool.Count - 1);
			pointer--;

			CheckUnDoRedoButtonsStatus ();
		}

		/// <summary>
		/// Clean the pool.
		/// </summary>
		public void CleanPool ()
		{
			if (pool != null) {
				pool.Clear ();
			}
			pointer = -1;
			CheckUnDoRedoButtonsStatus ();
		}

		/// <summary>
		/// Get the previous element in the pool.
		/// </summary>
		/// <returns>The previous element.</returns>
		/// <param name="element">Current Element.</param>
		/// <param name="index">Pool Search Index.</param>
		private Element getPreviousElementOf(Element element,int index){
			if (!(index >= 0 && index < pool.Count)) {
				return null;
			}

			for(int i = index;i >=0; i--){
				if(pool[i].type == element.type && pool[i].transform.GetInstanceID() == element.transform.GetInstanceID()){
					return pool[i];
				}
			}

			return null;
		}

		/// <summary>
		/// Returns the size of the pool.
		/// </summary>
		/// <returns>The pool size.</returns>
		public int GetPoolSize(){
			return pool.Count;
		}

		/// <summary>
		/// Returns the pointer of the pool.
		/// </summary>
		/// <returns>The pointer.</returns>
		public int GetPointer ()
		{
			return this.pointer;
		}

		/// <summary>
		/// Check the undo/redo buttons status.
		/// </summary>
		public void CheckUnDoRedoButtonsStatus(){

			//Setting up references
			if (undoBtn == null) {
				undoBtn = GameObject.Find("UndoButton").GetComponent<Button>();
			}
			
			if (redoBtn == null) {
				redoBtn = GameObject.Find("RedoButton").GetComponent<Button>();
			}

			if (undoBtn == null || redoBtn == null) {
				Debug.LogError("Undefined Undo/Redo references");
				return;
			}

			if (GetPoolSize () == 0) {
				DisableButton(undoBtn);
				DisableButton(redoBtn);
			} else {
				if(pointer == GetPoolSize() - 1){
					EnableButton(undoBtn);
					DisableButton(redoBtn);
				}
				else if(pointer == -1){
					DisableButton(undoBtn);
					EnableButton(redoBtn);
				}else{
					EnableButton(undoBtn);
					EnableButton(redoBtn);
				}
			}
		}

		/// <summary>
		/// Gets the last element.
		/// </summary>
		/// <returns>The last element.</returns>
		public Element GetLastElement(){

			if (pool == null) {
				return null;
			}

			if (pool.Count == 0) {
				return null;
			}
			return pool [pool.Count - 1];
		}

		/// <summary>
		/// Enable the given button.
		/// </summary>
		/// <param name="btn">Button.</param>
		private void EnableButton(Button btn){
			if (btn == null) {
				return;
			}

			btn.interactable = true;
		}

		/// <summary>
		/// Disable the given button.
		/// </summary>
		/// <param name="btn">Button.</param>
		private void DisableButton(Button btn){
			if (btn == null) {
				return;
			}

			btn.interactable = false;
		}

		/// <summary>
		/// Set the sorting order.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="sortingOrder">Sorting order.</param>
		private void SetSortingOrder(Transform target,int sortingOrder){
			if (target == null) {
				return;
			}

			if (target.GetComponent<SpriteRenderer> () != null) {
				target.GetComponent<SpriteRenderer> ().sortingOrder = sortingOrder;
			} else if (target.GetComponent<LineRenderer> () != null) {
				target.GetComponent<LineRenderer> ().sortingOrder = sortingOrder;
			}
		}

		public class Element
		{
			public EType type;
			public Transform transform;
			public Color color;
			public int sortingOrder;

			public enum EType
			{
				Object,
				Color
			}
		}
	}
}