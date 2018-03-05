using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using IndieStudio.DrawingAndColoring.Utility;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Logic
{
	public class Group : MonoBehaviour
	{
			public int Index;//the group's Index

			/// <summary>
			/// Create a group.
			/// </summary>
			/// <returns>The group.</returns>
			/// <param name="levelsGroupPrefab">Levels group prefab.</param>
			/// <param name="groupsParent">Groups parent.</param>
			/// <param name="groupIndex">Group index.</param>
			/// <param name="columnsPerGroup">Columns per group.</param>
			public static GameObject CreateGroup (GameObject levelsGroupPrefab, Transform groupsParent, int groupIndex, int columnsPerGroup)
			{
					//Create Levels Group
					GameObject levelsGroup = Instantiate (levelsGroupPrefab, Vector3.zero, Quaternion.identity) as GameObject;
					levelsGroup.transform.SetParent (groupsParent);
					levelsGroup.name = "Group-" + CommonUtil.IntToString(groupIndex + 1);
					levelsGroup.transform.localScale = Vector3.one;
					levelsGroup.GetComponent<RectTransform> ().offsetMax = Vector2.zero;
					levelsGroup.GetComponent<RectTransform> ().offsetMin = Vector2.zero;
					levelsGroup.GetComponent<Group> ().Index = groupIndex;
					levelsGroup.GetComponent<GridLayoutGroup> ().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
					levelsGroup.GetComponent<GridLayoutGroup> ().constraintCount = columnsPerGroup;
					return levelsGroup;
			}
	}
}
