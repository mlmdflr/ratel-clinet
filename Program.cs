using System;

namespace ratel_clinet
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string ip = Consts.DEFAULT_IP;
            int port = Consts.DEFAULT_PORT;
            string nickName;
            try
            {
                foreach (var argument in args) if (argument.StartsWith("--") && (argument.StartsWith("--ip=") || argument.StartsWith("--dn=")) && argument.IndexOf('=') > 0) ip = argument.Substring(argument.IndexOf('=') + 1);
                    else if (argument.StartsWith("--") && (argument.StartsWith("--p=") || argument.StartsWith("--port=")) && argument.IndexOf('=') > 0) port = int.Parse(argument.Substring(argument.IndexOf('=') + 1));
                    else if (argument.StartsWith("-") && (argument.StartsWith("-help") || argument.StartsWith("-h")) && argument.IndexOf('=') == -1) System.Diagnostics.Process.Start("https://github.com/ratel-online/server/blob/main/README.md");
            }
            catch (Exception)
            {
            }
        home:
            Console.Write("Nickname: ");
            while (true)
            {
                nickName = Tool.Readline();
                if (string.Empty.Equals(nickName)) goto home;
                break;
            }
            new Core().StartClient(ip, port, nickName);

        }
    }
}
