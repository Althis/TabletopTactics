using System;
using UnityEngine;

public class MapAssembler: MonoBehaviour
{
	[Serializable]
	public class MapAssembly
	{
		public GameObject cellPrefab;
		public GameObject borderPrefab;
		public GameObject viewAreaPrefab;

		public int sizeX = 10;
		public int sizeZ = 6;
		public int borderEndZ = 3;
		public int borderEndX = 6;

		public float offsetX = 1;
		public float offsetZ = 1;
	}


	public Transform transformToMatch;



	public MapAssembly assemblySettings;


	private Transform CellsOrigin;
	private Transform BordersOrigin;


	public void PlaceOnTable()
	{
		transform.position = transformToMatch.position;
	}

	public void GenerateMap()
	{
		Clear ();
		CreateCellsOrigin ();
		var origin = CellsOrigin;
			
		for (int i = 1; i < assemblySettings.sizeZ; i++) {
			var row = new GameObject ("Row");
			row.transform.parent = origin;

			row.transform.localPosition = rowOffset (i);

			for (int j = 1; j < assemblySettings.sizeX; j++) {
				var cell = GameObject.Instantiate (assemblySettings.cellPrefab);
				cell.transform.parent = row.transform;
				cell.transform.localPosition = collumnOffset (j);
			}
		}

		CreateBordersOrigin ();
		origin = BordersOrigin;
		for (int i = 0; i < assemblySettings.sizeZ; i++) {
			var row = new GameObject ("Row");
			row.transform.parent = origin;

			row.transform.localPosition = rowOffset (i);

			for (int j = 0; j < assemblySettings.sizeX; j++) {
				if (!isOnBorder(i, j))
					continue;
				var cell = GameObject.Instantiate (assemblySettings.borderPrefab);
				cell.transform.parent = row.transform;
				cell.transform.localPosition = collumnOffset (j);
			}
		}


		GameObject viewArea = GameObject.Instantiate (assemblySettings.viewAreaPrefab);
		viewArea.name = "View Area";
		viewArea.transform.parent = transform;
		viewArea.transform.localPosition = rowOffset (1) + collumnOffset (1);
		var scaleVec = new Vector3 (assemblySettings.borderEndX - 1, 
			               1, assemblySettings.borderEndZ - 1);
		viewArea.transform.localScale = Vector3.Scale( viewArea.transform.localScale,  scaleVec);
	}

	bool isOnBorder(int i, int j)
	{
		var bEndx = assemblySettings.borderEndX;
		var bEndZ = assemblySettings.borderEndZ;
		return (((i == 0 || i == bEndZ) && j <= bEndx)
			|| ((j == 0 || j == bEndx) && i <= bEndZ));
			
	}

	void CreateCellsOrigin()
	{
		CellsOrigin = new GameObject ("Cells Origin").transform;
		CellsOrigin.transform.parent = transform;
		CellsOrigin.transform.localPosition = Vector3.zero;
	}
	void CreateBordersOrigin()
	{
		BordersOrigin = new GameObject ("Borders Origin").transform;
		BordersOrigin.transform.parent = transform;
		BordersOrigin.transform.localPosition = Vector3.zero;
	}

	public void Clear()
	{
		TryDestroyChild ("Cells Origin");
		TryDestroyChild ("Borders Origin");
		TryDestroyChild ("View Area");
	}

	void TryDestroyChild(string name)
	{
		var transf = transform.Find (name);
		if(transf != null)
			GameObject.DestroyImmediate (transf.gameObject);
	}

	Vector3 rowOffset(int i)
	{
		return i * Vector3.forward * assemblySettings.offsetZ;
	}

	Vector3 collumnOffset(int j)
	{
		return j * Vector3.right * assemblySettings.offsetX;
	}
}


