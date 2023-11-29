namespace Portfolio.Gameplay.Combat;

public readonly struct Damage
{
    public readonly int Amount;
    public readonly DamageType Type;

    public Damage(int amount, DamageType type)
    {
        Amount = amount;
        Type = type;
    }
}
