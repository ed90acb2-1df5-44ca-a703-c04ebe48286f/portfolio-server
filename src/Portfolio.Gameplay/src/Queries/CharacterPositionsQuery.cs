using System;
using System.Collections.Generic;
using System.Numerics;
using Portfolio.Entities;

namespace Portfolio.Gameplay.Queries;

public class CharacterPositionsQuery : IQuery<CharacterPositionsQuery.Result>
{
    public sealed class Result : IDisposable
    {
        public readonly List<Vector2> Positions = new();

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
