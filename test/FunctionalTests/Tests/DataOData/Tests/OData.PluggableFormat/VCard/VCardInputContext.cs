//---------------------------------------------------------------------
// <copyright file="VCardInputContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.PluggableFormat.VCard
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.OData;
    using Microsoft.OData.Edm;

    internal class VCardInputContext : ODataInputContext
    {
        private bool throwExceptionOnDuplicatedPropertyNames;

        private VCardReader reader;

        private Stream stream;

        public VCardInputContext(
            ODataFormat format,
            ODataMessageInfo messageInfo,
            ODataMessageReaderSettings messageReaderSettings)
            : base(format, messageInfo, messageReaderSettings)
        {
            this.stream = messageInfo.MessageStream;
            this.reader = new VCardReader(new StreamReader(messageInfo.MessageStream, messageInfo.Encoding));
            this.throwExceptionOnDuplicatedPropertyNames = false;
        }

        public bool ThrowExceptionOnDuplicatedPropertyNames
        {
            get { return throwExceptionOnDuplicatedPropertyNames; }
        }

        public VCardReader VCardReader
        {
            get { return this.reader; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (stream != null)
                    {
                        stream.Dispose();
                    }
                }
                finally
                {
                    stream = null;
                }
            }

            base.Dispose(disposing);
        }

        public override ODataReader CreateResourceReader(IEdmNavigationSource navigationSource, IEdmStructuredType expectedResourceType)
        {
            return new ODataVCardReader(this);
        }

        public override Task<ODataReader> CreateResourceReaderAsync(IEdmNavigationSource navigationSource, IEdmStructuredType expectedResourceType)
        {
            return Task<ODataReader>.Factory.StartNew(() => new ODataVCardReader(this));
        }
    }
}
