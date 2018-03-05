using UnityEngine;
using System.Collections;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Logic
{
	[DisallowMultipleComponent]
	public class PaintLine : MonoBehaviour
	{
		/// <summary>
		/// The line renderer.
		/// </summary>
		public LineRenderer lineRenderer;

		/// <summary>
		/// Whether the line is lerping.
		/// </summary>
		private bool isLerping;

		/// <summary>
		/// The start width of the line.
		/// </summary>
		private float startWidth;

		/// <summary>
		/// The end width of the line.
		/// </summary>
		private float endWidth;

		/// <summary>
		/// The target width .
		/// </summary>
		private float targetWidth;

		/// <summary>
		/// The width lerp speed.
		/// </summary>
		private float widthLerpSpeed;

		/// <summary>
		/// The move lerp speed.
		/// </summary>
		private float moveLerpSpeed;

		/// <summary>
		/// The first point of the line.
		/// </summary>
		private Vector3 firstPoint;

		/// <summary>
		/// The second point of the line.
		/// </summary>
		private Vector3 secondPoint;

		/// <summary>
		/// The target point for the line.
		/// </summary>
		private Vector3 targetPoint;

		void Awake(){
			if(lineRenderer == null)
				lineRenderer = GetComponent<LineRenderer> ();
		}
		
		// Update is called once per frame
		void Update ()
		{
			if (isLerping) {
				secondPoint.x = Mathf.Lerp (secondPoint.x, targetPoint.x, moveLerpSpeed * Time.smoothDeltaTime);
				secondPoint.y = Mathf.Lerp (secondPoint.y, targetPoint.y, moveLerpSpeed * Time.smoothDeltaTime);

				lineRenderer.SetPosition (1, secondPoint);
				
				endWidth = Mathf.Lerp (endWidth, targetWidth, widthLerpSpeed * Time.smoothDeltaTime);
				lineRenderer.SetWidth (startWidth, endWidth);
			}
		}

		/// <summary>
		/// Get the line renderer.
		/// </summary>
		/// <returns>The line renderer.</returns>
		public LineRenderer GetLineRenderer(){
			return this.lineRenderer;
		}

		/// <summary>
		/// Set the width details.
		/// </summary>
		/// <param name="startWidth">Start width.</param>
		/// <param name="targetWidth">Target width.</param>
		public void SetWidthDetails (float startWidth, float targetWidth)
		{
			this.startWidth = startWidth;
			this.endWidth = startWidth;
			this.targetWidth = targetWidth;
		}

		/// <summary>
		/// Set the points details.
		/// </summary>
		/// <param name="firstPoint">First point.</param>
		/// <param name="targetPoint">Target point.</param>
		public void SetPointsDetails (Vector3 firstPoint, Vector3 targetPoint)
		{
			this.firstPoint = firstPoint;
			this.secondPoint = firstPoint;
			this.targetPoint = targetPoint;
		}

		/// <summary>
		/// Set the move lerp speed.
		/// </summary>
		/// <param name="moveLerpSpeed">Move lerp speed.</param>
		public void SetMoveLerpSpeed(float moveLerpSpeed){
			this.moveLerpSpeed = moveLerpSpeed;
		}

		/// <summary>
		/// Set the width lerp speed.
		/// </summary>
		/// <param name="widthLerpSpeed">Width lerp speed.</param>
		public void SetWidthLerpSpeed(float widthLerpSpeed){
			this.widthLerpSpeed = widthLerpSpeed;
		}

		/// <summary>
		/// Begin or start the work (lerping).
		/// </summary>
		public void Begin ()
		{
			lineRenderer.SetVertexCount (2);
			lineRenderer.SetPosition (0, firstPoint);
			lineRenderer.SetPosition (1, secondPoint);
			lineRenderer.SetWidth (startWidth, endWidth);
			isLerping = true;
		}
	}
}
