//---------------------------------------------------------------------
// <copyright file="ODataVCardWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;
using Microsoft.OData;

namespace Microsoft.Test.OData.PluggableFormat.VCard
{
    internal class ODataVCardWriter : ODataWriter
    {
        private readonly VCardWriter writer;

        public ODataVCardWriter(VCardOutputContext outputContext)
        {
            Debug.Assert(outputContext != null, "outputContext != null");

            this.writer = outputContext.VCardWriter;
        }

        public override void WriteStart(ODataResourceSet resourceSet)
        {
            throw new NotImplementedException();
        }

        public override System.Threading.Tasks.Task WriteStartAsync(ODataResourceSet resourceCollection)
        {
            throw new System.NotImplementedException();
        }

        public override void WriteStart(ODataResource resource)
        {
            this.writer.WriteStart();
            foreach (ODataProperty prop in resource.Properties)
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

                ODataUntypedValue untyped = prop.Value as ODataUntypedValue;
                if (untyped != null)
                {
                    object valueTmp = ParseJsonToPrimitiveValue(untyped.RawValue);
                    this.writer.WriteItem(null, name, @params, valueTmp as string);
                }
                else
                {
                    this.writer.WriteItem(null, name, @params, prop.Value as string);
                }
            }

            this.writer.WriteEnd();
        }

        public override System.Threading.Tasks.Task WriteStartAsync(ODataResource entry)
        {
            throw new System.NotImplementedException();
        }

        public override void WriteStart(ODataNestedResourceInfo navigationLink)
        {
            throw new System.NotImplementedException();
        }

        public override System.Threading.Tasks.Task WriteStartAsync(ODataNestedResourceInfo navigationLink)
        {
            throw new System.NotImplementedException();
        }

        public override void WriteEnd()
        {
        }

        public override System.Threading.Tasks.Task WriteEndAsync()
        {
            throw new System.NotImplementedException();
        }

        public override void WriteEntityReferenceLink(ODataEntityReferenceLink entityReferenceLink)
        {
            throw new System.NotImplementedException();
        }

        public override System.Threading.Tasks.Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink entityReferenceLink)
        {
            throw new System.NotImplementedException();
        }

        public override void Flush()
        {
        }

        public override System.Threading.Tasks.Task FlushAsync()
        {
            throw new System.NotImplementedException();
        }

        public object ParseJsonToPrimitiveValue(string rawValue)
        {
            Debug.Assert(rawValue != null && rawValue.Length > 0, "");
            ODataCollectionValue collectionValue = (ODataCollectionValue)
                Microsoft.OData.ODataUriUtils.ConvertFromUriLiteral(string.Format("[{0}]", rawValue), ODataVersion.V4);
            foreach (object item in collectionValue.Items)
            {
                return item;
            }

            return null;
        }
    }
}