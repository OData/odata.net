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
    using System.Diagnostics;
    using Microsoft.Hadoop.Avro;
    using Microsoft.OData.Core;

    internal class ODataAvroReader : ODataReader
    {
        private readonly AvroReader reader;
        private readonly bool readingFeed;
        private object currentObject;
        private ODataItem item;
        private IEnumerator recordEnumerator;
        private ODataReaderState state;

        public ODataAvroReader(ODataAvroInputContext inputContext, bool readingFeed, object currentObject = null)
        {
            Debug.Assert(inputContext != null, "inputContext != null");

            this.reader = inputContext.AvroReader;
            this.readingFeed = readingFeed;
            this.currentObject = currentObject;

            this.item = null;
            this.recordEnumerator = null;
            this.state = ODataReaderState.Start;
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

                    this.state = readingFeed
                        ? ODataReaderState.FeedStart
                        : ODataReaderState.EntryStart;
                    break;
                case ODataReaderState.FeedStart:
                    this.item = new ODataFeed();
                    var objs = this.currentObject as object[];
                    if (objs == null)
                    {
                        this.state = ODataReaderState.Exception;
                        return false;
                    }

                    this.recordEnumerator = objs.GetEnumerator();

                    this.state = (this.recordEnumerator.MoveNext())
                        ? ODataReaderState.EntryStart
                        : ODataReaderState.FeedEnd;
                    break;
                case ODataReaderState.FeedEnd:
                    this.state = ODataReaderState.Completed;
                    return false;
                case ODataReaderState.EntryStart:
                    var record = (this.readingFeed ? this.recordEnumerator.Current : this.currentObject) as AvroRecord;
                    if (record == null)
                    {
                        this.state = ODataReaderState.Exception;
                        return false;
                    }

                    this.item = ODataAvroConvert.ToODataEntry(record);
                    this.state = ODataReaderState.EntryEnd;
                    return true;
                case ODataReaderState.EntryEnd:
                    if (!readingFeed)
                    {
                        this.state = ODataReaderState.Completed;
                        return false;
                    }

                    this.state = this.recordEnumerator.MoveNext()
                        ? ODataReaderState.EntryStart
                        : ODataReaderState.FeedEnd;
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
    }
}
#endif