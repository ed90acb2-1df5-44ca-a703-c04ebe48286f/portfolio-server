using Portfolio.Client;
using Portfolio.Protocol.Commands;
using Portfolio.Protocol.Messages;

var client = new NetworkingService();

client.RegisterMessageHandler<LoginMessage>(packet =>
{
    Console.WriteLine($"Packet Data: {packet.Token}");
});

client.Connect("127.0.0.1", 1447, "secret");

while (true)
{
    client.Update();

    await Task.Delay(1);

    client.Send(new LoginCommand() {Login = "user@mail.com", Password = "password"});
}
