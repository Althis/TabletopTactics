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

	public void createBanner(Squad squad){
		Banner currentBanner = GameObject.Instantiate(bannerPrefab).GetComponent<Banner>();
        squad.banner = currentBanner;
        currentBanner.setPosition(calculateSquadPosition(squad));
//        currentBanner.interactionObject.OnGraspBegin +=()=> { onGraspBegin(currentBanner); };
//        currentBanner.interactionObject.OnGraspEnd+= () => { onGraspEnd(currentBanner); };
        //make banner get destroyed when squad is destroyed
        squad.OnDestroyed += () => { DestroyBanner(currentBanner, squad); };
	}

	public void Step (Squad squad) {
		if (squad.banner == null || squad.banner.Equals(null)) {
			createBanner(squad);
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
