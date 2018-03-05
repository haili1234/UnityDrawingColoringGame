using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Logic
{
	#pragma warning disable 0168 // variable declared but not used.
	#pragma warning disable 0219 // variable assigned but not used.

	[DisallowMultipleComponent]
	public class ShapesTable : MonoBehaviour
	{
			/// <summary>
			/// Whether to create groups pointers or not.
			/// </summary>
			public bool createGroupsPointers = true;

			/// <summary>
			/// Whether to save the last selected group or not.
			/// </summary>
			public bool saveLastSelectedGroup = true;

			/// <summary>
			/// The shapes list.
			/// </summary>
			public static List<TableShape> shapes;

			/// <summary>
			/// The groups parent.
			/// </summary>
			public Transform groupsParent;

			/// <summary>
			/// The pointers parent.
			/// </summary>
			public Transform pointersParent;

			/// <summary>
			/// The shape bright.
			/// </summary>
			public Transform shapeBright;

			/// <summary>
			/// The shape prefab.
			/// </summary>
			public GameObject shapePrefab;
		
			/// <summary>
			/// The shapes group prefab.
			/// </summary>
			public GameObject shapesGroupPrefab;

			/// <summary>
			/// The pointer prefab.
			/// </summary>
			public GameObject pointerPrefab;

			/// <summary>
			/// temporary transform.
			/// </summary>
			private Transform tempTransform;

			/// <summary>
			/// The Number of shapes per group.
			/// </summary>
			[Range (1, 100)]
			public int shapesPerGroup = 12;

			/// <summary>
			/// Number of columns per group.
			/// </summary>
			[Range (1, 10)]
			public int columnsPerGroup = 3;

			/// <summary>
			/// The content scale ratio.
			/// </summary>
			[Range (0f, 10)]
			public float contentScaleRatio = 0.5f;

			/// <summary>
			/// Whether to enable group grid layout.
			/// </summary>
			public bool EnableGroupGridLayout = true;

			/// <summary>
			/// The last shape that user reached.
			/// </summary>
			private Transform lastShape;

			void Start ()
			{
					//define the shapes list
					shapes = new List<TableShape> ();

					//Create new shapes
					CreateShapes ();
		
					//Setup the last selected group/shape index
					ScrollSlider scrollSlider = GameObject.FindObjectOfType<ScrollSlider> ();
					if (saveLastSelectedGroup && ShapesManager.instance!=null) {
						scrollSlider.currentGroupIndex = ShapesManager.instance.lastSelectedShape;
					}
			}

			void Update ()
			{
					if (lastShape != null) {
							//Set the bright postion to the last shape postion
							if (!Mathf.Approximately (lastShape.position.magnitude, shapeBright.position.magnitude)) {
									shapeBright.position = lastShape.position;
							}
					}
			}


			/// <summary>
			/// Creates the shapes in Groups.
			/// </summary>
			private void CreateShapes ()
			{
					if (ShapesManager.instance == null) {
						return;
					}
					//Clear current shapes list
					shapes.Clear ();

					//The ID of the shape
					int ID = 0;
				
					GameObject shapesGroup = null;
				
					float contentScale = (Screen.width *1.0f/Screen.height) * contentScaleRatio;//content scale

					//Create Shapes inside groups
					for (int i = 0; i < ShapesManager.instance.shapes.Count; i++) {

							if (i % shapesPerGroup == 0) {
									int groupIndex = (i / shapesPerGroup);
									shapesGroup = Group.CreateGroup (shapesGroupPrefab, groupsParent, groupIndex, columnsPerGroup);
									if(!EnableGroupGridLayout){
										shapesGroup.GetComponent<GridLayoutGroup>().enabled = false;
									}
					  				 if (createGroupsPointers) {
											Pointer.CreatePointer (groupIndex, shapesGroup, pointerPrefab, pointersParent);
									}
							}

							ID = (i + 1);//the ID of the shape
							//Create a Shape
							GameObject tableShapeGameObject = Instantiate (shapePrefab, Vector3.zero, Quaternion.identity) as GameObject;
							tableShapeGameObject.transform.SetParent (shapesGroup.transform);//setting up the shape's parent
							TableShape tableShapeComponent = tableShapeGameObject.GetComponent<TableShape> ();//get TableShape Component
							tableShapeComponent.ID = ID;//setting up shape ID
							tableShapeGameObject.name = "Shape-" + ID;//shape name
							tableShapeGameObject.transform.localScale = Vector3.one;
							tableShapeGameObject.GetComponent<RectTransform> ().offsetMax = Vector2.zero;
							tableShapeGameObject.GetComponent<RectTransform> ().offsetMin = Vector2.zero;

							GameObject content = Instantiate(ShapesManager.instance.shapes[i].gamePrefab,Vector3.zero,Quaternion.identity) as GameObject;

							//release unwanted resources
							Destroy(content.GetComponent<EventTrigger>());
							Destroy(content.GetComponent<UIEvents>());
							if(content.transform.Find ("Parts")!=null)
								Destroy (content.transform.Find ("Parts").gameObject);

							content.transform.SetParent(tableShapeGameObject.transform.Find("Content"));

							RectTransform rectTransform = tableShapeGameObject.transform.Find("Content").GetComponent<RectTransform>();
			
							//set up the scale
							content.transform.localScale = new Vector3(contentScale  ,contentScale);
							//set up the anchor
							content.GetComponent<RectTransform> ().anchorMin = Vector2.zero;
							content.GetComponent<RectTransform> ().anchorMax = Vector2.one;
							//set up the offset
							content.GetComponent<RectTransform> ().offsetMax = Vector2.zero;
							content.GetComponent<RectTransform> ().offsetMin = Vector2.zero;
							//enable image component
							content.GetComponent<Image> ().enabled = true;
							//add click listener
							tableShapeGameObject.GetComponent<Button> ().onClick.AddListener (() => GameObject.FindObjectOfType<UIEvents> ().AlbumShapeEvent (tableShapeGameObject.GetComponent<TableShape> ()));
							content.gameObject.SetActive (true);
							shapes.Add (tableShapeComponent);//add table shape component to the list
					}

					if (ShapesManager.instance.shapes.Count == 0) {
							Debug.Log ("There are no Shapes found");
					} else {
							Debug.Log ("New shapes have been created");
					}
			}

			/// <summary>
			/// Raise the change group/shape event.
			/// </summary>
			/// <param name="currentGroup">Current group/shape.</param>
			public void OnChangeGroup (int currentGroup)
			{
					if (saveLastSelectedGroup) {
						ShapesManager.instance.lastSelectedShape = currentGroup;
					}
			}
	}
}