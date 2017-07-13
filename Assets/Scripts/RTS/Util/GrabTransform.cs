using System;
using UnityEngine;

namespace RTS.Util
{
	public class GrabTransform
	{
		Vector3 localPos;
		Quaternion localRot;
		Transform parent;
		Transform child;

		public GrabTransform (Transform parent, Transform child)
		{
			this.parent = parent;
			this.child = child;
			localPos = parent.InverseTransformPoint (child.position);
//			localRot = Quaternion.FromToRotation (parent.forward, child.forward);
			localRot = Quaternion.Inverse(parent.rotation) * child.rotation;
		}

		public Vector3 TransformedPos
		{
			get {
				return parent.TransformPoint (localPos);
			}
		}

		public Quaternion TransformedRot
		{
			get
			{
				return parent.rotation * localRot;
			}
		}
	}
}

