using System.Numerics;
using System.Threading.Tasks;
using Portfolio.Gameplay;
using Portfolio.Gameplay.Commands;
using Portfolio.Protocol.Commands;
using Portfolio.Server.Net;

namespace Portfolio.Server.Controllers;

public class InputController : IController<InputCommand>
{
    private readonly Game _game;

    public InputController(Game game)
    {
        _game = game;
    }

    public Task Handle(Connection connection, InputCommand command)
    {
        var inputCommand = new InputPlayerCommand(new Vector2(command.Direction.X, command.Direction.Y));

        _game.Command(connection.Id, inputCommand);

        return Task.CompletedTask;
    }
}
