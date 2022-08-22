
using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace ratel_clinet
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
                IPAddress ipAddress;
                try
                {
                    ipAddress = IPAddress.Parse(ip);
                }
                catch (Exception)
                {
                    ipAddress = Dns.GetHostAddresses(ip)[0];
                }

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
                var authByte = Encoding.UTF8.GetBytes(authInfoStr);
                client.Send(Tool.Encode(new Packet()
                {
                    Body = authByte
                }));
                new Thread(() => ConsoleListener()).Start();
                ServerListener();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[error] >>> {ip}:{port}: " + e.Message);
                Environment.Exit(0);
            }
        }

        public static void ServerListener()
        {
            if (client == null) throw new Exception("client uninitialized");
            while (true)
            {
            input:
                var pk = Tool.Decode(client);

                string data = Encoding.UTF8.GetString(pk.Body, 0, pk.Body.Length);
                if (data == Consts.IsStart)
                {
                    if (!@is) Console.Write($"{Consts.cleanLine}[{nickName}@ratel ~]# ");
                    @is = true;
                    goto input;
                }
                else if (data == Consts.IsStop)
                {
                    if (@is) Console.Write(Consts.cleanLine);
                    @is = false;
                    goto input;
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
            input:
                var line = Tool.Readline();
                if (!@is)
                {
                    Console.Write($"{Consts.cleanLine}[{nickName}@ratel ~]# ");
                    goto input;
                };
                client.Send(Tool.Encode(new Packet()
                {
                    Body = Encoding.UTF8.GetBytes(line)
                }));
            }
        }

    }
}
