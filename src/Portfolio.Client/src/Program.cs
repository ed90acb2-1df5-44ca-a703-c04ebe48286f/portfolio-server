using System;
using System.Threading;
using System.Threading.Tasks;
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

var networkConnection = new NetworkConnection();
networkConnection.Open("127.0.0.1", 1447, "secret");
networkConnection.RegisterHandler<LoginMessage>((packet) =>
{
    counter = Interlocked.Increment(ref counter);
    //Console.WriteLine($"Packet Data: {packet.Token}");
});

networkConnection.RegisterHandler<BroadcastMessage>((packet) =>
{
    Console.WriteLine($"Broadcast received {packet.Positions.Count}");
});

while (true)
{
    networkConnection.PollEvents();

    await Task.Delay(1);

    networkConnection.Send(new LoginCommand() {Login = "user@mail.com", Password = "password"});
}
