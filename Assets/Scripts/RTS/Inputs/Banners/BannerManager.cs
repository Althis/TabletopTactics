using RTS.World.Squads;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Tem que ter isso aqui pra ter o menu de criar asset
[CreateAssetMenu()]
public class BannerManager : ScriptableObject {
    
    public GameObject bannerPrefab;
	
    public void Start()
    {

    }

    private Vector3 calculateSquadPosition (Squad squad)
    {
        Vector3 result = new Vector3(0,0,0);
        var i = 0;
        foreach (var squaddie in squad.Units)
        {
            if (squaddie==null || squaddie.Equals(null))
            {
                continue;
            }
            i++;
            result += squaddie.position;
        }
        result /= i;
        return result;
    }

	public void Step (Squad squad) {
        Banner currentBanner;
        currentBanner = squad.banner;

        if (currentBanner!=null && !currentBanner.Equals(null))
        {
            if (currentBanner.grasped == false)
            {
                Vector3 newPosition = calculateSquadPosition(squad);
                if (!float.IsNaN(newPosition.x) && !float.IsNaN(newPosition.y) && !float.IsNaN(newPosition.z))
                {
                    currentBanner.Move(newPosition);
                }
            }
        }
        else
        {
            currentBanner = GameObject.Instantiate(bannerPrefab).GetComponent<Banner>();
            squad.banner = currentBanner;
            currentBanner.setPosition(calculateSquadPosition(squad));
            currentBanner.interactionObject.OnGraspBegin +=()=> { onGraspBegin(currentBanner); };
            currentBanner.interactionObject.OnGraspEnd+= () => { onGraspEnd(currentBanner); };
            //make banner get destroyed when squad is destroyed
            var a = currentBanner;
            squad.OnDestroyed += () => { DestroyBanner(a, squad); };
        }
        currentBanner.squad = squad;
    }

    void onGraspBegin(Banner banner)
    {
        if (banner != null && !banner.Equals(null))
        {
            banner.grasped = true;
        }
    }
    void onGraspEnd(Banner banner)
    {
        if (banner != null && !banner.Equals(null))
        {
            banner.grasped = false;
            banner.setUpwards();

            Vector3 squadTarget = banner.getSquadTargetFromBanner();
            Debug.Log(squadTarget);
            if (!float.IsNaN(squadTarget.x) && !float.IsNaN(squadTarget.z) && !float.IsNaN(squadTarget.y))
            {
                banner.squad.setTarget(new TargetInformation(null, squadTarget));
            }
            else
            {
                //send feedback to user saying that some crazy shit is going on
            }
        }
    }

    void DestroyBanner(Banner ban, Squad squad)
    {
        if (ban!=null)
        {
            ban.Destroy();
        }

    }

}
