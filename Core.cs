using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace ratel_.net_client
{
    internal class Core
    {

        public static bool @is;

        public static Socket client = null;

        public static string nickName = string.Empty;

        public static void StartClient(string ip, int port, string nk)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(ip);

                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                client = new Socket(ipAddress.AddressFamily,
                   SocketType.Stream, ProtocolType.Tcp);
                client.Connect(remoteEP);
                var model = new Model
                {
                    Id = DateTimeOffset.UtcNow.UtcTicks,
                    Name = nk
                };
                nickName = nk;
                string authInfoStr = $"{"{"}" +
                    $"\"ID\":{model.Id}," +
                    $"\"Name\":\"{model.Name}\"," +
                    $"\"Score\":{model.Score}" +
                    $"{"}"}";
                var authByte = Encoding.ASCII.GetBytes(authInfoStr);
                client.Send(Tool.Encode(new Packet()
                {
                    Body = authByte
                }));
                new Thread(() => ConsoleListener()).Start();
                ServerListener();
            }
            catch (Exception)
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
        }

        public static void ServerListener()
        {
            if (client == null) throw new Exception("client uninitialized");
            while (true)
            {
                var pk = Tool.Decode(client);
                string data = Encoding.ASCII.GetString(pk.Body, 0, pk.Body.Length);
                if (data == Consts.IsStart)
                {
                    if (!@is) Console.Write($"{Consts.cleanLine}[{nickName}@ratel ~]# ");
                    @is = true;
                    continue;
                }
                else if (data == Consts.IsStop)
                {
                    if (@is) Console.Write(Consts.cleanLine);
                    @is = false;
                    continue;
                }
                if (@is) Console.Write($"{Consts.cleanLine}{data}{Consts.cleanLine}[{nickName}@ratel ~]# ");
                else Console.Write(data);
            }
        }

        public static void ConsoleListener()
        {
            if (client == null) throw new Exception("client uninitialized");
            while (true)
            {
                var line = Tool.Readline();
                if (!@is) continue;
                if (!"e".Equals(line.ToLower()) && !"exit".Equals(line.ToLower()))
                    Console.Write($"{Consts.cleanLine}[{nickName}@ratel ~] ");
                client.Send(Tool.Encode(new Packet()
                {
                    Body = Encoding.ASCII.GetBytes(line)
                }));
            }
        }

    }
}
