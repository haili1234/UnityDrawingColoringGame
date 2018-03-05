using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using IndieStudio.DrawingAndColoring.Utility;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Logic
{
	[DisallowMultipleComponent]
	public class GameManager : MonoBehaviour
	{
		/// <summary>
		/// The lines counter.
		/// </summary>
		private int linesCount;

		/// <summary>
		/// The cursor gameobject reference.
		/// </summary>
		public GameObject cursor;

		/// <summary>
		/// The line prefab reference.
		/// </summary>
		public GameObject linePrefab;

		/// <summary>
		/// The stamp prefab reference.
		/// </summary>
		public GameObject stampPrefab;

		/// <summary>
		/// The current line.
		/// </summary>
		private Line currentLine;

		/// <summary>
		/// The drawing area.
		/// </summary>
		public Transform drawingArea;

		/// <summary>
		/// The tool contents parent.
		/// </summary>
		public Transform toolContentsParent;

		/// <summary>
		/// Whether the GameManager is interactable (listening to user input).
		/// </summary>
		public static bool interactable;

		/// <summary>
		/// Whether the pointer in draw area.
		/// </summary>
		public static bool pointerInDrawArea;

		/// <summary>
		/// Whether the user clicked on the Area located in DrawCanvas.
		/// </summary>
		public static bool clickDownOnDrawArea;

		/// <summary>
		/// The default size of the cursor.
		/// </summary>
		private Vector3 cursorDefaultSize;

		/// <summary>
		/// The click size of the cursor .
		/// </summary>
		private Vector3 cursorClickSize;

		/// <summary>
		/// The arrow sprite.
		/// </summary>
		public Sprite arrowSprite;

		/// <summary>
		/// The current cursor sprite.
		/// </summary>
		[HideInInspector]
		public Sprite currentCursorSprite;

		/// <summary>
		/// The current tool.
		/// </summary>
		public Tool currentTool;

		/// <summary>
		/// The tools in the scene.
		/// </summary>
		[HideInInspector]
		public Tool [] tools ;

		/// <summary>
		/// The content of the current tool (the selected content).
		/// </summary>
		[HideInInspector]
		public ToolContent currentToolContent;

		/// <summary>
		/// The current thickness.
		/// </summary>
		public ThicknessSize currentThickness;

		/// <summary>
		/// The thickness size images reference.
		/// </summary>
		public Image [] thicknessSizeImages;

		/// <summary>
		/// The user interface events(contains the events of the UI elements like buttons,...etc).
		/// </summary>
		public static UIEvents uiEvents;
		
		/// <summary>
		/// The camera that renders the drawings.
		/// </summary>
		public Camera drawCamera;

		/// <summary>
		/// The cursor zoom output.
		/// </summary>
		public RawImage CursorZoomOutput;

		void Awake(){

			uiEvents = GameObject.FindObjectOfType<UIEvents> ();
			InstantiateDrawingContents ();
		}

		// Use this for initialization
		void Start ()
		{
			///Setting up the references
			tools = GameObject.FindObjectsOfType<Tool>() as Tool [];
			cursorDefaultSize = cursor.transform.localScale;
			cursorClickSize = cursorDefaultSize / 1.2f;
			interactable = false;
			clickDownOnDrawArea = false;
			linesCount = 0;

			///Change cursor sprite into arrow
			ChangeCursorToArrow ();

			if (currentTool != null) {
				///If the current tool is defined,set the appropriate sprites for the tools
				currentTool.EnableSelection();
				foreach(Tool tool in tools){
					if(tool.contents.Count!=0 && currentTool.selectedContentIndex >=0 && currentTool.selectedContentIndex < tool.contents.Count && !tool.useAsCursor)
						tool.GetComponent<Image>().sprite = tool.contents[currentTool.selectedContentIndex].GetComponent<Image>().sprite;
				}
			}

			///Instantiate Tools Contents
			InstantiateToolsContents ();

			///Load the contents of the current tool
			LoadCurrentToolContents();

			///Load the current shape
			LoadCurrentShape ();

			//Show the shape's order
			ShapesCanvas.shapeOrder.gameObject.SetActive (true);

			CursorZoomOutput.enabled = false;
		}

		// Update is called once per frame
		void Update ()
		{
			DrawCursor (GetCurrentPlatformClickPosition(Camera.main));

			if (!interactable || WebPrint.isRunning) { //Add || !pointerInDrawArea to limit drawing area
				return;
			}

			if (currentTool == null) {
				return;
			}

			HandleInput ();

			if (currentTool.feature == Tool.ToolFeature.Line) {
				UseLineFeature ();
			} else if (currentTool.feature == Tool.ToolFeature.Stamp) {
				UseStampFeature ();
			} else if (currentTool.feature == Tool.ToolFeature.Fill) {
				UseFillFeature ();
			} else if (currentTool.feature == Tool.ToolFeature.Hand) {
				//Do Nothing
			}
		}

		void OnDestroy() {

			//Hide the shape's order
			if(ShapesCanvas.shapeOrder!=null)
				ShapesCanvas.shapeOrder.gameObject.SetActive (false);

			//Hide the Shapes Canvas contents
			foreach (ShapesManager.Shape shape in ShapesManager.instance.shapes) {
				if(shape !=null)
					if(shape.gamePrefab!=null)
					shape.gamePrefab.SetActive(false);
			}

			//Hide the drawing contents
			foreach (DrawingContents dc in Area.shapesDrawingContents) {
				if(dc !=null)
					dc.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Handle the user input.
		/// </summary>
		private void HandleInput(){

			if (!Application.isMobilePlatform) {//current platform is not mobile
				if (Input.GetKeyDown (KeyCode.RightArrow)) {
					NextShape ();
				} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
					PreviousShape ();
				}
			}

			if (Input.GetMouseButtonUp (0)) {
				interactable = false;
			}
		}

		/// <summary>
		/// Get the current platform click position.
		/// </summary>
		/// <returns>The current platform click position.</returns>
		private Vector3 GetCurrentPlatformClickPosition(Camera camera){
			Vector3 clickPosition = Vector3.zero;
			
			if (Application.isMobilePlatform) {//current platform is mobile
				if(Input.touchCount!=0){
					Touch touch = Input.GetTouch(0);
					clickPosition = touch.position;
				}
			}else{//others
				clickPosition = Input.mousePosition;
			}
			
			clickPosition = camera.ScreenToWorldPoint(clickPosition);//get click position in the world space
			clickPosition.z = 0;
			return clickPosition;
		}

		/// <summary>
		/// Uses the line feature.
		/// </summary>
		private void UseLineFeature(){

			if (Application.isMobilePlatform) {//Mobile Platform
				if(Input.touchCount == 1){//Single Touch
					Touch touch = Input.GetTouch(0);
					if (touch.phase == TouchPhase.Began) {
						LineFeatureOnClickBegan();
					} else if (touch.phase == TouchPhase.Ended) {
						LineFeatureOnClickReleased();
					}
				}else{
					currentLine = null;
				}
			} else {//Others
				if (Input.GetMouseButtonDown (0)) {
					LineFeatureOnClickBegan();
				} else if (Input.GetMouseButtonUp (0)) {
					LineFeatureOnClickReleased();
				}
			}

			if (currentLine != null) {
				//Add touch/click point into current line
				currentLine.AddPoint (GetCurrentPlatformClickPosition(drawCamera));
			}
		}

		/// <summary>
		/// Line feature on click began.
		/// </summary>
		private void LineFeatureOnClickBegan(){

			//Enable Cursor Zoom Output
			CursorZoomOutput.enabled = true;

			SetCursorClickSize();
			
			//Create new line gameobject
			GameObject line = Instantiate (linePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			
			//Set the parent of line
			line.transform.SetParent(Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].transform);
			
			//Set the name of the line
			line.name = "Line";
			
			//Get the Line component
			currentLine = line.GetComponent<Line> ();
			
			//Increase linesCount by 1
			linesCount++;

			//Increase current sorting order
			Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].currentSortingOrder++;
			if(Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].currentSortingOrder <= Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].lastPartSortingOrder){
				Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].currentSortingOrder = Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].lastPartSortingOrder+1;
			}

			//Add the element to history
			History.Element element = new History.Element ();
			element.transform = line.transform;
			element.type = History.Element.EType.Object;
			element.sortingOrder = Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].currentSortingOrder;
			Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].GetComponent<History> ().AddToPool(element);

			//Set the sorting order of the line (last line must be on the top)
			currentLine.SetSortingOrder(Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].currentSortingOrder);
			
			if (currentTool.repeatedTexture) {
				//Set the material of the line
				currentLine.SetMaterial (new Material (Shader.Find ("Sprites/Default")));
				currentLine.material.mainTexture = currentToolContent.sprite.texture;
				currentLine.lineRenderer.numCapVertices = 0;
			} else {
				currentLine.SetMaterial (currentTool.drawMaterial);
			}

			//Set whether to create paint lines
			currentLine.createPaintLines = currentTool.createPaintLines;
			
			//Set the color of the line, if apply color flag in on
			if(currentToolContent!=null && currentToolContent.applyColor)
				currentLine.SetColor (currentToolContent.gradientColor);
			
			if(currentThickness !=null){
				currentLine.SetWidth(currentThickness.value * currentTool.lineThicknessFactor,currentThickness.value * currentTool.lineThicknessFactor);
			}

			//Set the line texture mode
			currentLine.lineRenderer.textureMode = currentTool.lineTextureMode;
		}

		/// <summary>
		/// Line feature on click released.
		/// </summary>
		private void LineFeatureOnClickReleased(){
			
			if (currentLine != null) {

				if (currentLine.GetPointsCount () == 0) {//Zero Points
					//Destroy the line
					Destroy (currentLine.gameObject);
				}else if (currentLine.GetPointsCount () == 1 || currentLine.GetPointsCount () == 2) {//One or Two Points
					if (!currentTool.roundedEdges) {
						//Destroy the line
						Area.shapesDrawingContents [ShapesManager.instance.lastSelectedShape].currentSortingOrder--;
						Area.shapesDrawingContents [ShapesManager.instance.lastSelectedShape].GetComponent<History> ().RemoveLastElement ();
						Destroy (currentLine.gameObject);
					} else {
						//Make the line as dot
						currentLine.lineRenderer.SetVertexCount(2);
						currentLine.lineRenderer.SetPosition (0, currentLine.points [0]);
						currentLine.lineRenderer.SetPosition (1, currentLine.points [0] - new Vector3 (0.015f, 0.015f, 0));
					}
				} 

				//Destroy Line component
				Destroy(currentLine);
				//if(currentTool.repeatedTexture)
					//currentLine.RunMaterialAnimation();
			}

			SetCursorDefaultSize();

			//Release current line
			currentLine = null;

			//Disable Cursor Zoom Output
			CursorZoomOutput.enabled = false;
		}

		/// <summary>
		/// Uses the stamp feature.
		/// </summary>
		private void UseStampFeature(){

			if (Application.isMobilePlatform) {//Mobile Platform
				if(Input.touchCount!=0){
					Touch touch = Input.GetTouch(0);
					if (touch.phase == TouchPhase.Began) {
						StampFeatureOnClickBegan();
					} else if (touch.phase == TouchPhase.Ended) {
						StampFeatureOnClickReleased();
					}
				}
			} else {//Others
				if (Input.GetMouseButtonDown (0)) {
					StampFeatureOnClickBegan();
				} else if (Input.GetMouseButtonUp (0)) {
					StampFeatureOnClickReleased();
				}
			}
		}

		/// <summary>
		/// Stamp feature on click began.
		/// </summary>
		private void StampFeatureOnClickBegan(){
			SetCursorClickSize();

			GameObject stamp  = Instantiate(stampPrefab,GetCurrentPlatformClickPosition (drawCamera),Quaternion.identity) as GameObject;
			
			//Set the name of stamp
			stamp.name = "Stamp";
			
			//Set the parent of stamp
			stamp.transform.SetParent(Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].transform);
			
			//Set the rotation of the stamp
			stamp.transform.rotation = Quaternion.Euler(new Vector3(0,0,Random.Range(-15,15)));

			Vector3 tempPos = stamp.GetComponent<RectTransform> ().anchoredPosition3D;
			tempPos.z = 0;

			///Set rect transform anchored postion 3d
			stamp.GetComponent<RectTransform> ().anchoredPosition3D = tempPos;

			//Increase current sorting order
			Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].currentSortingOrder++;
			if(Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].currentSortingOrder <= Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].lastPartSortingOrder){
				Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].currentSortingOrder = Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].lastPartSortingOrder+1;
			}

			//Add the element to history
			History.Element element = new History.Element ();
			element.transform = stamp.transform;
			element.type = History.Element.EType.Object;
			element.sortingOrder = Area.shapesDrawingContents [ShapesManager.instance.lastSelectedShape].currentSortingOrder;
			Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].GetComponent<History> ().AddToPool(element);

			//Get sprite renderer component
			SpriteRenderer sr = stamp.GetComponent<SpriteRenderer>();

			if (sr != null) {
				if(currentTool.audioClip!=null)
					CommonUtil.PlayOneShotClipAt(currentTool.audioClip,Vector3.zero,1);//play Tool audio clip

				//Set the sprite of the stamp
				sr.sprite = currentToolContent.GetComponent<Image> ().sprite;
			
				//Set the first gradient color key for the samp , if apply color flag in on
				if (currentToolContent.applyColor) {
					sr.color = currentToolContent.gradientColor.colorKeys[0].color;
				}

				//Set the sorting order for the stamp sprite renderer
				sr.sortingOrder = Area.shapesDrawingContents [ShapesManager.instance.lastSelectedShape].currentSortingOrder;
			}
		}

		/// <summary>
		/// Stamp feature on click released.
		/// </summary>
		private void StampFeatureOnClickReleased(){
			SetCursorDefaultSize();
		}

		/// <summary>
		/// Use the fill feature.
		/// </summary>
		private void UseFillFeature(){

			if (Application.isMobilePlatform) {//Mobile Platform
				if(Input.touchCount!=0){
					Touch touch = Input.GetTouch(0);
					if (touch.phase == TouchPhase.Began) {
						FillFeatureOnClickBegan();
					} else if (touch.phase == TouchPhase.Ended) {
						FillFeatureOnClickReleased();
					}
				}
			} else {//Others
				if (Input.GetMouseButtonDown (0)) {
					FillFeatureOnClickBegan();
				} else if (Input.GetMouseButtonUp (0)) {
					FillFeatureOnClickReleased();
				}
			}
		}

		/// <summary>
		/// fill feature on click began.
		/// </summary>
		private void FillFeatureOnClickBegan(){
			SetCursorClickSize ();
			
			RaycastHit2D hit2d = Physics2D.Raycast (GetCurrentPlatformClickPosition(drawCamera), Vector2.zero);
			if (hit2d.collider != null) {
				ShapePart shapePart = hit2d.collider.GetComponent<ShapePart>();//get the ShapePart component
				if(shapePart!=null){//Shape Part
					SpriteRenderer spriteRenderer = hit2d.collider.GetComponent<SpriteRenderer>();
					if (spriteRenderer != null) {
						if(currentTool.audioClip!=null)
							CommonUtil.PlayOneShotClipAt(currentTool.audioClip,Vector3.zero,1);//play Tool audio clip

						History.Element lastElement = Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].GetComponent<History> ().GetLastElement();

						bool equalsLastElement = false;
						if(lastElement!=null){
							equalsLastElement = lastElement.transform.GetInstanceID() == shapePart.transform.GetInstanceID();
						}

						if(shapePart.targetColor !=currentToolContent.gradientColor.colorKeys[0].color || !equalsLastElement){

							///Apply and save the attributes (Color,sortingOrder,lastPartSortingOrder)
							shapePart.SetColor(currentToolContent.gradientColor.colorKeys[0].color);
							Area.shapesDrawingContents [ShapesManager.instance.lastSelectedShape].shapePartsColors [hit2d.collider.name] = currentToolContent.gradientColor;
							spriteRenderer.sortingOrder = Area.shapesDrawingContents [ShapesManager.instance.lastSelectedShape].currentSortingOrder + 1;
							Area.shapesDrawingContents [ShapesManager.instance.lastSelectedShape].shapePartsSortingOrder [hit2d.collider.name] = spriteRenderer.sortingOrder;
							Area.shapesDrawingContents [ShapesManager.instance.lastSelectedShape].lastPartSortingOrder = spriteRenderer.sortingOrder;

							///Add the element to history
							History.Element element = new History.Element ();
							element.transform = hit2d.collider.transform;
							element.type = History.Element.EType.Color;
							element.color = currentToolContent.gradientColor.colorKeys[0].color;
							element.sortingOrder = spriteRenderer.sortingOrder;
							Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].GetComponent<History> ().AddToPool(element);
						}
					}
				}
			}
		}

		/// <summary>
		/// fill feature on click released.
		/// </summary>
		private void FillFeatureOnClickReleased(){
			SetCursorDefaultSize();
		}

		/// <summary>
		/// Draw the cursor.
		/// </summary>
		/// <param name="clickPosition">Click position.</param>
		private void DrawCursor (Vector3 clickPosition)
		{
			if (cursor == null) {
				return;
			}

			cursor.transform.position = clickPosition;
		}

		/// <summary>
		/// Set the size of the cursor to default size.
		/// </summary>
		public void SetCursorDefaultSize(){
			cursor.transform.localScale = cursorDefaultSize;
		}

		/// <summary>
		/// Set the size of the cursor to click size.
		/// </summary>
		public void SetCursorClickSize(){
			cursor.transform.localScale = cursorClickSize;
		}

		/// <summary>
		/// Set the cursor sprite.
		/// </summary>
		/// <param name="sprite">Sprite.</param>
		public void SetCursorSprite(Sprite sprite){
			cursor.GetComponent<SpriteRenderer> ().sprite = sprite;
		}

		/// <summary>
		/// Change the cursor into arrow.
		/// </summary>
		public void ChangeCursorToArrow(){
			cursor.GetComponent<SpriteRenderer> ().sprite = arrowSprite;
			cursor.transform.Find ("Shadow").GetComponent<SpriteRenderer> ().sprite = arrowSprite;

			Quaternion roation = cursor.transform.rotation;
			Vector3 eulerAngle = roation.eulerAngles;
			eulerAngle.z = 300;
			roation.eulerAngles = eulerAngle;
			cursor.transform.localRotation = roation;
		}

		/// <summary>
		/// Change the cursor to current sprite.
		/// </summary>
		public void ChangeCursorToCurrentSprite(){
			if (currentTool == null) {
				return;
			}

			cursor.GetComponent<SpriteRenderer> ().sprite = currentCursorSprite;
			cursor.transform.Find ("Shadow").GetComponent<SpriteRenderer> ().sprite = currentCursorSprite;
			cursor.transform.localRotation = Quaternion.Euler(new Vector3(0,0,currentTool.cursorRotation));
		}

		/// <summary>
		/// Change the color of the thickness size (circles).
		/// </summary>
		public void ChangeThicknessSizeColor(){
			if (currentToolContent == null) {
				Debug.Log("currentToolContent is undefined");
				return;
			}

			if (thicknessSizeImages != null) {
				Color thicknessSizeColor = currentToolContent.gradientColor.colorKeys[0].color;
				thicknessSizeColor.a = 1;

				foreach(Image img in thicknessSizeImages){
					img.color = thicknessSizeColor;
					if(img.gameObject.GetInstanceID() == currentThickness.gameObject.GetInstanceID()){
						currentThickness.EnableSelection();
					}
				}
			}
		}


		/// <summary>
		/// Instantiate the drawing contents for each Shape.
		/// </summary>
		private void InstantiateDrawingContents(){

			if (Area.shapesDrawingContents.Count == 0 && ShapesManager.instance.shapes.Count!= 0) {
				foreach (ShapesManager.Shape s in ShapesManager.instance.shapes) {
					if(s == null){
						continue;
					}
					GameObject drawingContents = new GameObject (s.gamePrefab.name + " Contents");
					drawingContents.layer = LayerMask.NameToLayer("MiddleCamera");
					DrawingContents drawingContentsComponent = drawingContents.AddComponent (typeof(DrawingContents))as DrawingContents;
					drawingContents.AddComponent (typeof(History));
					drawingContents.transform.SetParent (drawingArea);
					drawingContents.transform.position = Vector3.zero;
					drawingContents.AddComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
					drawingContents.transform.localScale = Vector3.one;
					drawingContents.SetActive (false);

					Transform shapeParts = s.gamePrefab.transform.Find ("Parts");
					if (shapeParts != null) {
						foreach(Transform part in shapeParts){
							if(part.GetComponent<ShapePart>()!=null && part.GetComponent<SpriteRenderer>()!=null){
								drawingContentsComponent.shapePartsColors.Add(part.name,part.GetComponent<SpriteRenderer>().color);
								drawingContentsComponent.shapePartsSortingOrder.Add(part.name,part.GetComponent<SpriteRenderer>().sortingOrder);
							}
						}
					}

					Area.shapesDrawingContents.Add (drawingContentsComponent);
				}
			}
		}
		
		/// <summary>
		/// Instantiate all tools contents in ToolsSlider
		/// </summary>
		public void InstantiateToolsContents(){

			if (toolContentsParent == null) {
				Debug.Log("toolContentsParent is undefined");
				return;
			}

			///Destroy all contents in the slider
			foreach (Transform child in toolContentsParent) {
				Destroy(child.gameObject);
			}

			Vector3 contentRotation;

			foreach (Tool tool in tools) {
				 contentRotation = new Vector3 (0, 0, tool.contentRotation);
				
				for(int i = 0 ;i <tool.contents.Count ;i++) {
					if (tool.contents[i] == null) {
						continue;
					}
					
					if (tool.contents[i].GetComponent<ToolContent> () == null) {
						continue;
					}
					
					GameObject c = Instantiate (tool.contents[i].gameObject, Vector3.zero, Quaternion.identity) as GameObject;
					c.name = tool.contents[i].name;
					c.transform.SetParent (toolContentsParent);
					c.transform.localScale = Vector3.one;
					c.transform.rotation = Quaternion.Euler (contentRotation);
					
					//Add onClick event
					Button btn = c.GetComponent<Button> ();
					if (btn != null)
						btn.onClick.AddListener (() => uiEvents.ToolContentClickEvent (c.GetComponent<ToolContent> ()));

					c.SetActive(false);

					if(currentTool !=null){
						if (currentTool.enableContentsShadow) {
							if(c.GetComponent<Shadow> ()!=null)
								c.GetComponent<Shadow> ().enabled = true;
						} else {
							if(c.GetComponent<Shadow> ()!=null)
								c.GetComponent<Shadow> ().enabled = false;
						}

						///Only show the contents of the current Tool
						if(currentTool.GetInstanceID() == tool.GetInstanceID()){
							c.SetActive(true);
						}
					}
					tool.contents[i] = c.transform;
				}
			}
		}

		/// <summary>
		/// Load the contents of the current tool.
		/// </summary>
		public void LoadCurrentToolContents(){

			if (currentTool == null) {
				Debug.Log("Current tool is undefined");
				return;
			}

			if (toolContentsParent == null) {
				return;
			}

			GridLayoutGroup toolContentsGL = toolContentsParent.GetComponent<GridLayoutGroup> ();
			toolContentsGL.cellSize = currentTool.sliderContentsCellSize;
			toolContentsGL.spacing = currentTool.sliderContentsSpacing;

			///Show the contents
			for(int i = 0 ; i < currentTool.contents.Count ;i++) {
				if(currentTool.contents[i] == null){
					continue;
				}

				if(currentTool.contents[i].GetComponent<ToolContent>() == null){
					continue;
				}

				currentTool.contents[i].gameObject.SetActive(true);

				ToolContent toolContent = currentTool.contents[i].GetComponent<ToolContent>();

				if (currentTool.enableContentsShadow) {
					if(	currentTool.contents[i].GetComponent<Shadow> ()!=null)
						currentTool.contents[i].GetComponent<Shadow> ().enabled = true;
				} else {
					if(currentTool.contents[i].GetComponent<Shadow> ()!=null)
						currentTool.contents[i].GetComponent<Shadow> ().enabled = false;
				}

				if(currentTool.selectedContentIndex == i){
					toolContent.EnableSelection();
					if(!currentTool.useAsCursor)
						currentCursorSprite = currentTool.contents[i].GetComponent<Image>().sprite;
					currentToolContent = toolContent;
				}

			}

			ChangeThicknessSizeColor ();
		}

		/// <summary>
		/// Select the content of the current tool.
		/// </summary>
		/// <param name="content">Content.</param>
		public void SelectToolContent(ToolContent content){

			if (content == null) {
				return;
			}

			currentToolContent.DisableSelection ();

			currentToolContent = content;
			if (!currentTool.useAsCursor)
				currentCursorSprite = content.GetComponent<Image> ().sprite;

			for (int i = 0; i < currentTool.contents.Count; i++) {
				if (currentTool.contents [i] == null) {
					continue;
				}
				
				if (currentTool.contents [i].name == content.transform.name) {
					currentTool.selectedContentIndex = i;

					foreach (Tool tool in tools) {
						if (tool.contents.Count != 0 && !tool.useAsCursor && (i >= 0 && i < tool.contents.Count)) {
							if(tool.contents [i]!=null)
							tool.GetComponent<Image> ().sprite = tool.contents [i].GetComponent<Image> ().sprite;
						}
					}
					break;
				}
			}

			SetShapeOrderColor ();
			ChangeThicknessSizeColor ();
			content.EnableSelection ();
		}


		/// <summary>
		/// Set the shape order.
		/// </summary>
		public void SetShapeOrder(){

			if (ShapesManager.instance.shapes == null || ShapesCanvas.shapeOrder == null) {
				return;
			}

			ShapesCanvas.shapeOrder.text = (ShapesManager.instance.lastSelectedShape + 1)+"/"+ShapesManager.instance.shapes.Count;
		}

		/// <summary>
		/// Set the color of the shape order.
		/// </summary>
		public void SetShapeOrderColor(){

			if (ShapesCanvas.shapeOrder == null ) {
				return;
			}

			if (currentToolContent != null) {
				ShapesCanvas.shapeOrder.color = currentToolContent.gradientColor.colorKeys[0].color;
			}
		}

		/// <summary>
		/// Load the current shape.
		/// </summary>
		public void LoadCurrentShape(){

			if (ShapesManager.instance.shapes == null) {
				return;
			}

			if (!(ShapesManager.instance.lastSelectedShape >= 0 && ShapesManager.instance.lastSelectedShape < ShapesManager.instance.shapes.Count)) {
				return;
			}

			SetShapeOrder ();
			SetShapeOrderColor ();
			ShapesManager.instance.shapes [ShapesManager.instance.lastSelectedShape].gamePrefab.SetActive(true);
			Area.shapesDrawingContents [ShapesManager.instance.lastSelectedShape].gameObject.SetActive (true);
			Area.shapesDrawingContents [ShapesManager.instance.lastSelectedShape].GetComponent<History> ().CheckUnDoRedoButtonsStatus ();
		}

		/// <summary>
		/// Go to the Next shape.
		/// </summary>
		public void NextShape(){

			if (ShapesManager.instance.shapes == null) {
				return;
			}

			ShapesManager.instance.shapes [ShapesManager.instance.lastSelectedShape].gamePrefab.SetActive(false);
			Area.shapesDrawingContents [ShapesManager.instance.lastSelectedShape].gameObject.SetActive (false);

			if (ShapesManager.instance.lastSelectedShape + 1 >=ShapesManager.instance.shapes.Count) {
				ShapesManager.instance.lastSelectedShape = 0;
			} else {
				ShapesManager.instance.lastSelectedShape += 1;
			}

			LoadCurrentShape ();
		}

		/// <summary>
		/// Go to the Previous shape.
		/// </summary>
		public void PreviousShape(){

			if (ShapesManager.instance.shapes == null) {
				return;
			}

			ShapesManager.instance.shapes [ShapesManager.instance.lastSelectedShape].gamePrefab.SetActive(false);
			Area.shapesDrawingContents [ShapesManager.instance.lastSelectedShape].gameObject.SetActive (false);

			if (ShapesManager.instance.lastSelectedShape - 1 < 0) {
				ShapesManager.instance.lastSelectedShape = ShapesManager.instance.shapes.Count - 1;
			} else {
				ShapesManager.instance.lastSelectedShape -= 1;
			}

			LoadCurrentShape ();
		}

		/// <summary>
		/// Hide the tool contents.
		/// </summary>
		/// <param name="tool">Tool.</param>
		public void HideToolContents(Tool tool){
			if (tool == null) {
				return;
			}

			foreach (Transform content in tool.contents) {
				if(content!=null)
					content.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Show the tool contents.
		/// </summary>
		/// <param name="tool">Tool.</param>
		public void ShowToolContents(Tool tool){
			if (tool == null) {
				return;
			}
			
			foreach (Transform content in tool.contents) {
				if(content!=null)
					content.gameObject.SetActive(true);
			}
		}

		/// <summary>
		/// Clean the screen.
		/// </summary>
		public void CleanCurrentShapeScreen(){

			if (Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape] == null) {
				return;
			}

			//Clean the history for the current shape
			Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].GetComponent<History> ().CleanPool ();

			//Remove all the childern in drawContents
			foreach (Transform child in Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].transform) {
				Destroy(child.gameObject);
			}

			Transform shapeParts =ShapesManager.instance.shapes [ShapesManager.instance.lastSelectedShape].gamePrefab.transform.Find ("Parts");
			if (shapeParts != null) {
				foreach(Transform part in shapeParts){
					part.GetComponent<SpriteRenderer>().color = Color.white;
					Area.shapesDrawingContents [ShapesManager.instance.lastSelectedShape].shapePartsColors[part.name] = Color.white;
					part.GetComponent<ShapePart>().ApplyInitialSortingOrder();
					part.GetComponent<ShapePart>().ApplyInitialColor();
					Area.shapesDrawingContents [ShapesManager.instance.lastSelectedShape].shapePartsSortingOrder[part.name] = part.GetComponent<ShapePart>().initialSortingOrder;
				}
			}

			linesCount = 0;
			Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].currentSortingOrder = 0;
			Area.shapesDrawingContents[ShapesManager.instance.lastSelectedShape].lastPartSortingOrder = 0;
		}


		/// <summary>
		/// Clean the shapes.
		/// </summary>
		public void CleanShapes(){

			for (int i = 0; i < ShapesManager.instance.shapes.Count; i++) {
				//Clean the history for the current shape
				Area.shapesDrawingContents [i].GetComponent<History> ().CleanPool ();

				//Remove all the childern in drawContents
				foreach (Transform child in Area.shapesDrawingContents[i].transform) {
					Destroy (child.gameObject);
				}

				Transform shapeParts = ShapesManager.instance.shapes [i].gamePrefab.transform.Find ("Parts");
				if (shapeParts != null) {
					foreach (Transform part in shapeParts) {
						part.GetComponent<SpriteRenderer> ().color = Color.white;
						Area.shapesDrawingContents [i].shapePartsColors [part.name] = Color.white;
						part.GetComponent<ShapePart> ().ApplyInitialSortingOrder ();
						part.GetComponent<ShapePart> ().ApplyInitialColor ();
						Area.shapesDrawingContents [i].shapePartsSortingOrder [part.name] = part.GetComponent<ShapePart> ().initialSortingOrder;
					}
				}

				linesCount = 0;
				Area.shapesDrawingContents [i].currentSortingOrder = 0;
				Area.shapesDrawingContents [i].lastPartSortingOrder = 0;
			}
		}
	}
}