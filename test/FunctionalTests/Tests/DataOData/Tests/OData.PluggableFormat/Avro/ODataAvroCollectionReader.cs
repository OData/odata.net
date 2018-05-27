//---------------------------------------------------------------------
// <copyright file="ODataAvroCollectionReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ENABLE_AVRO
namespace Microsoft.Test.OData.PluggableFormat.Avro
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using Microsoft.OData;

    internal class ODataAvroCollectionReader : ODataCollectionReader
    {
        private readonly AvroReader reader;
        private IEnumerator enumerator;
        private ODataCollectionReaderState state;

        public ODataAvroCollectionReader(ODataAvroInputContext inputContext)
        {
            Debug.Assert(inputContext != null, "inputContext != null");

            this.reader = inputContext.AvroReader;
            this.enumerator = null;
            this.state = ODataCollectionReaderState.Start;
        }

        public override ODataCollectionReaderState State
        {
            get { return state; }
        }

        public override object Item
        {
            get { return this.enumerator == null ? null : this.enumerator.Current; }
        }

        public override bool Read()
        {
            return this.ReadImplementation();
        }

        public override System.Threading.Tasks.Task<bool> ReadAsync()
        {
            throw new System.NotImplementedException();
        }

        private bool ReadImplementation()
        {
            switch (this.state)
            {
                case ODataCollectionReaderState.Start:
                    if (!this.reader.MoveNext())
                    {
                        this.state = ODataCollectionReaderState.Completed;
                        return false;
                    }

                    var collection = this.reader.Current as object[];
                    if (collection == null)
                    {
                        this.state = ODataCollectionReaderState.Exception;
                        return false;
                    }

                    this.enumerator = collection.GetEnumerator();
                    this.state = ODataCollectionReaderState.CollectionStart;
                    break;
                case ODataCollectionReaderState.CollectionStart:
                case ODataCollectionReaderState.Value:
                    this.state = this.enumerator.MoveNext()
                        ? ODataCollectionReaderState.Value
                        : ODataCollectionReaderState.CollectionEnd;
                    break;
                case ODataCollectionReaderState.CollectionEnd:
                    this.state = ODataCollectionReaderState.Completed;
                    return false;
                default:
                    throw new ApplicationException("Invalid reader state.");
            }

            return true;
        }
    }
}
#endif