using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RTS.World.Squads;
using UnityEngine;
using RTS.World;

namespace RTS.AI
{
    //Tem que ter isso aqui pra ter o menu de criar asset
    [CreateAssetMenu()]

    class ThreeKindsAIStrategy : AIStrategy
    {
        public float viewRadius;
        public int numberOfMeeleUnitsOnOneEnemy;
        public int numberOfTotalUnitsOnOneEnemy;
        public float ArtillaryDeseperoRadius; //when under the distance from enemy to an artillary unit is smaller than this radius, the artillary unit will focus on this enemy
        public float ArtillaryStillThreshold;// should be around 1.5x the size of the archer's mesh

        Dictionary<Squad, Dictionary<IHittable, MeeleOrRanged>> numberOfSquaddiesOnEnemy; // given a squad and an enemy, this should return how many squaddies are already engaging that enemy
        private Dictionary<Squad, Dictionary<Unit, IHittable>> LockedEnemies; // com quem cada unidade de cada esquadrão está lutando
            
        private class MeeleOrRanged //this is to check if the units already attacking a unit are meele or ranged
        {
            public int Meele;
            public int Ranged;
            public MeeleOrRanged()
            {
                Meele = 0;
                Ranged = 0;
            }
        }

        public override void Start()
        {
            numberOfSquaddiesOnEnemy = new Dictionary<Squad, Dictionary<IHittable, MeeleOrRanged>>();
            LockedEnemies = new Dictionary<Squad, Dictionary<Unit, IHittable>>();
        }

        private bool isEnemy (Unit unit, IHittable supposedEnemy)
        {
            if (supposedEnemy.Team == unit.Team)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private HashSet<IHittable> FindSquadEnemies(Squad squad)
        {
            HashSet<IHittable> finalEnemiesList = new HashSet<IHittable>();
            foreach (var unit in squad.Units)
            {
                if (unit.Equals(null))
                {
                    continue;
                }
                Collider[] hitColliders = Physics.OverlapSphere(unit.position, viewRadius);
                int i = 0;
                while (i < hitColliders.Length)
                {
                    ChildOfInteractiveGameObject[] seenEnemies = hitColliders[i].gameObject.GetComponents<ChildOfInteractiveGameObject>();
                    if (seenEnemies != null && seenEnemies.Length != 0)
                    {
                        int j = 0;
                        while (j < seenEnemies.Length)
                        {
                            IHittable finalEnemy = seenEnemies[j].Owner.GetComponent<IHittable>();
                            if (finalEnemy != null && this.isEnemy(unit, finalEnemy))
                            {
                                finalEnemiesList.Add(finalEnemy);
                            }
                            j++;
                        }
                    }
                    i++;
                }
            }
            return finalEnemiesList;
        }

        private IHittable defineEnemy(Unit.ClassType squaddieType, HashSet<IHittable> enemiesList, Dictionary<IHittable, MeeleOrRanged> numberOfSiblingsOnEnemy, Unit squaddie)
        {
            IHittable currentTarget = null;
            //First, let's try to ignore the buildings
            HashSet<IHittable> buildingsList = new HashSet<IHittable>();
            if (squaddieType != Unit.ClassType.Siege) // the following is for "human" troops only
            {
                foreach (var enemy in enemiesList)
                {
                    if (enemy.getType() == RTS.HitType.Building)
                    {
                        buildingsList.Add(enemy);
                    }
                }
                if (buildingsList.Count() == enemiesList.Count()) //if all enemies are buildings
                {
                    enemiesList = buildingsList;
                }
                else
                {
                    foreach (var building in buildingsList)
                    {
                        enemiesList.Remove(building); //focus on troops first
                    }
                }
            }
            switch (squaddieType)
            {
                case Unit.ClassType.Infantry:
                    float smallestDistance = float.PositiveInfinity;
                    foreach (var enemy in enemiesList) 
                    {   
                        if (numberOfSiblingsOnEnemy[enemy].Meele < numberOfMeeleUnitsOnOneEnemy) //só atacar se tiver poucos "irmãos" atacando
                        {
                            float currentDistance = Vector3.Distance(enemy.position, squaddie.position);
                            if (currentDistance < smallestDistance)
                            {
                                smallestDistance = currentDistance;
                                currentTarget = enemy;
                            }
                        }
                    }
                    break;
                case Unit.ClassType.Artillary:
                    float smallestHp = float.PositiveInfinity;
                        foreach (var enemy in enemiesList)
                        {
                            if (numberOfSiblingsOnEnemy[enemy].Meele + numberOfSiblingsOnEnemy[enemy].Ranged < numberOfTotalUnitsOnOneEnemy) //não queremos arqueiros disperdiçando tiros e dando one-shot. 6 é um tanto arbitrário
                            {
                                if (enemy.getHp() < smallestHp)
                                {
                                    smallestDistance = enemy.getHp();
                                    currentTarget = enemy;
                                }
                            }
                        }
                    break;
                case Unit.ClassType.Cavalry:
                    HashSet<IHittable> oldEnemiesList = enemiesList;
                    HashSet<IHittable> archersList = new HashSet<IHittable>();
                    foreach (var enemy in enemiesList)
                    {
                        if (enemy.getType() == RTS.HitType.ArtillaryUnit)
                        {
                            archersList.Add(enemy);
                        }
                    }
                    if (archersList.Count() > 0 ) //if there are archers
                    {
                        enemiesList = archersList;
                    }
                    float largestDistance = float.NegativeInfinity;
                    foreach (var enemy in enemiesList)
                    {
                        if (numberOfSiblingsOnEnemy[enemy].Meele < numberOfMeeleUnitsOnOneEnemy) //só atacar se tiver poucos "irmãos" atacando
                        {
                            float currentDistance = Vector3.Distance(enemy.position, squaddie.position);
                            if (currentDistance > largestDistance)
                            {
                                largestDistance = currentDistance;
                                currentTarget = enemy;
                            }
                        }
                    }
                    if (currentTarget== null) //then none of the archers are avaiable (maybe because there are too many meele units around them
                    {
                        enemiesList = oldEnemiesList;
                        foreach (var archer in archersList)
                        {
                            enemiesList.Remove(archer);
                        }
                        foreach (var enemy in enemiesList)
                        {
                            if (numberOfSiblingsOnEnemy[enemy].Meele < numberOfMeeleUnitsOnOneEnemy) //só atacar se tiver poucos "irmãos" atacando
                            {
                                float currentDistance = Vector3.Distance(enemy.position, squaddie.position);
                                if (currentDistance > largestDistance)
                                {
                                    largestDistance = currentDistance;
                                    currentTarget = enemy;
                                }
                            }
                        }
                    }
                    break;
                default:
                    currentTarget = null;
                    break;
            }
            return currentTarget;
        }

        private bool AttackOrMove (Unit squaddie, IHittable target, HashSet<IHittable> enemiesList, Unit.ClassType type)
        {
            //true means attack, false means move
            switch (type)
            {
                case Unit.ClassType.Artillary:
                    float smallestDistance = float.PositiveInfinity;
                    //first, check if there are enemies closer than ArtillaryDesesperoRadius
                    foreach (var enemy in enemiesList)
                    {
                        float currentDistance = Vector3.Distance(enemy.position, squaddie.position);
                        if (currentDistance < smallestDistance)
                        {
                            smallestDistance = currentDistance;
                        }                        
                    }
                    if (smallestDistance > ArtillaryDeseperoRadius) //then we are relatively safe, let's find the most weakened enemy
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case Unit.ClassType.Infantry:
                case Unit.ClassType.Cavalry:
                    float targetDistance = Vector3.Distance(target.position, squaddie.position);
                    if (targetDistance < squaddie.Range)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                default:
                    return false;
                    break;
            }
        }

        private Vector3 whereToMove(Unit squaddie, IHittable target, HashSet<IHittable> enemiesList, Unit.ClassType type)
        {
            switch (type)
            {
                case Unit.ClassType.Artillary:
                    float smallestDistance = float.PositiveInfinity;
                    IHittable closestThreat = null;
                    //first, check if there are enemies closer than ArtillaryDesesperoRadius
                    foreach (var enemy in enemiesList)
                    {
                        float currentDistance = Vector3.Distance(enemy.position, squaddie.position);
                        if (currentDistance<smallestDistance)
                        {
                            smallestDistance = currentDistance;
                            closestThreat = enemy;
                        }
                    }
                    if (smallestDistance < ArtillaryDeseperoRadius) // if there are enemies whoa re too close
                    {
                        Vector3 result = (closestThreat.position - squaddie.position);
                        result.Normalize();
                        //maybe we should multiply result for something here, if it is too small.
                        return result;
                    }
                    else
                    {
                        if (smallestDistance < ArtillaryDeseperoRadius + ArtillaryStillThreshold)
                        {
                            //this is to prevent archers from dancing in position.
                            return squaddie.position;
                        }
                        else
                        {
                            return target.position;
                        }
                    }
                    break;
                case Unit.ClassType.Infantry:
                case Unit.ClassType.Cavalry:
                    return target.position;
                default:
                    return squaddie.position;
                    break;
            }
        }


        public override void Step(Squad squad)
        {
            bool animosity = squad.animosity;
            if (animosity == false && squad.TargetInfo != null && squad.TargetInfo.Position != null)
            {// se nós estamos em modo defensivo (animosity==false) e não há alvo-posição, não há nada a fazer

                int i = 0;
                float tmp1 = 0f;
                float tmp2 = 0f;
                float tmp3 = 0f;
                foreach (var squaddie in squad.Units)
                {

                    // squaddie.CurrentAction = UnitAction.MoveAction(squad.TargetInfo.Position + new Vector3(tmp, 0, 1.5f));
                    // i++;
                    // tmp += 1.5f;
                     if (squaddie.Type == Unit.ClassType.Infantry)
                     {
                         squaddie.CurrentAction = UnitAction.MoveAction(squad.TargetInfo.Position + new Vector3(tmp1, 0, 1.5f));
                         i++;
                         tmp1 += 1.5f;
                     }
                     if (squaddie.Type == Unit.ClassType.Artillary)
                     {
                         squaddie.CurrentAction = UnitAction.MoveAction(squad.TargetInfo.Position + new Vector3(tmp2, 0, 3.0f));
                         i++;
                         tmp2 += 1.5f;
                     }
                     if (squaddie.Type == Unit.ClassType.Cavalry)
                     {
                         squaddie.CurrentAction = UnitAction.MoveAction(squad.TargetInfo.Position + new Vector3(tmp3, 0, 0));
                         i++;
                         tmp3 += 1.5f;
                     }
                }
            }

            if (animosity == true)
            {
                HashSet<IHittable> enemiesList = this.FindSquadEnemies(squad);
                if (enemiesList.Count == 0)
                {
                    if (squad.TargetInfo != null && squad.TargetInfo.Position != null)
                    {
                        int i = 0;
                        float tmp1 = 0f;
                        float tmp2 = 0f;
                        float tmp3 = 0f;
                        foreach (var squaddie in squad.Units)
                        {

                            // squaddie.CurrentAction = UnitAction.MoveAction(squad.TargetInfo.Position + new Vector3(tmp, 0, 1.5f));
                            // i++;
                            // tmp += 1.5f;
                            if (squaddie.Type == Unit.ClassType.Infantry)
                            {
                                squaddie.CurrentAction = UnitAction.MoveAction(squad.TargetInfo.Position + new Vector3(tmp1, 0, 1.5f));
                                i++;
                                tmp1 += 1.5f;
                            }
                            if (squaddie.Type == Unit.ClassType.Artillary)
                            {
                                squaddie.CurrentAction = UnitAction.MoveAction(squad.TargetInfo.Position + new Vector3(tmp2, 0, 3.0f));
                                i++;
                                tmp2 += 1.5f;
                            }
                            if (squaddie.Type == Unit.ClassType.Cavalry)
                            {
                                squaddie.CurrentAction = UnitAction.MoveAction(squad.TargetInfo.Position + new Vector3(tmp3, 0, 0));
                                i++;
                                tmp3 += 1.5f;
                            }
                        }
                    }
                    else
                    {
                        foreach (var squaddie in squad.Units)
                        {
                            squaddie.CurrentAction = UnitAction.IdleAction();
                        }
                    }
                }
                else
                {
                    
                    Dictionary<Unit, IHittable> localLockedEnemies;
                    try
                    {
                        localLockedEnemies = LockedEnemies[squad];
                    }
                    catch
                    {
                        localLockedEnemies = new Dictionary<Unit, IHittable>();
                        LockedEnemies.Add(squad, localLockedEnemies);
                    }
                    foreach (var squaddie in squad.Units)
                    {
                        if (!localLockedEnemies.ContainsKey(squaddie))
                        {
                            localLockedEnemies.Add(squaddie, null);
                        }
                    }

                    Dictionary<IHittable, MeeleOrRanged> numberOfSiblingsOnEnemy;
                    try
                    {
                        numberOfSiblingsOnEnemy = numberOfSquaddiesOnEnemy[squad];
                    }
                    catch (KeyNotFoundException)
                    {
                        numberOfSiblingsOnEnemy = new Dictionary<IHittable, MeeleOrRanged>();
                        numberOfSquaddiesOnEnemy.Add(squad, numberOfSiblingsOnEnemy);
                    }

                    foreach (var enemy in enemiesList)
                    {
                        if (!numberOfSiblingsOnEnemy.ContainsKey(enemy))
                        {
                            numberOfSquaddiesOnEnemy[squad].Add(enemy, new MeeleOrRanged());
                        }
                    }

                    foreach (var squaddie in squad.Units) //action definition loop
                    {
                        if (squaddie.Equals(null))
                        {
                            continue;
                        }
                        IHittable currentTarget=null;
                        if (localLockedEnemies[squaddie] == (null) || localLockedEnemies[squaddie].Equals(null)) //then we have to search for an enemy
                        {
                            currentTarget = defineEnemy(squaddie.Type, enemiesList, numberOfSiblingsOnEnemy, squaddie);
                            if (!(currentTarget == null || currentTarget.Equals(null)))
                            {
                                switch (squaddie.Type)
                                {
                                    case Unit.ClassType.Infantry:
                                    case Unit.ClassType.Cavalry:
                                        numberOfSiblingsOnEnemy[currentTarget].Meele++;
                                        break;
                                    case Unit.ClassType.Artillary:
                                        numberOfSiblingsOnEnemy[currentTarget].Ranged++;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        else
                        { 
                            currentTarget = localLockedEnemies[squaddie];
                        }
                        if (currentTarget != null)
                        {
                            Debug.Log(currentTarget);
                            //decide if attacking or moving
                            bool SupposedToAttack = AttackOrMove(squaddie, currentTarget, enemiesList, squaddie.Type); //false means we're supposed to move
                            Debug.Log(SupposedToAttack);
                            if (SupposedToAttack)
                            {
                                squaddie.CurrentAction = UnitAction.AttackAction(currentTarget, whereToMove(squaddie, currentTarget, enemiesList, squaddie.Type));
                            }
                            else
                            {
                                
                                //decide where we're moving to
                                Vector3 whereTo = whereToMove(squaddie, currentTarget, enemiesList, squaddie.Type);
                                squaddie.CurrentAction = UnitAction.MoveAction(whereTo);
                            }
                            squaddie.OnDestroyed += () =>
                            {
                                try
                                {
                                    switch (squaddie.Type)
                                    {
                                        case Unit.ClassType.Infantry:
                                        case Unit.ClassType.Cavalry:
                                            numberOfSiblingsOnEnemy[currentTarget].Meele--;
                                            break;
                                        case Unit.ClassType.Artillary:
                                            numberOfSiblingsOnEnemy[currentTarget].Ranged--;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                catch (KeyNotFoundException)
                                {
                                    //ignore it
                                }
                            };
                        }
                        else
                        {
                            //seguir os outros caras e ficar pronto pra iniciar combate.
                            //o comportamento a seguir é temporário!
                            if (squad.TargetInfo != null && squad.TargetInfo.Position != null)
                            {
                                squaddie.CurrentAction = UnitAction.IdleAction();
                            }
                        }
                        if (currentTarget!=null && !currentTarget.Equals(null))// && currentTarget.getType() != RTS.HitType.Building) //don't lock on buildings, priorizze troops
                        {
                            localLockedEnemies[squaddie] = currentTarget;
                        }
                        
                    }
                }
            }

           
        }

        private void Squaddie_OnDestroyed()
        {
            throw new NotImplementedException();
        }
    }
}
