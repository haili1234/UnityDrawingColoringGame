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
	[DisallowMultipleComponent]
	public class ShapesCanvas : MonoBehaviour {

		public static ShapesCanvas instance;

		/// <summary>
		/// The shapes container.
		/// </summary>
		public Transform shapesContainer;

		/// <summary>
		/// The shape order.
		/// </summary>
		public static Text shapeOrder;

		// Use this for initialization
		void Awake () {
			if (instance == null) {
				instance = this;
				DontDestroyOnLoad (gameObject);

				SetShapeOrderReference();

				///Instantiate the shapes
				InstantiateShapes ();
			} else {
				//Set up the render camera of the Canvas
				Canvas canvas = instance.GetComponent<Canvas> ();
				if (canvas.worldCamera == null) {
					canvas.worldCamera = Camera.main;
				}

				SetShapeOrderReference();

				Destroy (gameObject);
			}
		}

		/// <summary>
		/// Set the shape order reference.
		/// </summary>
		private static void SetShapeOrderReference(){
			if(shapeOrder == null){
				shapeOrder = GameObject.Find("ShapeOrder").GetComponent<Text>();
			}
		}

		/// <summary>
		/// Instantiate the shapes.
		/// </summary>
		public void InstantiateShapes(){
			
			if (shapesContainer == null) {
				Debug.LogError("Shapes Container is undefined");
				return;
			}

			if (ShapesManager.instance.shapes.Count == 0) {
				Debug.LogWarning("No Shapes Found in the list");
			}

			///Destroy all shapes in the shapesContainer
			foreach (Transform child in shapesContainer) {
				Destroy(child.gameObject);
			}
			
			RectTransform rectTransform;
			
			for (int i = 0 ; i < ShapesManager.instance.shapes.Count ;i++){
				if(ShapesManager.instance.shapes[i] == null){
					continue;
				}
				GameObject shape = Instantiate (ShapesManager.instance.shapes[i].gamePrefab, Vector3.zero, Quaternion.identity) as GameObject;
				shape.name  = ShapesManager.instance.shapes[i].gamePrefab.name;//set the name of the shape
				if (shape.name == "FreeArea") {//Hide Free Area image
					shape.GetComponent<Image> ().enabled = false;
				}
				shape.transform.SetParent (shapesContainer);//set the parent of the shape
				rectTransform = shape.GetComponent<RectTransform>();//get RectTransform component
				//rectTransform.offsetMax = rectTransform.offsetMin = Vector2.zero;//reset offset
				rectTransform.anchoredPosition3D = Vector3.zero;//reset anchor position
				shape.transform.localScale = Vector3.one;//reset the scale to (1,1,1)
				shape.SetActive (false);//disable the shape
				ShapesManager.instance.shapes[i].gamePrefab = shape.gameObject;
			}
		}
	}
}
