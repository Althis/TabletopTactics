using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS.Inputs.SteamVR;

[RequireComponent(typeof(VRGrabbable))]
public class VRGrabMove : MonoBehaviour {

	VRGrabbable grabHandler;

	public bool rotate = true;

	void Start()
	{
		grabHandler = GetComponent<VRGrabbable> ();
	}
	void Update () {
		if (grabHandler.Grabbed) {
			transform.position = grabHandler.GrabRelativePos;
			transform.rotation = grabHandler.GrabRelativeRot;
		}
	}
}
