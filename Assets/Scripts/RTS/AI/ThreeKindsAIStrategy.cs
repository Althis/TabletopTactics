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
        class State
        {
            public bool animosity; //if false, it's defensive. If true, it's aggressive
            public State(bool animosity)
            {
                this.animosity = animosity;
            }
        }
        public float viewRadius;
        public int numberOfMeeleUnitsOnOneEnemy;
        public int numberOfTotalUnitsOnOneEnemy;
        public float ArtillaryDeseperoRadius; //when under the distance from enemy to an artillary unit is smaller than this radius, the artillary unit will focus on this enemy

        private Dictionary<Squad, State> states;
        private Dictionary<Squad, Dictionary<IHittable, int>> numberOfSquaddiesOnEnemy; // given a squad and an enemy, this should return how many squaddies are already engaging that enemy
        private Dictionary<Squad, Dictionary<Unit, IHittable>> LockedEnemies; // com quem cada unidade de cada esquadrão está lutando
            
        public override void Start()
        {
            states = new Dictionary<Squad, State>();
            numberOfSquaddiesOnEnemy = new Dictionary<Squad, Dictionary<IHittable, int>>();
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

        private IHittable defineEnemy(Unit.ClassType squaddieType, HashSet<IHittable> enemiesList, Dictionary<IHittable, int>numberOfSiblingsOnEnemy, Unit squaddie)
        {
            IHittable currentTarget = null;
            switch (squaddieType)
            {
                case Unit.ClassType.Infantry:
                    float smallestDistance = float.PositiveInfinity;
                    foreach (var enemy in enemiesList)
                    {
                        if (numberOfSiblingsOnEnemy[enemy] < numberOfMeeleUnitsOnOneEnemy) //só atacar se tiver poucos "irmãos" atacando
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
                            if (numberOfSiblingsOnEnemy[enemy] < numberOfTotalUnitsOnOneEnemy) //não queremos arqueiros disperdiçando tiros e dando one-shot. 6 é um tanto arbitrário
                            {
                                if (enemy.getHp() < smallestHp)
                                {
                                    smallestDistance = enemy.getHp();
                                    currentTarget = enemy;
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
                    Vector3 result = (closestThreat.position - squaddie.position);
                    result.Normalize();
                    //maybe we should multiply result for something here, if it is too small.
                    return result;
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
            bool animosity;
            try
            {
                animosity = states[squad].animosity;
            }
            catch (KeyNotFoundException)
            {
                animosity = true; //this is for testing purposes, usually it should be 'false' here
                states.Add(squad, new State(animosity));
            }
            if (animosity == false && squad.TargetInfo != null && squad.TargetInfo.Position != null)
            {// se nós estamos em modo defensivo (animosity==false) e não há alvo-posição, não há nada a fazer
                foreach (var squaddie in squad.Units)
                {
                    squaddie.CurrentAction = UnitAction.MoveAction(squad.TargetInfo.Position);
                }
            }

            if (animosity == true)
            {
                HashSet<IHittable> enemiesList = this.FindSquadEnemies(squad);
                if (enemiesList.Count == 0)
                {
                    if (squad.TargetInfo != null && squad.TargetInfo.Position != null)
                    {
                        foreach (var squaddie in squad.Units)
                        {
                            squaddie.CurrentAction = UnitAction.MoveAction(squad.TargetInfo.Position);
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

                    Dictionary<IHittable, int> numberOfSiblingsOnEnemy;
                    try
                    {
                        numberOfSiblingsOnEnemy = numberOfSquaddiesOnEnemy[squad];
                    }
                    catch (KeyNotFoundException)
                    {
                        numberOfSiblingsOnEnemy = new Dictionary<IHittable, int>();
                        numberOfSquaddiesOnEnemy.Add(squad, numberOfSiblingsOnEnemy);
                    }

                    foreach (var enemy in enemiesList)
                    {
                        if (!numberOfSiblingsOnEnemy.ContainsKey(enemy))
                        {
                            numberOfSquaddiesOnEnemy[squad].Add(enemy, 0);
                        }
                    }

                    foreach (var squaddie in squad.Units) //action definition loop
                    {
                        if (squaddie.Equals(null))
                        {
                            continue;
                        }
                        IHittable currentTarget=null;
                        if (localLockedEnemies[squaddie] == null) //then we have to search for an enemy
                        {
                            currentTarget = defineEnemy(squaddie.Type, enemiesList, numberOfSiblingsOnEnemy, squaddie);
                        }
                        else
                        {
                            if (!localLockedEnemies[squaddie].Equals(null) && localLockedEnemies != null)
                            {
                                currentTarget = localLockedEnemies[squaddie];
                            }
                        }
                        if (currentTarget != null)
                        {
                            
                            //decide if attacking or moving
                            bool SupposedToAttack = AttackOrMove(squaddie, currentTarget, enemiesList, squaddie.Type); //false means we're supposed to move
                            if (SupposedToAttack)
                            {
                                Debug.Log(currentTarget);
                                squaddie.CurrentAction = UnitAction.AttackAction(currentTarget, squaddie.position);
                            }
                            else
                            {
                                
                                //decide where we're moving to
                                Vector3 whereTo = whereToMove(squaddie, currentTarget, enemiesList, squaddie.Type);
                                squaddie.CurrentAction = UnitAction.MoveAction(whereTo);
                            }
                            numberOfSiblingsOnEnemy[currentTarget]++; // já foi checado antes se todos os inimigos tinham uma entrada no dicionário
                        }
                        else
                        {
                            //seguir os outros caras e ficar pronto pra iniciar combate.
                            //o comportamento a seguir é temporário!
                            if (squad.TargetInfo != null && squad.TargetInfo.Position != null)
                            {
                                squaddie.CurrentAction = UnitAction.MoveAction(squad.TargetInfo.Position);
                            }
                        }
                        localLockedEnemies[squaddie] = currentTarget;                     
                    }
                }
            }

           
        }
    }
}
