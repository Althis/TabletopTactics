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
            i++;
            result += squaddie.position;
        }
        result /= i;
        return result;
    }

	public void Step (Squad squad) {
        Banner currentBanner;
        currentBanner = squad.banner;

        if (currentBanner!=null)
        {
            currentBanner.Move(calculateSquadPosition(squad));
        }
        else
        {
            currentBanner = GameObject.Instantiate(bannerPrefab).GetComponent<Banner>();
            squad.banner = currentBanner;
            currentBanner.setPosition(calculateSquadPosition(squad));
            //make banner get destroyed when squad is destroyed
            var a = currentBanner;
            squad.OnDestroyed += () => DestroyBanner(a, squad);
        }
        currentBanner.squad = squad;
    }

    void DestroyBanner(Banner ban, Squad squad)
    {
        if (ban!=null)
        {
            ban.Destroy();
        }

    }

}
