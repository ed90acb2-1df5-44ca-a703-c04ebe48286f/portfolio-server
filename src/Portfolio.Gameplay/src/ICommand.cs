using Portfolio.Entities;

namespace Portfolio.Gameplay;

public interface ICommand
{
    void Execute(World world);
}
