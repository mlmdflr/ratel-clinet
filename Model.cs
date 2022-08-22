namespace ratel_clinet
{
    internal class Model
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Score
        {
            get { return 99999; }
            internal set { }
        }
    }

    internal class Packet
    {
        public const int MaxPacketSize = 65536;
        public byte[] Body { get; set; }
    }
}
