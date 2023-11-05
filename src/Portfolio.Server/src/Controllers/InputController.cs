using System.Numerics;
using System.Threading.Tasks;
using Portfolio.Gameplay;
using Portfolio.Gameplay.Commands;
using Portfolio.Protocol.Requests;
using Portfolio.Server.Net;

namespace Portfolio.Server.Controllers;

public class InputController : IController<InputRequest>
{
    private readonly Game _game;

    public InputController(Game game)
    {
        _game = game;
    }

    public Task Handle(Connection connection, InputRequest request)
    {
        var inputCommand = new InputCommand(
            connection.Id, new Vector2(request.Direction.X, request.Direction.Y));

        _game.Command(inputCommand);

        return Task.CompletedTask;
    }
}
