using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS.Inputs.SteamVR;

public class Handle : MonoBehaviour {
	public VRGrabbable tip;
	public Transform tipBase;

	Vector3 tipLocalPos;

	// Use this for initialization
	void Start () {
		tipLocalPos = transform.TransformPoint (tip.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		if (tip.Grabbed) {
			Vector3 localGrabPos = transform.TransformPoint (tip.GrabRelativePos);

			Quaternion rot = Quaternion.FromToRotation (tipLocalPos, localGrabPos);

			tipBase.localRotation = rot;
		}
	}
}
