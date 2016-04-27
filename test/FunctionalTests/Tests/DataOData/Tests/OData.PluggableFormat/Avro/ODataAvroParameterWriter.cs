//---------------------------------------------------------------------
// <copyright file="ODataAvroParameterWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ENABLE_AVRO
namespace Microsoft.Test.OData.PluggableFormat.Avro
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Hadoop.Avro;
    using Microsoft.Hadoop.Avro.Schema;
    using Microsoft.OData;
    using Microsoft.OData.Edm;

    class ODataAvroParameterWriter : ODataParameterWriter
    {
        private readonly ODataAvroOutputContext outputContext;
        private AvroRecord record;
        private RecordSchema schema;

        public ODataAvroParameterWriter(ODataAvroOutputContext outputContext, IEdmOperation operation)
        {
            Debug.Assert(outputContext != null, "outputContext != null");
            Debug.Assert(operation != null, "operation != null");

            this.outputContext = outputContext;
            schema = (RecordSchema)this.outputContext.AvroWriter.UpdateSchema(null, this.GetTmpType(operation));
            this.record = new AvroRecord(schema);
        }

        private IEdmType GetTmpType(IEdmOperation operation)
        {
            EdmComplexType type = new EdmComplexType(operation.Namespace, operation.Name + AvroConstants.ParameterTypeSuffix);

            foreach (var parameter in operation.Parameters.Skip(operation.IsBound ? 1 : 0))
            {
                type.AddStructuralProperty(parameter.Name, parameter.Type);
            }

            return type;
        }

        public override void WriteStart()
        {
        }

        public override void WriteValue(string parameterName, object parameterValue)
        {
            this.record[parameterName] = ODataAvroConvert.FromODataObject(parameterValue, this.GetSubSchema(parameterName));
        }

        public override ODataCollectionWriter CreateCollectionWriter(string parameterName)
        {
            throw new System.NotImplementedException();
        }

        public override ODataWriter CreateResourceWriter(string parameterName)
        {
            return CreateWriterImplementation(parameterName, writingFeed: false);
        }

        public override ODataWriter CreateResourceSetWriter(string parameterName)
        {
            return CreateWriterImplementation(parameterName, writingFeed: true);
        }

        public override void WriteEnd()
        {
            this.outputContext.AvroWriter.Write(this.record);
        }

        public override void Flush()
        {
            this.outputContext.Flush();
        }

        private TypeSchema GetSubSchema(string name)
        {
            RecordField field;
            if (!schema.TryGetField(name, out field))
            {
                throw new ApplicationException("Given paramenter name " + name + " does not exist");
            }

            return field.TypeSchema;
        }

        public override System.Threading.Tasks.Task WriteStartAsync()
        {
            throw new System.NotImplementedException();
        }

        public override System.Threading.Tasks.Task WriteValueAsync(string parameterName, object parameterValue)
        {
            throw new System.NotImplementedException();
        }

        public override System.Threading.Tasks.Task<ODataCollectionWriter> CreateCollectionWriterAsync(string parameterName)
        {
            throw new System.NotImplementedException();
        }

        public override System.Threading.Tasks.Task<ODataWriter> CreateResourceWriterAsync(string parameterName)
        {
            throw new System.NotImplementedException();
        }

        public override System.Threading.Tasks.Task<ODataWriter> CreateResourceSetWriterAsync(string parameterName)
        {
            throw new System.NotImplementedException();
        }

        public override System.Threading.Tasks.Task WriteEndAsync()
        {
            throw new System.NotImplementedException();
        }
        public override System.Threading.Tasks.Task FlushAsync()
        {
            throw new System.NotImplementedException();
        }

        private ODataWriter CreateWriterImplementation(string parameterName, bool writingFeed)
        {
            var subSchema = this.GetSubSchema(parameterName);

            if (writingFeed)
            {
                subSchema = ODataAvroSchemaGen.GetItemSchema(subSchema);
            }

            return new ODataAvroWriter(
                this.outputContext,
                obj => { this.record[parameterName] = obj; },
                subSchema,
                writingFeed);
        }
    }
}
#endif