using System.Collections.Generic;
using Portfolio.Gameplay.Combat;

namespace Portfolio.Gameplay.Components;

readonly struct IncomingDamage
{
    public readonly Queue<Damage> Value;

    public IncomingDamage()
    {
        Value = new Queue<Damage>();
    }
}
