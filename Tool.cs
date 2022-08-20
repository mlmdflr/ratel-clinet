using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace ratel_.net_client
{
    internal class Tool
    {
        public static byte[] Encode(Packet packet)
        {
            byte[] lenBytes = new byte[4];
            PutUint32(ref lenBytes, (uint)packet.Body.Length);
            List<byte> data = new List<byte>();
            for (int i = 0; i < lenBytes.Length; i++) data.Add(lenBytes[i]);
            for (int i = 0; i < packet.Body.Length; i++) data.Add(packet.Body[i]);
            return data.ToArray();
        }

        public static Packet Decode(Socket client)
        {
            var l = ReadUint32(client);
            if (l > Packet.MaxPacketSize) return null;
            var data = new byte[l];
            client.Receive(data);
            return new Packet()
            {
                Body = data
            };
        }

        public static void PutUint32(ref byte[] b, uint v)
        {
            _ = b[3];
            b[0] = (byte)(v >> 24);
            b[1] = (byte)(v >> 16);
            b[2] = (byte)(v >> 8);
            b[3] = (byte)(v);
        }

        public static uint ReadUint32(Socket client)
        {
            byte[] data = new byte[4];
            client.Receive(data);
            return Uint32(data);
        }

        public static uint Uint32(byte[] b)
        {
            _ = b[3];
            return (b[3]) | (uint)(b[2]) << 8 | (uint)(b[1]) << 16 | (uint)(b[0]) << 24;
        }


        public static string Readline()
        {
            return Console.ReadLine().Trim();
        }
    }
}
