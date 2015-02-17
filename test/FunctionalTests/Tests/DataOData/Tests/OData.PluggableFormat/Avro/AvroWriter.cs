//---------------------------------------------------------------------
// <copyright file="AvroWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ENABLE_AVRO
namespace Microsoft.Test.OData.PluggableFormat.Avro
{
    using System;
    using System.IO;
    using Microsoft.Hadoop.Avro;
    using Microsoft.Hadoop.Avro.Container;
    using Microsoft.Hadoop.Avro.Schema;
    using Microsoft.OData.Edm;

    internal sealed class AvroWriter : IDisposable
    {
        private readonly Stream stream;
        private SequentialWriter<object> seqWriter;

        public AvroWriter(Stream stream)
        {
            this.stream = stream;
        }

        public void Write(object value)
        {
            this.seqWriter.Write(value);
        }

        public void Dispose()
        {
            if (this.seqWriter != null)
            {
                this.seqWriter.Flush();
                this.seqWriter.Dispose();
            }
        }

        internal void Flush()
        {
            if (seqWriter != null)
            {
                this.seqWriter.Flush();
                this.stream.Flush();
            }
        }

        public TypeSchema UpdateSchema(object value, IEdmType edmType, bool collection = false)
        {
            TypeSchema schema = null;
            if (edmType != null)
            {
                try
                {
                    schema = ODataAvroSchemaGen.GetSchemaFromModel(edmType);
                }
                catch (ApplicationException) { }
            }

            if (schema == null)
            {
                if (value == null)
                {
                    return null;
                }

                schema = ODataAvroSchemaGen.GetSchema(value);
            }

            TypeSchema single = AvroSerializer.CreateGeneric(schema.ToString()).WriterSchema;

            if (collection)
            {
                schema = ODataAvroSchemaGen.GetArraySchema(schema);
                single = ((ArraySchema)AvroSerializer.CreateGeneric(schema.ToString()).WriterSchema).ItemSchema;
            }

            var writer = AvroContainer.CreateGenericWriter(schema.ToString(), stream, true, Codec.Null);
            this.seqWriter = new SequentialWriter<object>(writer, 16);

            return single;
        }
    }
}
#endif 