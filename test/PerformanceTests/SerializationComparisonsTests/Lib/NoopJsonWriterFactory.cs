using Microsoft.OData.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentsLib
{
    public class NoopJsonWriterFactory : IJsonWriterFactory, IJsonWriterFactoryAsync
    {
        public IJsonWriterAsync CreateAsynchronousJsonWriter(TextWriter textWriter, bool isIeee754Compatible)
        {
            return new NoopJsonWriter();
        }

        public IJsonWriter CreateJsonWriter(TextWriter textWriter, bool isIeee754Compatible)
        {
            return new NoopJsonWriter();
        }

        public IJsonWriter CreateJsonWriterFromStream(Stream stream, bool isIeee754Compatible)
        {
            return new NoopJsonWriter();
        }
    }
}
