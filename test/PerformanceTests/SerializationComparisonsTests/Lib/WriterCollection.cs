//---------------------------------------------------------------------
// <copyright file="WriterCollection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace ExperimentsLib
{
    /// <summary>
    /// Manages a collection of <see cref="IPayloadWriter{T}" instances/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WriterCollection<T>
    {
        private readonly Dictionary<string, IPayloadWriter<T>> writers = new Dictionary<string, IPayloadWriter<T>>();

        public void AddWriter(string name, IPayloadWriter<T> serverWriter)
        {
            this.writers[name] = serverWriter;
        }

        public void AddWriters(params (string name, IPayloadWriter<T> serverWriter)[] servers)
        {
            foreach (var (name, serverWriter) in servers)
            {
                AddWriter(name, serverWriter);
            }
        }

        public IPayloadWriter<T> GetWriter(string name)
        {
            return FindWriter(name);
        }

        public IEnumerable<string> GetWriterNames()
        {
            return writers.Keys;
        }

        private IPayloadWriter<T> FindWriter(string name)
        {
            if (!this.writers.TryGetValue(name, out IPayloadWriter<T> writer))
            {
                throw new Exception($"Server not found '{name}'");
            }

            return writer;
        }
    }
}
