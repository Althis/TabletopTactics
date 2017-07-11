using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewArea : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void OnTriggerEnter(Collider other)
    {
        var viewObj = other.GetComponent<ViewObject>();
        if (viewObj != null)
            viewObj.OnEnterView();
    }
    private void OnTriggerExit(Collider other)
    {
        var viewObj = other.GetComponent<ViewObject>();
        if (viewObj != null)
            viewObj.OnExitView();
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
    }
}