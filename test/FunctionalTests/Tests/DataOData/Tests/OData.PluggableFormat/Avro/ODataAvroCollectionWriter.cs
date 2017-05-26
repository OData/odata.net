//---------------------------------------------------------------------
// <copyright file="ODataAvroCollectionWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ENABLE_AVRO
namespace Microsoft.Test.OData.PluggableFormat.Avro
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Hadoop.Avro.Schema;
    using Microsoft.OData;

    class ODataAvroCollectionWriter : ODataCollectionWriter
    {
        private readonly ODataAvroOutputContext outputContext;
        private TypeSchema writerSchema;
        private IList<object> collection;

        public ODataAvroCollectionWriter(ODataAvroOutputContext outputContext, TypeSchema schema)
        {
            Debug.Assert(outputContext != null, "outputContext != null");

            this.outputContext = outputContext;
            this.writerSchema = schema;
            collection = new List<object>();
        }

        public override void WriteStart(ODataCollectionStart collectionStart)
        {
        }

        public override void WriteItem(object item)
        {
            this.WriteItemImplementation(item);
        }

        public override void WriteEnd()
        {
            this.WriteEndImplementation();
            this.FlushImplementation();
        }

        public override void Flush()
        {
            this.FlushImplementation();
        }
        public override Task WriteStartAsync(ODataCollectionStart collectionStart)
        {
            return Task.Factory.StartNew(() => { });
        }

        public override Task WriteItemAsync(object item)
        {
            return Task.Factory.StartNew(() => WriteItemImplementation(item));
        }

        public override Task WriteEndAsync()
        {
            return Task.Factory.StartNew(this.WriteEndImplementation).ContinueWith(task => this.FlushImplementation());
        }

        public override Task FlushAsync()
        {
            return Task.Factory.StartNew(this.FlushImplementation);
        }

        private void WriteItemImplementation(object item)
        {
            if (this.writerSchema == null)
            {
                this.writerSchema = this.outputContext.AvroWriter.UpdateSchema(item, null, true);
            }

            collection.Add(item);
        }

        private void WriteEndImplementation()
        {
            if (collection.Any())
            {
                this.outputContext.AvroWriter.Write(collection.ToArray());
                collection.Clear();
            }
        }

        private void FlushImplementation()
        {
            this.outputContext.Flush();
        }
    }
}
#endif