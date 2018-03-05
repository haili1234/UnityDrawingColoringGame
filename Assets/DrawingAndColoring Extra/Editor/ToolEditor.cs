using UnityEngine;
using UnityEditor;
using System.Collections;
using IndieStudio.DrawingAndColoring.Utility;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.DLEditor
{
	[CustomEditor(typeof(IndieStudio.DrawingAndColoring.Logic.Tool))]
	public class ToolEditor : Editor
	{
		public override void OnInspectorGUI ()
		{
			if (Application.isPlaying) {
				return;
			}

			IndieStudio.DrawingAndColoring.Logic.Tool tool = (IndieStudio.DrawingAndColoring.Logic.Tool)target;//get the target

			EditorGUILayout.Separator ();
			EditorGUILayout.HelpBox ("The tool GameObject must be breakable from Prefab instance", MessageType.Info);
			EditorGUILayout.Separator ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Review Drawing & Coloring Extra", GUILayout.Width (230), GUILayout.Height (22))) {
				Application.OpenURL (Links.packageURL);
			}

			GUI.backgroundColor = Colors.greenColor;         

			if (GUILayout.Button ("More Assets", GUILayout.Width (120), GUILayout.Height (22))) {
				Application.OpenURL (Links.indieStudioStoreURL);
			}
			GUI.backgroundColor = Colors.whiteColor;         

			GUILayout.EndHorizontal ();
			EditorGUILayout.Separator ();

			tool.selectedContentIndex = EditorGUILayout.IntField ("Selected Content's Index", tool.selectedContentIndex);
			EditorGUILayout.Separator ();
			tool.feature = (IndieStudio.DrawingAndColoring.Logic.Tool.ToolFeature)EditorGUILayout.EnumPopup ("Feature", tool.feature);

			if (tool.feature == IndieStudio.DrawingAndColoring.Logic.Tool.ToolFeature.Line) {
				if(!tool.repeatedTexture)
					tool.drawMaterial = EditorGUILayout.ObjectField ("Line Material", tool.drawMaterial, typeof(Material), true) as Material;
				tool.lineThicknessFactor = EditorGUILayout.Slider ("Line Thickness Factor", tool.lineThicknessFactor, 0.1f, 10);
				tool.lineTextureMode = (LineTextureMode)EditorGUILayout.EnumPopup ("Line Texture Mode", tool.lineTextureMode);
				tool.createPaintLines = EditorGUILayout.Toggle ("Create Paint Lines", tool.createPaintLines);
				tool.roundedEdges = EditorGUILayout.Toggle ("Rounded Edges", tool.roundedEdges);
			}

			tool.useAsToolContent = EditorGUILayout.Toggle ("Use As Content", tool.useAsToolContent);

			tool.useAsCursor = EditorGUILayout.Toggle ("Use As Cursor", tool.useAsCursor);

			tool.enableContentsShadow = EditorGUILayout.Toggle ("Enable Contents Shadow", tool.enableContentsShadow);

			tool.repeatedTexture = EditorGUILayout.Toggle ("Repeated Texture", tool.repeatedTexture);

			tool.sliderContentsCellSize = EditorGUILayout.Vector2Field ("Slider Contents Cell Size", tool.sliderContentsCellSize);

			tool.sliderContentsSpacing = EditorGUILayout.Vector2Field ("Slider Contents Spacing", tool.sliderContentsSpacing);

			tool.contentRotation = EditorGUILayout.Slider ("Content Rotation", tool.contentRotation, 0, 360);

			tool.cursorRotation = EditorGUILayout.Slider ("Cursor Rotation", tool.cursorRotation, 0, 360);

			if (tool.feature != IndieStudio.DrawingAndColoring.Logic.Tool.ToolFeature.Hand) {
			
				tool.showContents = EditorGUILayout.Foldout (tool.showContents, "Contents");

				if (tool.showContents) {
					GUILayout.BeginHorizontal ();
					GUI.backgroundColor = Colors.greenColor;         

					if (GUILayout.Button ("Add New Content", GUILayout.Width (110), GUILayout.Height (20))) {
						tool.contents.Add (null);
					}

					GUI.backgroundColor = Colors.redColor;         
					if (GUILayout.Button ("Remove Last Content", GUILayout.Width (150), GUILayout.Height (20))) {
						if (tool.contents.Count != 0) {
							tool.contents.RemoveAt (tool.contents.Count - 1);
						}
					}

					GUI.backgroundColor = Colors.whiteColor;
					GUILayout.EndHorizontal ();

					EditorGUILayout.Separator ();

					for (int i = 0; i <  tool.contents.Count; i++) {
						tool.contents [i] = EditorGUILayout.ObjectField ("Element " + i, tool.contents [i], typeof(Transform), true) as Transform;
					}
				}
			}

			//Audioclip effect for Fill and Stamp features
			if (tool.feature == IndieStudio.DrawingAndColoring.Logic.Tool.ToolFeature.Fill || tool.feature == IndieStudio.DrawingAndColoring.Logic.Tool.ToolFeature.Stamp) {
				tool.audioClip = EditorGUILayout.ObjectField ("Audio Clip", tool.audioClip, typeof(AudioClip), true) as AudioClip;
			}

			if (GUI.changed) {
				DirtyUtil.MarkSceneDirty ();
			}
		}
	}
}