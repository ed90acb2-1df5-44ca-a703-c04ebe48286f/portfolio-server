using System.Numerics;
using Portfolio.Entities;
using Portfolio.Gameplay.Components;

namespace Portfolio.Gameplay.Commands;

public readonly struct InputPlayerCommand : IPlayerCommand
{
    private readonly Vector2 _direction;

    public InputPlayerCommand(Vector2 direction)
    {
        _direction = direction;
    }

    public void Execute(Entity entity, World world)
    {
        var input = world.GetComponent<Input>(entity);
        input.Direction = _direction;
    }
}
