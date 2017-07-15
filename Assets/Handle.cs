using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS.Inputs.SteamVR;

public class Handle : MonoBehaviour {
	public VRGrabbable tip;
	public Transform tipBase;

	public float maxAngle = 90;


	public float xPos{get; private set;}
	public float zPos{get; private set;}
	Vector3 tipLocalPos;
	Vector3 tipStartPos;

	// Use this for initialization
	void Start () {
		tipLocalPos = transform.TransformPoint (tip.transform.position);
		tipStartPos = tip.transform.position;
		tip.onRelease.AddListener (Reset);
	}
	
	// Update is called once per frame
	void Update () {
		if (tip.Grabbed) {
			Vector3 localGrabPos = transform.TransformPoint (tip.GrabRelativePos);

			Quaternion rot = Quaternion.FromToRotation (tipLocalPos, localGrabPos);



			float angle;
			Vector3 axis;
			axis.y = 0;
			rot.ToAngleAxis (out angle, out axis);
			angle = Mathf.Clamp (angle, -maxAngle, maxAngle);

			rot =  Quaternion.AngleAxis (angle, axis);


			Vector3 delta = tip.GrabRelativePos - tipStartPos;

			delta.y = 0;

			delta = delta.normalized;

			delta *= angle / maxAngle;

			xPos = delta.x;

			zPos = delta.z;

//			Vector3 euler = rot.eulerAngles;
//			euler.x = euler.x > 180 ? 360 - euler.x : euler.x;
//			euler.z = euler.z > 180 ? 360 - euler.z : euler.z;
//
//
//			xPos = euler.x / maxAngle;
//			zPos = euler.z / maxAngle;

			tipBase.localRotation = rot;
		}
	}

	void Reset()
	{
		xPos = 0;
		zPos = 0;
		tipBase.localRotation = Quaternion.identity;
	}

//	Quaternion ClampAngle(Quaternion rotation, bool removeYRot)
//	{
//		
//	}
}
