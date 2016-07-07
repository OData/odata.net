//---------------------------------------------------------------------
// <copyright file="ODataAvroWriter.cs" company="Microsoft">
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

    /// <summary>
    ///  The intermediate class between OData instance and Avro objects
    /// </summary>
    internal class ODataAvroWriter : ODataWriter
    {
        private readonly ODataAvroOutputContext outputContext;
        private readonly Action<object> writeAction;
        private Schema schema;
        private AvroRecord currentEntityObject;
        private IList<AvroRecord> entityObjectList;
        private bool writingFeed;

        private Stack<ODataItem> scopes = new Stack<ODataItem>();

        public ODataAvroWriter(ODataAvroOutputContext outputContext, Action<object> writeAction, Schema schema, bool writingFeed)
        {
            Debug.Assert(outputContext != null, "outputContext != null");
            Debug.Assert(writeAction != null, "flushAction != null");

            this.outputContext = outputContext;
            this.writeAction = writeAction;
            this.schema = schema;
            this.writingFeed = writingFeed;
            if (writingFeed)
            {
                this.entityObjectList = new List<AvroRecord>();
            }
        }

        public override void WriteStart(ODataResourceSet resourceCollection)
        {
        }

        public override System.Threading.Tasks.Task WriteStartAsync(ODataResourceSet resourceCollection)
        {
            throw new System.NotImplementedException();
        }

        public override void WriteStart(ODataResource entry)
        {
            this.WriteEntryImplementation(entry);
        }

        public override System.Threading.Tasks.Task WriteStartAsync(ODataResource entry)
        {
            throw new System.NotImplementedException();
        }

        public override void WriteStart(ODataNestedResourceInfo navigationLink)
        {
            this.scopes.Push(navigationLink);
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
            if (this.writingFeed)
            {
                this.writeAction(this.entityObjectList.ToArray());
            }
            else
            {
                this.writeAction(this.currentEntityObject);
            }

            this.outputContext.Flush();
        }

        public override System.Threading.Tasks.Task FlushAsync()
        {
            throw new System.NotImplementedException();
        }

        private void WriteEntryImplementation(ODataResource entry)
        {
            if (this.schema == null)
            {
                this.schema = this.outputContext.AvroWriter.UpdateSchema(entry, null, this.writingFeed);
            }

            if (this.scopes.Count > 0)
            {
                var parent = this.scopes.Pop() as ODataNestedResourceInfo;
                var obj = this.currentEntityObject;
                ODataAvroConvert.UpdateNestedInfoFromODataObject(obj, entry, parent, schema);
            }
            else
            {
                var obj = (AvroRecord)ODataAvroConvert.FromODataObject(entry, this.schema);

                if (this.writingFeed)
                {
                    this.entityObjectList.Add(obj);
                    this.currentEntityObject = obj;
                }
                else
                {
                    this.currentEntityObject = obj;
                }
            }
        }
    }
}
#endif