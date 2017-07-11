using RTS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RTS.World.Squads;
using System;

public class Banner : MonoBehaviour, ISelectionUnit
{
    public Squad squad;
	public LayerMask objectCheckMask;
	public float checkDistance = 20;

	Vector3 dampVel;

    public event Action OnDestroyed;

    //selection stuff
    public Squad Squad { get { return squad; } }
    public bool Selectable { get { return true; } }
    public GameObject Owner { get { return gameObject; } }
    public Team Team { get { return squad.Team; } }
    public event Action selectionAction;
    public event Action OnSelected { add { selectionAction += value; } remove { selectionAction -= value; } }
    public event Action deselectionAction;
    public event Action OnDeselected { add { deselectionAction += value; } remove { deselectionAction -= value; } }
    public SelectionIndicator selectionIndicator;
    public void OnSelect()
    {
        selectionAction();
    }
    public void OnDeselect()
    {
        deselectionAction();
    }

    public void setPosition(Vector3 newPosition)
    {
        this.transform.position=GetRealTarget(newPosition);
    }

    void Awake ()
    {
        selectionIndicator.unit = this;
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
