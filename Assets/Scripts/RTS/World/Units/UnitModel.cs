using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.World.Units
{
    public class UnitModel : MonoBehaviour
    {
        public bool allowMaterialChange = true;
        public List<Renderer> excludedRenderers = new List<Renderer>();

        public Material UnitMaterial
        {
            set
            {
                if (!allowMaterialChange)
                    return;
                foreach (var renderer in transform.GetComponentsInChildren<Renderer>())
                {
                    if (!excludedRenderers.Contains(renderer))
                    {
                        renderer.material = value;
                    }
                }
            }
        }

    }
}