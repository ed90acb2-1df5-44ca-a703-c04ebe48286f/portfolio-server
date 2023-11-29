using System;
using Portfolio.Gameplay.Combat;
using Portfolio.Gameplay.Events;
using Portfolio.Protocol.Messages;
using Damage = Portfolio.Protocol.Values.Damage;

namespace Portfolio.Server.Mappers;

public class EntityDamagedEventMapper : IMapper<EntityDamagedEvent, EntityDamagedMessage>
{
    public EntityDamagedMessage Map(EntityDamagedEvent source)
    {
        return new EntityDamagedMessage
        {
            Damage = new Damage
            {
                Amount = source.Damage.Amount,
                Type = ConvertDamageType(source.Damage.Type),
            },
            AttackerId = source.Attacker.Index,
            VictimId = source.Victim.Index,
        };
    }

    private static Damage.Types.DamageType ConvertDamageType(DamageType damageType)
    {
        return damageType switch
        {
            DamageType.True => Damage.Types.DamageType.True,
            DamageType.Physical => Damage.Types.DamageType.Physical,
            DamageType.Magical => Damage.Types.DamageType.Magical,
            _ => throw new ArgumentOutOfRangeException(nameof(damageType), damageType, null)
        };
    }
}
