using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RTS
{
    public enum HitType {InfantryUnit, ArtillaryUnit, CavalryUnit, SiegeUnit, Building};
    public interface ISelectionUnit : IDestructionNotifier
    {
        World.Squads.Squad Squad { get; }
        bool Selectable { get; }
        GameObject Owner { get; }
        event Action OnSelected;
        event Action OnDeselected;

        Team Team { get; }
    }


    public interface IHighlightable : IDestructionNotifier
    {
        bool Highlightable { get; }

        void HighlightOn();
        void HighlightOff();
    }

    public interface IHittable: ITargetable
    {

        void OnHit(int damage);
        HitType getType();
        float getHp();
        Team Team { get;}
    }

    public interface ITargetable: IDestructionNotifier
    {
        bool Targetable { get; }

        Vector3 position { get; }
    }

    public interface IInteractive
    {
        GameObject Owner { get; }
    }

    public interface IDestructionNotifier
    {
        event Action OnDestroyed;
    }

    public interface IHealth
    {
        float Health { get; }

        float MaxHealth { get; }

        event Action<float> OnHealthChanged;
    }
}
