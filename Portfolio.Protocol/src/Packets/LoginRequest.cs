namespace Portfolio.Protocol.Packets
{
    public class LoginRequest : IPacket
    {
        public string Login = string.Empty;
        public string Password = string.Empty;

        public void Serialize(IPacketWriter writer)
        {
            writer.WriteString(Login);
            writer.WriteString(Password);
        }

        public void Deserialize(IPacketReader reader)
        {
            Login = reader.ReadString();
            Password = reader.ReadString();
        }
    }
}
