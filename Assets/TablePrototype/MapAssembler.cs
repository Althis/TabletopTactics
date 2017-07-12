using System;
using UnityEngine;

public class MapAssembler: MonoBehaviour
{
	[Serializable]
	public class MapAssembly
	{
		public GameObject cellPrefab;

		public int sizeX = 10;
		public int sizeZ = 6;

		public float offsetX = 1;
		public float offsetZ = 1;
	}


	public Transform tableMapOrigin;



	public MapAssembly assemblySettings;


	private Transform CellsOrigin;


	public void PlaceOnTable()
	{
		transform.position = tableMapOrigin.position;
	}

	public void GenerateMap()
	{
		Clear ();
		CreateCellsOrigin ();
		var origin = CellsOrigin;
			
		for (int i = 0; i < assemblySettings.sizeZ; i++) {
			var row = new GameObject ("Row");
			row.transform.parent = origin;

			row.transform.localPosition = rowOffset (i);

			for (int j = 0; j < assemblySettings.sizeX; j++) {
				var cell = GameObject.Instantiate (assemblySettings.cellPrefab);
				cell.transform.parent = row.transform;
				cell.transform.localPosition = collumnOffset (j);
			}
		}
	}

	void CreateCellsOrigin()
	{
		CellsOrigin = new GameObject ("Cells Origin").transform;
		CellsOrigin.transform.parent = transform;
		CellsOrigin.transform.localPosition = Vector3.zero;
	}

	public void Clear()
	{
		if (CellsOrigin != null)
			GameObject.DestroyImmediate (CellsOrigin.gameObject);
	}

	Vector3 rowOffset(int i)
	{
		return i * Vector3.back * assemblySettings.offsetZ;
	}

	Vector3 collumnOffset(int j)
	{
		return j * Vector3.right * assemblySettings.offsetX;
	}
}


