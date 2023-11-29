using Portfolio.Entities;
using Portfolio.Gameplay.Combat;

namespace Portfolio.Gameplay.Events;

public readonly struct EntityDamagedEvent
{
    public readonly Damage Damage;
    public readonly Entity Victim;
    public readonly Entity Attacker;

    public EntityDamagedEvent(Damage damage, Entity victim, Entity attacker)
    {
        Damage = damage;
        Victim = victim;
        Attacker = attacker;
    }
}
