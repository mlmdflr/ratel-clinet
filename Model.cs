namespace ratel_.net_client
{
    internal class Model
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Score
        {
            get { return 9999; }
            internal set { }
        }
    }

    internal class Packet
    {
        public const int MaxPacketSize = 65536;
        public byte[] Body { get; set; }
    }
}
