using LiteNetLib.Utils;

namespace Portfolio.Protocol.Commands
{
    public struct LoginCommand : ICommand
    {
        public string Login;
        public string Password;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Login);
            writer.Put(Password);
        }

        public void Deserialize(NetDataReader reader)
        {
            Login = reader.GetString();
            Password = reader.GetString();
        }
    }
}
