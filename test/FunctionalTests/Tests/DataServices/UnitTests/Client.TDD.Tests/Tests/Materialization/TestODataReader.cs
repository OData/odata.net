//---------------------------------------------------------------------
// <copyright file="TestODataReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData;
    using System.Threading.Tasks;

    /// <summary>
    /// Used to mimic an ODataReader, allows testing of components that previously required a full ODataReader
    /// </summary>
    internal class TestODataReader : ODataReader, IEnumerable
    {
        private ODataReaderState currentState;
        private ODataItem currentItem;
        private IEnumerator<TestODataReaderItem> enumerator;
        private List<TestODataReaderItem> items;

        public TestODataReader()
        {
            this.items = new List<TestODataReaderItem>();
            this.currentState = ODataReaderState.Start;
            this.ReadFunc = this.DefaultRead;
        }

        public override ODataReaderState State
        {
            get { return this.currentState; }
        }

        public override ODataItem Item
        {
            get { return this.currentItem; }
        }

        internal Func<bool> ReadFunc { get; set; }

        /// <summary>
        /// Adds given navigation property to <see cref="NavigationProperties"/> collection.
        /// </summary>
        /// <param name="prop">Navigation property to add.</param>
        /// <remarks>Used as part of the IEnumerable to make building these via code easy</remarks>
        public void Add(TestODataReaderItem item)
        {
            this.items.Add(item);
        }

        public override bool Read()
        {
            return this.ReadFunc();
        }

        public IEnumerator GetEnumerator()
        {
            // Implements IEnumerable so that we can add Items via Add above, not required to implement the read side
            throw new System.NotImplementedException();
        }

        public override Task<bool> ReadAsync()
        {
            // not needed as the client doesn't use this part of the api
            throw new System.NotImplementedException();
        }

        private bool DefaultRead()
        {
            if (this.enumerator == null)
            {
                this.enumerator = this.items.GetEnumerator();
            }

            bool hasRead = this.enumerator.MoveNext();
            if (hasRead)
            {
                this.currentState = this.enumerator.Current.State;
                this.currentItem = this.enumerator.Current.Item;
            }
            else
            {
                this.currentState = ODataReaderState.Completed;
                this.currentItem = null;
            }

            return hasRead;
        }
    }

    internal class TestODataReaderItem
    {
        public TestODataReaderItem(ODataReaderState state, ODataItem item)
        {
            this.State = state;
            this.Item = item;
        }

        public ODataReaderState State { get; private set; }

        public ODataItem Item { get; private set; }
    }
}
