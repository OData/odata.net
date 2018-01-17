//---------------------------------------------------------------------
// <copyright file="ODataAvroInputContext.cs" company="Microsoft">
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
    using Microsoft.OData;
    using Microsoft.OData.Edm;

    internal class ODataAvroInputContext : ODataInputContext
    {
        private Stream stream;

        public ODataAvroInputContext(
            ODataFormat format,
            ODataMessageInfo messageInfo,
            ODataMessageReaderSettings messageReaderSettings)
            : base(format, messageInfo, messageReaderSettings)
        {
            this.stream = messageInfo.MessageStream;

            MemoryStream st = new MemoryStream();
            stream.CopyTo(st);
            st.Seek(0, SeekOrigin.Begin);
            this.AvroReader = new AvroReader(AvroContainer.CreateGenericReader(st));
        }

        public AvroReader AvroReader { get; private set; }

        public override ODataReader CreateResourceReader(IEdmNavigationSource navigationSource, IEdmStructuredType expectedResourceType)
        {
            return new ODataAvroReader(this, false);
        }

        public override ODataReader CreateResourceSetReader(IEdmEntitySetBase entitySet, IEdmStructuredType expectedResourceType)
        {
            return new ODataAvroReader(this, true);
        }

        public override ODataCollectionReader CreateCollectionReader(IEdmTypeReference expectedItemTypeReference)
        {
            return new ODataAvroCollectionReader(this);
        }

        public override ODataParameterReader CreateParameterReader(IEdmOperation operation)
        {
            return new ODataAvroParameterReader(this, operation);
        }

        public override ODataProperty ReadProperty(IEdmStructuralProperty property, IEdmTypeReference expectedPropertyTypeReference)
        {
            if (!this.AvroReader.MoveNext())
            {
                throw new ApplicationException("Error Reader state");
            }

            return new ODataProperty { Value = ODataAvroConvert.ToODataValue(this.AvroReader.Current) };
        }

        public override ODataError ReadError()
        {
            if (!this.AvroReader.MoveNext() || !(this.AvroReader.Current is AvroRecord))
            {
                throw new ApplicationException("Error Reader state");
            }

            dynamic record = this.AvroReader.Current;

            return new ODataError
            {
                ErrorCode = record.ErrorCode,
                Message = record.Message,
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (this.AvroReader != null)
                    {
                        this.AvroReader.Dispose();
                    }

                    if (this.stream != null)
                    {
                        this.stream.Dispose();
                    }

                }
                finally
                {
                    this.AvroReader = null;
                    this.stream = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
#endif