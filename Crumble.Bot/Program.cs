using System.Diagnostics;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

namespace Crumble.Bot
{
    class Program
    {
        private class Config
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public List<string> Tokens { get; set; }
            public string Server { get; set; }
            public int Port { get; set; }
        }

        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            string configJson = File.ReadAllText("config.json");
            Config config = JsonConvert.DeserializeObject<Config>(configJson);

            MumbleClient c = new MumbleClient();
            await c.ConnectAsync(new IPEndPoint(Dns.GetHostAddresses(config.Server)[0], config.Port), config.Server);
            ProtoBuf.Version v = new ProtoBuf.Version();
            v.os = "Operating System";
            v.os_version = "Version";
            v.version = 0x010200;
            v.release = "Release";

            ProtoBuf.Authenticate a = new ProtoBuf.Authenticate();
            a.username = config.Username;
            a.password = config.Password;
            a.tokens.AddRange(config.Tokens);

            await c.SendPacketAsync(v, PacketType.Version);
            await c.SendPacketAsync(a, PacketType.Authenticate);
            var poll = c.PollAsync();
            var delay = Task.Delay(1000);
            while (true)
            {
                var task = Task.WaitAny(delay, poll);
                if (task == 1)
                    poll = c.PollAsync();
                else
                {
                    delay = Task.Delay(1000);
                    ProtoBuf.Ping p = new ProtoBuf.Ping();
                    p.timestamp = (ulong)Stopwatch.GetTimestamp();
                    await c.SendPacketAsync(p, PacketType.Ping);
                }
            }
            
        }
    }
}
