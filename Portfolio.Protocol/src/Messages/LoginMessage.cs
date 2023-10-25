using LiteNetLib.Utils;

namespace Portfolio.Protocol.Messages
{
    public class LoginMessage : IMessage
    {
        public string Token = string.Empty;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Token);
        }

        public void Deserialize(NetDataReader reader)
        {
            Token = reader.GetString();
        }
    }
}
