
using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace ratel_clinet
{
    internal class Core
    {

        public bool @is;

        public Socket client = null;

        public string nickName = string.Empty;


        public void StartClient(string ip, int port, string nk)
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

        public void ServerListener()
        {
            if (client == null) throw new Exception("client uninitialized");
            while (true)
            {
            input:
                var pk = Tool.Decode(client);
                string data = Encoding.UTF8.GetString(pk.Body, 0, pk.Body.Length);
                if (Consts.IsStart.Equals(data))
                {
                    if (!@is) Write($"{Consts.cleanLine}[{nickName}@ratel ~]# ");
                    @is = true;
                    goto input;
                }
                else if (Consts.IsStop.Equals(data))
                {
                    if (@is) Write(Consts.cleanLine);
                    @is = false;
                    goto input;
                }
                if (@is) Write($"{Consts.cleanLine}{data}{Consts.cleanLine}[{nickName}@ratel ~]# ");
                else Write(data);
            }
        }

        public void ConsoleListener()
        {
            if (client == null) throw new Exception("client uninitialized");
            while (true)
            {
            input:
                var line = Tool.Readline();
                if (!@is || string.Empty.Equals(line))
                {
                    Write($"{Consts.cleanLine}[{nickName}@ratel ~]# ");
                    goto input;
                };
                client.Send(Tool.Encode(new Packet()
                {
                    Body = Encoding.UTF8.GetBytes(line)
                }));
            }
        }


        public void Write(string str)
        {
            lock (this)
            {
                Console.Write(str);
            }
        }

    }
}
