//---------------------------------------------------------------------
// <copyright file="ODataAvroParameterReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ENABLE_AVRO
namespace Microsoft.Test.OData.PluggableFormat.Avro
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Hadoop.Avro;
    using Microsoft.Hadoop.Avro.Schema;
    using Microsoft.OData;
    using Microsoft.OData.Edm;

    internal class ODataAvroParameterReader : ODataParameterReader
    {
        private readonly ODataAvroInputContext inputContext;
        private readonly IEdmOperation operation;
        private ODataParameterReaderState state;
        private string name;
        private object value;
        private AvroRecord avroRecord;
        private IEnumerator<RecordField> enumerator;

        public ODataAvroParameterReader(ODataAvroInputContext inputContext, IEdmOperation operation)
        {
            Debug.Assert(inputContext != null, "inputContext != null");
            Debug.Assert(operation != null, "operation != null");

            this.inputContext = inputContext;
            this.operation = operation;
            this.state = ODataParameterReaderState.Start;
        }

        public override ODataParameterReaderState State
        {
            get { return state; }
        }

        public override string Name
        {
            get { return name; }
        }

        public override object Value
        {
            get { return value; }
        }

        public override ODataReader CreateResourceReader()
        {
            return new ODataAvroReader(this.inputContext, false, this.value);
        }

        public override ODataReader CreateResourceSetReader()
        {
            return new ODataAvroReader(this.inputContext, true, this.value);
        }

        public override ODataCollectionReader CreateCollectionReader()
        {
            throw new System.NotImplementedException();
        }

        public override bool Read()
        {
            switch (this.State)
            {
                case ODataParameterReaderState.Start:
                    if (!this.inputContext.AvroReader.MoveNext())
                    {
                        this.state = ODataParameterReaderState.Completed;
                        return false;
                    }

                    this.avroRecord = this.inputContext.AvroReader.Current as AvroRecord;
                    if (this.avroRecord == null)
                    {
                        this.state = ODataParameterReaderState.Exception;
                        return false;
                    }

                    this.enumerator = this.avroRecord.Schema.Fields.GetEnumerator();
                    return this.UpdateState();
                case ODataParameterReaderState.Value:
                case ODataParameterReaderState.Resource:
                case ODataParameterReaderState.ResourceSet:
                    return this.UpdateState();
                default:
                    throw new ApplicationException("Invalid state " + this.State);
            }
        }

        private bool UpdateState()
        {
            if (!this.enumerator.MoveNext())
            {
                this.state = ODataParameterReaderState.Completed;
                return false;
            }

            var parameter = this.operation.Parameters.SingleOrDefault(para => para.Name == this.enumerator.Current.Name);
            if (parameter == null)
            {
                throw new ApplicationException("Given parameter name " + this.enumerator.Current.Name + " not found.");
            }

            this.name = parameter.Name;
            this.value = this.avroRecord[this.enumerator.Current.Position];
            if (this.value is AvroRecord)
            {
                if (parameter.Type.Definition is IEdmStructuredType)
                {
                    this.state = ODataParameterReaderState.Resource;
                }
                else
                {
                    this.state = ODataParameterReaderState.Exception;
                    return false;
                }
            }
            else if (this.value is Array && !(this.value is byte[]))
            {
                this.state = ODataParameterReaderState.ResourceSet;
            }
            else
            {
                this.state = ODataParameterReaderState.Value;
            }

            return true;
        }

        public override System.Threading.Tasks.Task<bool> ReadAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif