using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Banner : MonoBehaviour {

    private bool animosity; //true is aggressive, false is defensive
 
	public LayerMask objectCheckMask;
	public float checkDistance = 20;

	Vector3 dampVel;

    public void setAnimosity(bool newAnimosity)
    {
        animosity = newAnimosity;
    }
    public void setPosition(Vector3 newPosition)
    {
        this.transform.position=GetRealTarget(newPosition);
    }

    void Start () {
    }

    public void Move (Vector3 position)
    {
		var target = GetRealTarget(position);    
		transform.position = Vector3.SmoothDamp (transform.position, target, ref dampVel, .1f);
    }

	public Vector3 GetRealTarget(Vector3 position)
    {       
        Vector3 startPos = position + Vector3.up * checkDistance;

        RaycastHit rayResult;

		//Physics.Raycast(startPos, Vector3.down, objectCheckMask.value, out rayResult);
		Physics.Raycast (new Ray(startPos, Vector3.down), out rayResult, checkDistance * 2, objectCheckMask.value);
        Debug.Log(rayResult.collider);
        Debug.DrawRay(startPos, Vector3.down);
        if (rayResult.collider == null)
            return position;
        else
            return rayResult.point;
    }

    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }

	// Update is called once per frame
	void Update () {
		//interaction code (such as picking the banner up) goes here.
	}
}
