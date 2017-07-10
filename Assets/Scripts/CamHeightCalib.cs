using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamHeightCalib : MonoBehaviour {

	Transform trans;

	// Use this for initialization
	void Start () {
		trans = GetComponent<Transform> ();
	}
	
	// Update is called once per frame
	void Update () {
		trans.position = new Vector3(trans.position.x, trans.position.y + Input.GetAxisRaw ("Adjust") / 100 + Input.GetAxisRaw ("Narrow Adjust") / 1000, trans.position.z);
	}
}
