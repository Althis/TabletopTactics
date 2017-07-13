using RTS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RTS.World.Squads;
using System;
using RTS.Inputs.SteamVR;

[RequireComponent(typeof(VRGrabbable))]
public class Banner : MonoBehaviour, ISelectionUnit
{
    public Squad squad;
	public LayerMask objectCheckMask;
	public LayerMask groundLayers;
	public float checkDistance = 20;
	private VRGrabbable grabbable;
	private bool isDownwards=false;

	private AnimosityIndicator[] indicators;

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






    void Awake ()
    {
        selectionIndicator.unit = this;
		grabbable = GetComponent<VRGrabbable>();
		grabbable.onRelease.AddListener (onGraspEnd);
		indicators = GetComponentsInChildren<AnimosityIndicator> ();
    }

	void Start () {
		Debug.Assert (squad != null);
		setPosition (calculateSquadPosition(squad));
		updateAnimosityAppearance ();

    }

	void Update(){
		if (grabbable.Grabbed == false) {
			Move (calculateSquadPosition (squad));
		}
		if (Vector3.Dot (transform.up, Vector3.up) < 0) {
			if (isDownwards == false) {
				squad.animosity = !squad.animosity;
				updateAnimosityAppearance ();
			}
			isDownwards = true;
		} else {
			isDownwards = false;
		}

	}

	void updateAnimosityAppearance (){
		foreach (var indicator in indicators) {
			if (indicator.animosity == squad.animosity) {
				indicator.gameObject.SetActive (true);
			} else {
				indicator.gameObject.SetActive (false);
			}
		}
	}

	public void setPosition(Vector3 newPosition)
	{
		this.transform.position=FindBannerTarget(newPosition);
		setUpwards();
	}

    public void Move (Vector3 position)
    {
        var target = FindBannerTarget(position);    
		transform.position = Vector3.SmoothDamp (transform.position, target, ref dampVel, .1f);
    }



    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }




	void setUpwards()
	{
		this.transform.rotation = Quaternion.FromToRotation(new Vector3(0, 0, 0), new Vector3(0, 1, 0));
	}


	void onGraspEnd()
	{
		setUpwards();
		squad.wantsToMerge = true;
		Vector3 squadTarget =FindSquadTarget();
		if (!float.IsNaN(squadTarget.x) && !float.IsNaN(squadTarget.z) && !float.IsNaN(squadTarget.y))
		{
			squad.setTarget(new TargetInformation(null, squadTarget));
		}
		else
		{
			//send feedback to user saying that some crazy shit is going on
		}

	}

	Vector3 FindSquadTarget()
	{
		Vector3 startPos = transform.position + Vector3.up * checkDistance;

		RaycastHit rayResult;

		Physics.Raycast(new Ray(startPos, Vector3.down), out rayResult, checkDistance * 2, groundLayers.value);
		if (rayResult.collider != null)
			return rayResult.point;
		else return new Vector3(float.NaN, float.NaN,float.NaN); // quem chamou tem que verificar
	}

	Vector3 FindBannerTarget(Vector3 position)
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

	Vector3 calculateSquadPosition (Squad squad)
	{
		Vector3 result = new Vector3(0,0,0);
		var i = 0;
		foreach (var squaddie in squad.Units) {
			if (squaddie != null && !squaddie.Equals (null) && !float.IsNaN(squaddie.position.x) &&!float.IsNaN(squaddie.position.y)&& !float.IsNaN(squaddie.position.z)) {
				i++;
				result += squaddie.position;
			}
		}
		if (i>0){
			result /= i;
		}
		else{
			result= new Vector3 (0,0,0);
		}
		return result;
	}
}
