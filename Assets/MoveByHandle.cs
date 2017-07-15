using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveByHandle : MonoBehaviour {
	public Handle handle;
	public float xSpeed = 10;
	public float zSpeed = 10;

	void Update () {
//		Debug.LogFormat ("X: {0}, Y:{1}", handle.xPos, handle.zPos);
		var delta = new Vector3(handle.xPos * xSpeed, 0, handle.zPos * zSpeed);
		delta *= Time.deltaTime;
		transform.Translate(delta);
	}
}
