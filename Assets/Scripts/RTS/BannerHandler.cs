using RTS.AI;
using RTS.World.Squads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RTS
{
    public class BannerHandler : MonoBehaviour
    {
        public BannerManager manager;

        void Start()
        {
            manager.Start();
        }

        void Update()
        {
            foreach (var squad in Squad.AllSquads)
            {
                manager.Step(squad);
            }
        }
    }
}
