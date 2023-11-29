using Portfolio.Entities;

namespace Portfolio.Gameplay.Queries;

public interface IQuery<out T>
{
    T Execute(World world);
}
