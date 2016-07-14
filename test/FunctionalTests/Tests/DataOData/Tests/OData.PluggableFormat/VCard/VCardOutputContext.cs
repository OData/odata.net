//---------------------------------------------------------------------
// <copyright file="VCardOutputContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.PluggableFormat.VCard
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Microsoft.OData;

    internal class VCardOutputContext : ODataOutputContext
    {
        private Stream outputStream;
        private VCardWriter writer;

        internal VCardOutputContext(
            ODataFormat format,
            ODataMessageInfo messageInfo,
            ODataMessageWriterSettings messageWriterSettings)
            : base(format, messageInfo, messageWriterSettings)
        {
            this.writer = new VCardWriter(new StreamWriter(messageInfo.MessageStream, messageInfo.Encoding));
            this.outputStream = messageInfo.MessageStream;
        }

        public VCardWriter VCardWriter
        {
            get { return this.writer; }
        }

        public override ODataWriter CreateODataResourceWriter(Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmStructuredType resourceType)
        {
            return new ODataVCardWriter(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (this.outputStream != null)
                    {
                        this.outputStream.Flush();
                        this.outputStream.Dispose();
                    }

                }
                finally
                {
                    this.outputStream = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
