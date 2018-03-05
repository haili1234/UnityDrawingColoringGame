using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Logic
{
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	public class Line : MonoBehaviour
	{
		/// <summary>
		/// The material of the line.
		/// </summary>
		public Material material;

		/// <summary>
		/// The color gradient of the line.
		/// </summary>
		public Gradient color;

		/// <summary>
		/// The line renderer reference.
		/// </summary>
		public LineRenderer lineRenderer;

		/// <summary>
		/// The paint line prefab.
		/// </summary>
		public GameObject paintLinePrefab;

		/// <summary>
		/// The start width of the line.
		/// </summary>
		[Range(0,1)]
		public float startWidth = 0.2f;

		/// <summary>
		/// The end width of the line.
		/// </summary>
		[Range(0,1)]
		public float endWidth = 0.2f;

		/// <summary>
		/// The minimum offset between points.
		/// </summary>
		[Range(0,1)]
		public float offsetBetweenPoints = 0.05f;

		/// <summary>
		/// The point Z position.
		/// </summary>
		[Range(-20,20)]
		public float pointZPosition;

		/// <summary>
		/// The sorting order of line
		/// </summary>
		public int sortingOrder;

		/// <summary>
		/// Whether to use bezier path.
		/// </summary>
		public bool useBezier = true;

		/// <summary>
		/// Whether to create paint lines.
		/// </summary>
		public bool createPaintLines;

		/// <summary>
		/// The points of the line.
		/// </summary>
		[HideInInspector]
		public List<Vector3> points = new List<Vector3>();

		/// <summary>
		/// The previous time value.
		/// </summary>
		private float previousTime;

		/// <summary>
		/// The game manager reference.
		/// </summary>
		private static GameManager gameManager;

		/// <summary>
		/// The material animation speed.
		/// </summary>
		///private float materialAnimationSpeed = 2;

		/// <summary>
		/// Whether to run material animation speed.
		/// </summary>
		///private bool runMaterialAnimation;

		/// <summary>
		/// A temp offset vector2.
		/// </summary>
		///private Vector2 tempOffset;

		//Drawing points list of the bezier path
		private static List<Vector3> bezierDrawingPoints;

		// Use this for initialization
		void Awake ()
		{
			if(gameManager == null)
				gameManager = GameObject.FindObjectOfType<GameManager>(); 
			
			previousTime = Time.time;
			Init ();
		}

		void Update(){
			/*
			if (runMaterialAnimation) {
				tempOffset = material.mainTextureOffset;
				tempOffset.x += Time.deltaTime * materialAnimationSpeed;
				material.mainTextureOffset = tempOffset;
			}*/
		}

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		public void Init ()
		{
			if (material == null) {
				material = new Material (Shader.Find ("Sprites/Default"));
			}

			points = new List<Vector3> ();
			lineRenderer = GetComponent<LineRenderer> ();
			GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
			if (lineRenderer != null) {
				lineRenderer.material = material;
				lineRenderer.SetWidth(startWidth,endWidth);
			}
		}

		/// <summary>
		/// Add the point to the line.
		/// </summary>
		/// <param name="point">Point.</param>
		public void AddPoint (Vector3 point)
		{
			if (lineRenderer != null) {
				point.z = pointZPosition;
				if (points.Count > 1) {
					if (Vector3.Distance (point, points [points.Count - 1]) < offsetBetweenPoints) {
						return;//skip the point
					}
				}

				points.Add(point);
				if(createPaintLines){
					if(Time.time - previousTime >=0.15f){///control line paints number per time by changing 0.15f
						previousTime = Time.time;
					   CreatePaintLine(point);
					}
				}
				lineRenderer.SetVertexCount(points.Count);
				lineRenderer.SetPosition (points.Count - 1, point);
				if(useBezier){
					BezierInterpolate();
				}
			}
		}

		/// <summary>
		/// Set the material.
		/// </summary>
		/// <param name="material">Material.</param>
		public void SetMaterial(Material material){
			this.material = material;
			if (lineRenderer != null) {
				lineRenderer.material = this.material;
			}
		}

		/// <summary>
		/// Sets the width.
		/// </summary>
		/// <param name="startWidth">Start width.</param>
		/// <param name="endWidth">End width.</param>
		public void SetWidth(float startWidth,float endWidth){
			if (lineRenderer != null) {
				lineRenderer.SetWidth (startWidth, endWidth);
			}
			this.startWidth = startWidth;
			this.endWidth = endWidth;

		}

		/// <summary>
		/// Set the color.
		/// </summary>
		/// <param name="value">Value.</param>
		public void SetColor(Gradient color){
				
			if (lineRenderer != null) {
				lineRenderer.colorGradient = color;
			}

			this.color = color;
		}

		/// <summary>
		/// Set the sorting order.
		/// </summary>
		/// <param name="sortingOrder">Sorting order.</param>
		public void SetSortingOrder(int sortingOrder){

			if (lineRenderer != null) {
				lineRenderer.sortingOrder = sortingOrder;
			}
			this.sortingOrder = sortingOrder;
		}

		/// <summary>
		/// Bezier interpolate to smooth the line's curve.
		/// </summary>
		public void BezierInterpolate()
		{
			BezierPath bezierPath = new BezierPath ();
			bezierPath.Interpolate(points, 0.3f);
			bezierDrawingPoints = bezierPath.GetDrawingPoints2();
			lineRenderer.SetVertexCount(bezierDrawingPoints.Count);
			lineRenderer.SetPositions (bezierDrawingPoints.ToArray ());
		}

		/// <summary>
		/// Create paint line.
		/// </summary>
		/// <param name="position">Position.</param>
		private void CreatePaintLine(Vector3 position){

			if (paintLinePrefab == null) {
				return;
			}

			GameObject paintLine = Instantiate (paintLinePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			paintLine.name = "PaintLine";
			paintLine.transform.SetParent (transform);
			paintLine.GetComponent<RectTransform> ().anchoredPosition3D = Vector3.zero;
			PaintLine pl = paintLine.GetComponent<PaintLine> ();
			if(pl == null){
				return;
			}

			pl.lineRenderer.material = material;
			pl.lineRenderer.colorGradient = color;
			pl.lineRenderer.sortingOrder = sortingOrder;

			if (!gameManager.currentTool.roundedEdges) {
				pl.lineRenderer.numCapVertices = 0;
			} 

			Vector3 targetPosition = position;
			targetPosition.y -= Random.Range(0.2f,1.3f);
			pl.SetWidthDetails (Random.Range(startWidth * 0.2f,startWidth * 0.7f),Random.Range(startWidth * 0.1f,startWidth * 0.4f));

			float moveWidthSpeed = Random.Range (0.1f, 6);
			pl.SetMoveLerpSpeed (moveWidthSpeed);
			pl.SetWidthLerpSpeed (moveWidthSpeed);

			pl.SetPointsDetails (position, targetPosition);
			pl.Begin ();
		}

		/// <summary>
		/// Get the points count.
		/// </summary>
		/// <returns>The points count.</returns>
		public int GetPointsCount(){
			return this.points.Count;
		}

		public void RunMaterialAnimation(){
			//runMaterialAnimation = true;
		}

	}
}