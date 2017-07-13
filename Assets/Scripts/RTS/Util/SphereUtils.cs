using System;
using UnityEngine;
using System.Collections.Generic;

namespace RTS.Util
{
	public static class SphereUtils
	{
		public static List<T> GetAllInSphere<T>(Vector3 pos, float range, int layerMaks)
		{
			List<T> objs = new List<T> ();
			foreach (var col in Physics.OverlapSphere(pos, range, layerMaks)) {
				var t = col.GetComponent<T> ();
				if (t != null) {
					objs.Add (t);
				}
			}
			return objs;
		}

	}
}

