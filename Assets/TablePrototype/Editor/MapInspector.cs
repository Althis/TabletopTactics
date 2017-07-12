using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AI;

[CustomEditor(typeof(MapAssembler))]
public class MapInspector : Editor {

	public override void OnInspectorGUI ()
	{
		var mapAssembler = (MapAssembler)target;
		base.OnInspectorGUI ();
		if(GUILayout.Button("Generate Map"))
		{
			mapAssembler.GenerateMap ();
			if(EditorUtility.DisplayDialog("Created Map" ,"Rebuild NavMesh?", "Yes", "No"))
			{			
				NavMeshBuilder.BuildNavMesh ();
			}
//			EditorUtility.DisplayDialog(
		}
		if (GUILayout.Button ("Place On Table")) {
			mapAssembler.PlaceOnTable ();
		}
		if(GUILayout.Button("Clear")){
			mapAssembler.Clear();
		}
	}
}
