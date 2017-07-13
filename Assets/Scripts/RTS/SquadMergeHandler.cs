using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS.World.Squads;

public class SquadMergeHandler : MonoBehaviour {

	Squad mergeFirst;
	Squad[] toMerge;
	void Start () {
		
	}

	void Update () {
		Debug.Log ("yo");
		mergeFirst = null;
		var j = 0;
		var i = 0;
		foreach (var squad in Squad.AllSquads) {
			i++;
		}
		if (i > 0) {
			toMerge = new Squad[i];
			foreach (var squad in Squad.AllSquads) {
				if (squad.wantsToMerge == true) {
					toMerge [j] = squad;
					j++;
				}
			}
			Debug.Log (toMerge);
			if (j > 0) {
				mergeFirst = toMerge [0];
				if (mergeFirst != null) {
					for (var k = 1; k < j; k++) {
						toMerge [k].MergeInto (mergeFirst);
					}
				}
				foreach (var squad in Squad.AllSquads) {
					squad.wantsToMerge = false;
				}
			}
		}

	}
}
