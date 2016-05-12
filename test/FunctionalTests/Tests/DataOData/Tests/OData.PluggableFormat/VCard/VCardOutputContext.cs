//---------------------------------------------------------------------
// <copyright file="VCardOutputContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.PluggableFormat.VCard
{
    using System;
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

        public override void WriteProperty(ODataProperty property)
        {
            var val = property.Value as ODataComplexValue;
            if (val == null)
            {
                throw new ApplicationException("only support write complex type property.");
            }

            this.writer.WriteStart();
            foreach (ODataProperty prop in val.Properties)
            {
                string name = null;
                string @params = string.Empty;

                int idx = prop.Name.IndexOf('_');
                if (idx < 0)
                {
                    name = prop.Name;
                }
                else
                {
                    name = prop.Name.Substring(0, idx);
                    @params = string.Join(";", prop.Name.Substring(idx + 1).Split('_'));
                }

                foreach (ODataInstanceAnnotation anns in prop.InstanceAnnotations)
                {
                    @params += ";" + anns.Name.Substring(6) /*VCARD.*/ + "=" + ((ODataPrimitiveValue)anns.Value).Value;
                }

                this.writer.WriteItem(null, name, @params, (string)prop.Value);
            }

            this.writer.WriteEnd();
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
