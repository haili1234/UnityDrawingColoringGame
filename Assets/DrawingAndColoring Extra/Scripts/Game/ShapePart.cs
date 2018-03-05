using UnityEngine;
using System.Collections;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Logic
{
	[DisallowMultipleComponent]
	public class ShapePart : MonoBehaviour {

		/// <summary>
		/// The initial sorting order.
		/// </summary>
		[HideInInspector]
		public int initialSortingOrder;

		/// <summary>
		/// The sprite renderer reference.
		/// </summary>
		private SpriteRenderer spriteRenderer;

		/// <summary>
		/// The color lerp speed.
		/// </summary>
		private static float colorLerpSpeed = 7;

		/// <summary>
		/// The target color
		/// </summary>
		[HideInInspector]
		public Color targetColor = Color.white;

		// Use this for initialization
		void Start () {

			if (spriteRenderer == null) {
				spriteRenderer = GetComponent<SpriteRenderer>();
			}

			//Set up the initial sorting order
			initialSortingOrder = GetComponent<SpriteRenderer> ().sortingOrder;

			//Apply the previous color on part
			object previousColor = Area.shapesDrawingContents [ShapesManager.instance.lastSelectedShape].shapePartsColors [name];
			if(previousColor!=null)
				spriteRenderer.color = (Color)previousColor;

			targetColor = (Color)previousColor;

			//Apply the previous sorting order on part
			object previousSortingOrder = Area.shapesDrawingContents [ShapesManager.instance.lastSelectedShape].shapePartsSortingOrder [name];
			if(previousSortingOrder!=null)
				spriteRenderer.sortingOrder = (int)previousSortingOrder;
		}

		void Update(){
			LerpToColor ();
		}

		/// <summary>
		/// Lerp the target color.
		/// </summary>
		public void LerpToColor(){
			if (spriteRenderer == null) {
				return;
			}

			if (targetColor == spriteRenderer.color) {
				return;
			}
			spriteRenderer.color = Color.Lerp (spriteRenderer.color, targetColor, colorLerpSpeed * Time.smoothDeltaTime);
		}

		/// <summary>
		/// Apply the initial sorting order.
		/// </summary>
		public void ApplyInitialSortingOrder(){
			GetComponent<SpriteRenderer> ().sortingOrder = initialSortingOrder;
		}

		/// <summary>
		/// Set the color of the part.
		/// </summary>
		/// <param name="targetColor">Target color.</param>
		public void SetColor(Color targetColor){
			this.targetColor = targetColor;
		}

		/// <summary>
		/// Apply the initial color.
		/// </summary>
		public void ApplyInitialColor(){
			this.targetColor = Color.white;
		}
	}
}
