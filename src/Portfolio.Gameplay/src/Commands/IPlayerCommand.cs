using Portfolio.Entities;

namespace Portfolio.Gameplay.Commands;

public interface IPlayerCommand
{
    void Execute(Entity entity, World world);
}
