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
	public class Tool : MonoBehaviour
	{
		/// <summary>
		/// The feature of the current tool.
		/// </summary>
		public ToolFeature feature = ToolFeature.Line;

		/// <summary>
		/// Whether to use the image of tool as cursor.
		/// </summary>
		public bool useAsCursor;

		/// <summary>
		/// Whether to use the tool as content.
		/// </summary>
		public bool useAsToolContent;

		/// <summary>
		/// Whether to create paint lines in the line.
		/// </summary>
		public bool createPaintLines;

		/// <summary>
		/// Whether to make the line edges rounded or not.
		/// </summary>
		public bool roundedEdges = true;

		/// <summary>
		/// Whether to enable the shadow of contents in the slider.
		/// </summary>
		public bool enableContentsShadow = true;

		/// <summary>
		/// Whether to repeat the texture of the tool's content as line.
		/// </summary>
		public bool repeatedTexture;

		/// <summary>
		/// The draw material.
		/// </summary>
		public Material drawMaterial;

		/// <summary>
		/// The line thickness factor.
		/// </summary>
		public float lineThicknessFactor = 1;

		/// <summary>
		/// The texture mode of the line.
		/// </summary>
		public LineTextureMode lineTextureMode = LineTextureMode.Stretch;

		/// <summary>
		/// A flag used only in the Tool Editor to Foldout the contents
		/// </summary>
		[HideInInspector]
		public bool showContents;

		/// <summary>
		/// The size of the slider contents cell.
		/// </summary>
		public Vector2 sliderContentsCellSize = new Vector2(90,115);

		/// <summary>
		/// The spacing between slider contents.
		/// </summary>
		public Vector2 sliderContentsSpacing = new Vector2(0,10);

		/// <summary>
		/// The index of the selected content.
		/// </summary>
		public int selectedContentIndex = 0;

		[Range(0,360)]
		/// <summary>
		/// The content rotation in the slider.
		/// </summary>
		public float contentRotation = 135;

		[Range(0,360)]
		/// <summary>
		/// The cursor rotation when a content of the tool is selected.
		/// </summary>
		public float cursorRotation = 50;

		/// <summary>
		/// The contents of the tool.
		/// </summary>
		public List<Transform> contents = new List<Transform>();

		/// <summary>
		/// The audio clip of the Tool.
		/// </summary>
		public AudioClip audioClip;

		void Awake(){
		}

		/// <summary>
		/// Enable tool selection.
		/// </summary>
		public void EnableSelection(){
			GetComponent<Animator>().SetBool("RunScale",true);
		}

		/// <summary>
		/// Disable tool selection.
		/// </summary>
		public void DisableSelection(){
			GetComponent<Animator>().SetBool("RunScale",false);
		}

		public enum ToolFeature{
			Line,
			Fill,
			Stamp,
			Hand,
		};
	}
}
