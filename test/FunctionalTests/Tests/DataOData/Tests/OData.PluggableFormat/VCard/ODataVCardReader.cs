//---------------------------------------------------------------------
// <copyright file="ODataVCardReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData;

namespace Microsoft.Test.OData.PluggableFormat.VCard
{
    internal class ODataVCardReader : ODataReader
    {
        private readonly VCardReader reader;
        private ODataItem item;
        private ODataReaderState state;
        private bool throwExceptionOnDuplicatedPropertyNames;

        public ODataVCardReader(VCardInputContext inputContext)
        {
            Debug.Assert(inputContext != null, "inputContext != null");

            this.reader = inputContext.VCardReader;
            this.item = null;
            this.state = ODataReaderState.Start;
            this.throwExceptionOnDuplicatedPropertyNames = inputContext.ThrowExceptionOnDuplicatedPropertyNames;
        }

        public override ODataReaderState State
        {
            get { return state; }
        }

        public override ODataItem Item
        {
            get { return item; }
        }

        public override bool Read()
        {
            switch (this.state)
            {
                case ODataReaderState.Start:
                    List<VCardItem> items = new List<VCardItem>();
                    this.reader.Read();
                    this.reader.Read();
                    while (this.reader.State == VCardReaderState.Item)
                    {
                        VCardItem vCardItem = new VCardItem()
                        {
                            Name = this.reader.Name,
                            Value = this.reader.Value,
                            Groups = this.reader.Groups,
                        };

                        if (this.reader.Params != null)
                        {
                            vCardItem.Params = new Dictionary<string, string>();
                            foreach (string param in this.reader.Params.Split(';'))
                            {
                                int idx = param.IndexOf('=');
                                if (idx >= 0)
                                {
                                    // need to care about when = appear in last.
                                    vCardItem.Params.Add(param.Substring(0, idx), param.Substring(idx + 1));
                                }
                                else
                                {
                                    if (vCardItem.Params.ContainsKey("TYPE"))
                                    {
                                        vCardItem.Params["TYPE"] = vCardItem.Params["TYPE"] + ";" + param;
                                    }
                                    else
                                    {
                                        vCardItem.Params.Add("TYPE", param);
                                    }
                                }
                            }
                        }

                        items.Add(vCardItem);
                        this.reader.Read();
                    }

                    Debug.Assert(this.reader.State == VCardReaderState.End);

                    this.item = GetEntryFromItems(items);
                    this.state = ODataReaderState.ResourceEnd;
                    break;
                case ODataReaderState.ResourceEnd:
                    this.state = ODataReaderState.Completed;
                    return false;
                default:
                    throw new ApplicationException("Invalid state");
            }

            return true;
        }

        public override sealed Task<bool> ReadAsync()
        {
            return Task<bool>.Factory.StartNew(this.Read);
        }

        private ODataResource GetEntryFromItems(IEnumerable<VCardItem> items)
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

            ODataResource val = new ODataResource
            {
                Properties = dic.Values,
                TypeName = "VCard21.VCard"
            };

            return val;
        }
    }
}
