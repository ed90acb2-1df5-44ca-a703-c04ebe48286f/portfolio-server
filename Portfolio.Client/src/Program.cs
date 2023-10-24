using Portfolio.Client;
using Portfolio.Protocol.Packets;

var client = new ClientNetworkingService();

client.RegisterPacketHandler<LoginResponse>(packet =>
{
    Console.WriteLine($"Packet Data: {packet.Token}");
});

client.Connect("127.0.0.1", 1447, "secret");

while (true)
{
    client.Update();

    await Task.Delay(1000);

    client.Send(new LoginRequest() {Login = "user@mail.com", Password = "password"});
}
