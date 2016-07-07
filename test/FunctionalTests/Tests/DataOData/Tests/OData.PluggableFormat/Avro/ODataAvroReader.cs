//---------------------------------------------------------------------
// <copyright file="ODataAvroReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ENABLE_AVRO
namespace Microsoft.Test.OData.PluggableFormat.Avro
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Hadoop.Avro;
    using Microsoft.OData;

    internal class ODataAvroReader : ODataReader
    {
        private readonly AvroReader reader;
        private readonly bool readingFeed;
        private object currentObject;
        private ODataItem item;
        private IEnumerator recordEnumerator;
        private ODataReaderState state;
        private Stack<ODataItem> scopes;

        public ODataAvroReader(ODataAvroInputContext inputContext, bool readingFeed, object currentObject = null)
        {
            Debug.Assert(inputContext != null, "inputContext != null");

            this.reader = inputContext.AvroReader;
            this.readingFeed = readingFeed;
            this.currentObject = currentObject;

            this.item = null;
            this.recordEnumerator = null;
            this.state = ODataReaderState.Start;
            this.scopes = new Stack<ODataItem>();
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
                    if (this.currentObject == null)
                    {
                        if (!this.reader.MoveNext())
                        {
                            this.state = ODataReaderState.Exception;
                            return false;
                        }

                        this.currentObject = this.reader.Current;
                    }

                    if (readingFeed)
                    {
                       this.state = ODataReaderState.ResourceSetStart;
                    }
                    else
                    {
                        this.state = ODataReaderState.ResourceStart;
                        var record = this.currentObject as AvroRecord;

                        return (ReadResourceFromRecord(this.currentObject));
                    }
                    break;
                case ODataReaderState.ResourceSetStart:
                    {
                        this.item = new ODataResourceSet();
                        scopes.Push(this.item);
                        var objs = this.currentObject as object[];
                        if (objs == null)
                        {
                            this.state = ODataReaderState.Exception;
                            return false;
                        }

                        this.recordEnumerator = objs.GetEnumerator();

                        return MoveToNextAndReadResource();
                    }
                case ODataReaderState.ResourceSetEnd:
                    this.state = ODataReaderState.Completed;
                    return false;
                case ODataReaderState.ResourceStart:
                    {
                        var nestedInfo = this.scopes.Count > 0 ? this.scopes.Peek() as ODataNestedResourceInfo : null;
                        this.scopes.Push(this.item);
                        if (nestedInfo != null)
                        {
                            this.state = ODataReaderState.ResourceEnd;
                            return true;
                        }

                        var record = (this.readingFeed ? this.recordEnumerator.Current : this.currentObject) as AvroRecord;
                        
                        nestedInfo = ODataAvroConvert.GetNestedResourceInfo(record);
                        if (nestedInfo != null)
                        {
                            this.item = nestedInfo;
                            this.state = ODataReaderState.NestedResourceInfoStart;
                        }
                        else
                        {
                            this.state = ODataReaderState.ResourceEnd;
                        }
                        return true;
                    }
                case ODataReaderState.ResourceEnd:
                    {
                        this.scopes.Pop();

                        var nestedInfo = this.scopes.Count > 0 ? this.scopes.Peek() as ODataNestedResourceInfo : null;
                        if (nestedInfo != null)
                        {
                            this.item = nestedInfo;
                            this.state = ODataReaderState.NestedResourceInfoEnd;
                            return true;
                        }

                        if (!readingFeed)
                        {
                            this.state = ODataReaderState.Completed;
                            return false;
                        }

                        return MoveToNextAndReadResource();
                    }
                case ODataReaderState.NestedResourceInfoStart:
                    {
                        this.scopes.Push(this.item);
                        var record = (this.readingFeed ? this.recordEnumerator.Current : this.currentObject) as AvroRecord;
                        var nestedInfo = this.item as ODataNestedResourceInfo;
                        this.item = ODataAvroConvert.ToODataEntry(record, nestedInfo.Name);
                        this.state = ODataReaderState.ResourceStart;
                    }
                    break;
                case ODataReaderState.NestedResourceInfoEnd:
                    this.scopes.Pop();
                    this.item = this.scopes.Peek();
                    this.state = ODataReaderState.ResourceEnd;
                    break;
                default:
                    throw new ApplicationException("Invalid state");
            }

            return true;
        }

        public override System.Threading.Tasks.Task<bool> ReadAsync()
        {
            throw new System.NotImplementedException();
        }

        private bool MoveToNextAndReadResource()
        {
            if (this.recordEnumerator.MoveNext())
            {
                this.state = ODataReaderState.ResourceStart;
                ReadResourceFromRecord(this.recordEnumerator.Current);
            }
            else
            {
                this.state = ODataReaderState.ResourceSetEnd;
            }

            return true;
        }

        private bool ReadResourceFromRecord(object record)
        {
            this.item = ODataAvroConvert.ToODataEntry(record as AvroRecord);
            if (record == null)
            {
                this.state = ODataReaderState.Exception;
                return false;
            }
            return true;
        }
    }
}
#endif