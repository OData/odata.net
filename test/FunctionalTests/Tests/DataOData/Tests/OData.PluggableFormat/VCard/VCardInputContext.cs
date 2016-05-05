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

        public VCardInputContext(ODataFormat format,
            Stream messageStream,
            ODataMediaType contentType,
            Encoding encoding,
            ODataMessageReaderSettings messageReaderSettings,
            bool readingResponse,
            bool synchronous,
            IEdmModel model,
            IODataUrlResolver urlResolver)
            : base(format, messageReaderSettings, readingResponse, synchronous, model, urlResolver, /*container*/null)
        {
            this.stream = messageStream;
            this.reader = new VCardReader(new StreamReader(messageStream, encoding));
            this.throwExceptionOnDuplicatedPropertyNames = false;
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

        private ODataProperty ReadPropertyImplementation()
        {
            List<VCardItem> items = new List<VCardItem>();
            this.reader.Read();
            this.reader.Read();
            while (this.reader.State == VCardReaderState.Item)
            {
                VCardItem item = new VCardItem()
                {
                    Name = this.reader.Name,
                    Value = this.reader.Value,
                    Groups = this.reader.Groups,
                };

                if (this.reader.Params != null)
                {
                    item.Params = new Dictionary<string, string>();
                    foreach (string param in this.reader.Params.Split(';'))
                    {
                        int idx = param.IndexOf('=');
                        if (idx >= 0)
                        {
                            // need to care about when = appear in last.
                            item.Params.Add(param.Substring(0, idx), param.Substring(idx + 1));
                        }
                        else
                        {
                            if (item.Params.ContainsKey("TYPE"))
                            {
                                item.Params["TYPE"] = item.Params["TYPE"] + ";" + param;
                            }
                            else
                            {
                                item.Params.Add("TYPE", param);
                            }
                        }
                    }
                }

                items.Add(item);
                this.reader.Read();
            }

            Debug.Assert(this.reader.State == VCardReaderState.End);

            return new ODataProperty() { Name = "fake", Value = GetEntryFromItems(items) };
        }

        public override ODataProperty ReadProperty(IEdmStructuralProperty property, IEdmTypeReference expectedPropertyTypeReference)
        {
            return this.ReadPropertyImplementation();
        }

        public override Task<ODataProperty> ReadPropertyAsync(IEdmStructuralProperty property, IEdmTypeReference expectedPropertyTypeReference)
        {
            return Task<ODataProperty>.Factory.StartNew(this.ReadPropertyImplementation);
        }

        private ODataComplexValue GetEntryFromItems(IEnumerable<VCardItem> items)
        {
            Dictionary<string, ODataProperty> dic = new Dictionary<string, ODataProperty>();
            foreach (var item in items)
            {
                string propertyName = item.Name;

                List<ODataInstanceAnnotation> annotations = null;

                if (item.Params != null)
                {
                    if (item.Params.ContainsKey("TYPE"))
                    {
                        propertyName += "_" + string.Join("_", item.Params["TYPE"].Split(';'));
                    }

                    annotations = item.Params.Where(_ => _.Key != "TYPE")
                        .Select(param => new ODataInstanceAnnotation("VCARD." + param.Key, new ODataPrimitiveValue(param.Value)))
                        .ToList();
                }

                if (!dic.ContainsKey(propertyName))
                {
                    ODataProperty property = new ODataProperty() { Name = propertyName, Value = item.Value };

                    if (annotations != null)
                    {
                        property.InstanceAnnotations = annotations;
                    }

                    dic.Add(propertyName, property);
                }
                else
                {
                    if (this.throwExceptionOnDuplicatedPropertyNames)
                    {
                        throw new ODataException(string.Format("Duplicate property found:{0}", propertyName));
                    }
                }
            }

            ODataComplexValue val = new ODataComplexValue
            {
                Properties = dic.Values,
                TypeName = "VCard21.VCard"
            };

            return val;
        }
    }
}
