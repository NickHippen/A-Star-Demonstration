using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathfindingUnit))]
public class PathfindingUnitEditor : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		PathfindingUnit unit = (PathfindingUnit)target;
		if (GUILayout.Button("Find Path")) {
			unit.FindPath();
		}
		if (GUILayout.Button("Follow Path")) {
			unit.FollowPath();
		}
	}

}
