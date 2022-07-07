using System;
using System.Collections.Generic;
using System.Linq;

namespace ExperimentsLib
{
    /// <summary>
    /// Manages a collection of <see cref="IPayloadWriter{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WriterCollection<T>
    {
        private readonly List<(string name, IPayloadWriter<T> writer)> writers = new List<(string name, IPayloadWriter<T> writer)>();

        public void AddWriter(string name, string charset, IPayloadWriter<T> serverWriter)
        {
            writers.Add((name, serverWriter));
        }

        public void AddWriters(params (string name, string charset, IPayloadWriter<T> serverWriter)[] servers)
        {
            foreach (var (name, charset, serverWriter) in servers)
            {
                AddWriter(name, charset, serverWriter);
            }
        }

        public IPayloadWriter<T> GetWriter(string name)
        {
            return FindWriter(name);
        }

        public IEnumerable<string> GetServerNames()
        {
            var names = writers.Select(r => r.name);

            foreach (var name in names)
            {
                Console.WriteLine(name);
            }

            return names;
        }

        private IPayloadWriter<T> FindWriter(string name)
        {
            var entry = writers
                .FirstOrDefault(entry => entry.name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (entry == default)
            {
                throw new Exception($"Server not found '{name}'");
            }

            return entry.writer;
        }
    }
}
