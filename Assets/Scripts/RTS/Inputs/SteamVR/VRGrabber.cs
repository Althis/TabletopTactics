using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS.Util;
using System.Linq;
using UnityEngine.Events;

namespace RTS.Inputs.SteamVR
{
	[RequireComponent(typeof(SteamVR_TrackedController))]
	public class VRGrabber : MonoBehaviour {

		public LayerMask layerMask;
		public float range;

		SteamVR_TrackedController controller;

		List<VRGrabbable> grabbed;


		void Awake()
		{
			grabbed = new List<VRGrabbable> ();
		}

		void Start () {
			controller = GetComponent<SteamVR_TrackedController> ();
			controller.TriggerUnclicked += Controller_TriggerUnclicked;
		}

		void Controller_TriggerUnclicked (object sender, ClickedEventArgs e)
		{
			ReleaseAll ();
		}

		void Update () {
			if (controller.triggerPressed) {
				List<VRGrabbable> allObjs = GrabbablesInRange ();
				TryAddToGrabbed (allObjs);
			}
		}

		void OnDrawGizmos()
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere (transform.position, range);
		}

		void ReleaseAll()
		{
			foreach (var obj in grabbed) {
				obj.OnReleasedBy (this);
			}
			grabbed = new List<VRGrabbable> ();
		}


		void TryAddToGrabbed(List<VRGrabbable> objects)
		{
			var newObjs = objects.FindAll (x => !grabbed.Contains (x));
			grabbed.AddRange (newObjs);

			foreach (var obj in newObjs) {
				obj.OnGrabbedBy (this);
			}
		}

		List<VRGrabbable> GrabbablesInRange()
		{
			return SphereUtils.GetAllInSphere<VRGrabbable> (transform.position, range, layerMask.value);
		}
	}
}