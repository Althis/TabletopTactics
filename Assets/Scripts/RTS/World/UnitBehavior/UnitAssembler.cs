using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;
using RTS.Util;
using RTS.World.Units;


namespace RTS.World.UnitBehavior
{
    public class UnitAssembler: MonoBehaviour
    {
        public static GameObject UnitPrefab { get; set; }


        public SelectionIndicator SelectionIndicator;
        public Transform CanvasHolder;
        public HealthBar healthBar;
        public NavMeshAgent agentNM;

        [Space]
        public Team team;
        public UnitClassData classData;

        public void Awake()
        {
            VerifyReferences();

            GameObject modelObj = AssembleModel();
            VerifyModel(modelObj);
            var model = modelObj.GetComponent<UnitModel>();
            model.UnitMaterial = team.DefaultUnitMaterial;

            Unit unit = AssembleUnit(modelObj);

            this.SelectionIndicator.unit = unit;


            foreach (var highlighter in modelObj.GetComponentsInChildren<UnitMeshHighlight>())
            {
                highlighter.unit = unit;
            }
            healthBar.BarColor = team.UIColor;

            Destroy(this);
        }

        private Unit AssembleUnit(GameObject model)
        {
            Unit unit = gameObject.AddWithPreemptiveExecution<Unit>((u) =>
            {
                u.settings = classData.settings;
                u.StartTeam = team;
                u.animationHandler = model.GetComponentInChildren<UnitAnimationHandler>();
            });
			unit.animationHandler.OnHitFrame+=()=>{UnitSoundHandler.singleton.playHitAudio(unit);};
            return unit;
        }

        private GameObject AssembleModel()
        {
            var model = GameObject.Instantiate(classData.model, transform);
            var animator = model.GetComponent<Animator>();

            foreach (var child in model.GetComponentsInChildren<ChildOfInteractiveGameObject>())
            {
                child.owner = gameObject;
            }

            return model;
        }

        private void VerifyModel(GameObject model)
        {
            Debug.Assert(model.GetComponentInChildren<Collider>() != null);
            Debug.Assert(model.GetComponentInChildren<ChildOfInteractiveGameObject>() != null);
            Debug.Assert(model.GetComponentsInChildren<Animator>() != null);
            Debug.Assert(model.GetComponentInChildren<UnitAnimationHandler>() != null);
            Debug.Assert(model.GetComponentInChildren<UnitModel>() != null);
        }

        private void VerifyReferences()
        {
            Debug.Assert(SelectionIndicator != null);
            Debug.Assert(CanvasHolder != null);
            Debug.Assert(healthBar != null);

            Debug.Assert(agentNM != null);


            Debug.Assert(classData != null);
        }
    }

}
