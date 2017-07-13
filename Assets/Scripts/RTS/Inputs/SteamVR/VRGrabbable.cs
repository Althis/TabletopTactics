using System;
using UnityEngine;
using UnityEngine.Events;
using RTS.Util;

namespace RTS.Inputs.SteamVR
{
	public class VRGrabbable: MonoBehaviour
	{
		public Vector3 GrabRelativePos
		{
			get {
				return Grabbed ? grabTransform.TransformedPos : transform.position;
			}
		}

		public Quaternion GrabRelativeRot
		{
			get
			{
				return Grabbed ? grabTransform.TransformedRot : transform.rotation;
			}
		}


		public UnityEvent onGrab;
		public UnityEvent onRelease;

		public bool Grabbed { get; private set; }

		private VRGrabber grabber;
		private GrabTransform grabTransform; 


		public void OnGrabbedBy(VRGrabber grabber)
		{
			this.grabber = grabber;
			grabTransform = new GrabTransform (grabber.transform, transform);

			Grabbed = true;
			onGrab.Invoke ();
		}
		public void OnReleasedBy(VRGrabber grabber)
		{
			Grabbed = false;
			Debug.Assert (grabber == this.grabber);
			onRelease.Invoke ();
		}
	}
}

