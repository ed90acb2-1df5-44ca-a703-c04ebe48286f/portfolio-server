using Portfolio.Client;
using Portfolio.Protocol.Commands;
using Portfolio.Protocol.Messages;
using Timer = System.Timers.Timer;

var counter = 0;
var timer = new Timer(1000);
timer.AutoReset = true;
timer.Elapsed += (_, _) =>
{
    Console.WriteLine(counter);
    counter = 0;
};
timer.Start();

var client = new ClientNetworkingService();
client.Connect("127.0.0.1", 1447, "secret");

client.RegisterHandler<LoginMessage>((_, packet) =>
{
    counter = Interlocked.Increment(ref counter);
    //Console.WriteLine($"Packet Data: {packet.Token}");
});

while (true)
{
    client.Update();

    await Task.Delay(1);

    client.Send(new LoginCommand() {Login = "user@mail.com", Password = "password"});
}
