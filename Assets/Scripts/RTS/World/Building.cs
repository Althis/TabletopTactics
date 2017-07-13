﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.World
{
    public class Building : MonoBehaviour, ITargetable, IInteractive, IHittable, IHealth
    {
        [System.Serializable]
        public class Settings
        {
            public int MaxHealth = 20;
            public int DeathMoraleLoss = 1;
        }


        //health and type
        private int health;
        public float getHp ()
        {
            return (float)health;
        }
        public HitType type;
        public HitType getType()
        {
            return type;
        }

        public Settings settings;

        [Space]
        [SerializeField]
        private Team team;

        [SerializeField]
        private bool targetable;

        public event System.Action<float> OnHealthChanged;



        public event System.Action OnDestroyed;
        public bool Targetable { get { return targetable; } }
        public GameObject Owner { get { return gameObject; } }
        public Team Team { get { return team; } }
        public Vector3 position { get { return transform.position; } }
        public float MaxHealth { get { return settings.MaxHealth; } }
        public float Health { get { return health; } }

        void Awake()
        {
            health = settings.MaxHealth;
            type = RTS.HitType.Building;
        }
        public void OnDestroy()
        {
            if (OnDestroyed != null)
                OnDestroyed();
        }


        public void OnHit(int damage)
        {
            if (this != null) // isso é pra evitar que, ao sofrer ataques simultâneos, a morte ocorra (ou tente ocorrer) várias vezes
            {
                this.health -= damage;
                this.OnHealthChanged(this.health);
                if (this.health <= 0)
                    OnHealthZero();
            }
        }
        
        public void OnHealthZero()
        {
            int a = team.Morale;
            int b = settings.DeathMoraleLoss;
            if (OnDestroyed != null)
                OnDestroyed();
            GameObject.Destroy(gameObject);
        }

    }
}