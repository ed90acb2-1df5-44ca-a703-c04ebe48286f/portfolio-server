using System.Numerics;
using Portfolio.Entities;
using Portfolio.Gameplay.Components;

namespace Portfolio.Gameplay.Commands;

public class InputCommand : ICommand
{
    private readonly int _playerId;
    private readonly Vector2 _direction;

    public InputCommand(int playerId, Vector2 direction)
    {
        _playerId = playerId;
        _direction = direction;
    }

    public void Execute(World world)
    {
        var query = new QueryBuilder(world)
            .Require<Player>()
            .Require<Input>()
            .Build();

        foreach (var entity in world.Query(query))
        {
            if (world.GetComponent<Player>(entity).Id == _playerId)
            {
                world.SetComponent(entity, new Input {Direction = _direction});
                break;
            }
        }
    }
}
