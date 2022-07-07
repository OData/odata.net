using System.IO;
using Microsoft.OData.Json;

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
    }
}
