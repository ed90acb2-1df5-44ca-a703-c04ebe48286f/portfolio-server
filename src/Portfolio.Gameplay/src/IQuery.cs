using Portfolio.Entities;

namespace Portfolio.Gameplay;

public interface IQuery<out T>
{
    T Execute(World world);
}
