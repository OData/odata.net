using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentsLib
{
    /// <summary>
    /// Manages a collection of servers that can be started and stopped
    /// at the same time.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServerCollection<T>
    {
        T data;
        List<Entry> servers = new List<Entry>();
        string baseHost;

        public ServerCollection(T data, string baseHostUrl)
        {
            this.data = data;
            this.baseHost = baseHostUrl;
        }

        public void AddServer(string name, string charset, IServerWriter<T> serverWriter)
        {
            servers.Add(new Entry(name, serverWriter, new Server<T>(serverWriter, data, charset)));
        }

        public void AddServers(params (string name, string charset, IServerWriter<T> serverWriter)[] servers)
        {
            foreach (var (name, charset, serverWriter) in servers)
            {
                AddServer(name, charset, serverWriter);
            }
        }

        public IServerWriter<T> GetWriter(string name)
        {
            return FindEntry(name).Writer;
        }

        public void StartServers(int startPort = 9000)
        {
            int port = startPort;
            foreach (var entry in servers)
            {
                entry.Server.Start($"{baseHost}:{port}");
                Console.WriteLine($"{entry.Name} server running on {baseHost}:{port}");
                port++;
            }
        }

        public Server<T> StartServer(string name, int port)
        {
            Server<T> server = FindEntry(name).Server;

            server.Start($"{baseHost}:{port}");
            return server;
        }

        public async Task StopServers()
        {
            await Task.WhenAll(servers.Select(item => item.Server.Stop()));
        }

        public IEnumerable<string> GetServerNames()
        {
            var names = servers.Select(r => r.Name);

            foreach (var name in names)
            {
                Console.WriteLine(name);
            }

            return names;
        }

        private Entry FindEntry(string name)
        {
            Entry entry = servers
                .FirstOrDefault(entry => entry.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (entry == null)
            {
                throw new Exception($"Server not found '{name}'");
            }

            return entry;
        }

        internal class Entry
        {
            public Entry(string name, IServerWriter<T> writer, Server<T> server)
            {
                Name = name;
                Writer = writer;
                Server = server;
            }

            public string Name { get; set; }
            public IServerWriter<T> Writer { get; set; }
            public Server<T> Server { get; set; }
        }
    }
}
