using RTS.World.Squads;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Tem que ter isso aqui pra ter o menu de criar asset
[CreateAssetMenu()]
public class BannerManager : ScriptableObject {
    
    private Dictionary<Squad, Banner> bannerFromSquad;
    public GameObject bannerPrefab;
    public void Start () {
        bannerFromSquad = new Dictionary<Squad, Banner>();
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
        try
        {
            currentBanner = bannerFromSquad[squad];
            currentBanner.Move(calculateSquadPosition(squad));
        }
        catch (KeyNotFoundException)
        {
            currentBanner = GameObject.Instantiate(bannerPrefab).GetComponent<Banner>();
            bannerFromSquad[squad] = currentBanner;
            currentBanner.setPosition(calculateSquadPosition(squad));
            //make banner get destroyed when squad is destroyed
            var a = currentBanner;
            squad.OnDestroyed += () => DestroyBanner(a, squad);
        }
        currentBanner.setAnimosity(squad.animosity);
	}

    void DestroyBanner(Banner ban, Squad squad)
    {
        if (squad!=null)
        {
            try
            {
                bannerFromSquad.Remove(squad);
            }
            catch (KeyNotFoundException)
            {
                //do nothing
            }
        }
        if (ban!=null)
        {
            ban.Destroy();
        }

    }

}
