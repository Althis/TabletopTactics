using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ViewObject : MonoBehaviour {
    
    void Awake()
    {
        OnExitView();
    }

    public void OnExitView()
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
		foreach (var canvas in GetComponentsInChildren<Canvas>()) {
			canvas.enabled = false;
		}
    }

    public void OnEnterView()
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = true;
		}
		foreach (var canvas in GetComponentsInChildren<Canvas>()) {
			canvas.enabled = true;
		}
    }
}