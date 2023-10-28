using System;
using Portfolio.Entities;

namespace Portfolio.Gameplay.Queries;

public class CharacterPositionsQuery : IQuery<CharacterPositionsQuery.Result>
{
    public sealed class Result : IDisposable
    {
        public void Dispose()
        {
            // ...
        }
    }

    public Result Execute(World world)
    {
        return new Result();
    }
}
