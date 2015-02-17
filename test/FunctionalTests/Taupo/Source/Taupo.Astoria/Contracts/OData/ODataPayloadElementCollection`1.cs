//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementCollection`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Abstract base class for a generic collection of payload elements
    /// </summary>
    /// <typeparam name="TItem">The type of the individual items, must derive from PayloadElement</typeparam>
    public abstract class ODataPayloadElementCollection<TItem> : ODataPayloadElementCollection, IList<TItem>, IEnumerable<TItem> where TItem : ODataPayloadElement
    {
        /// <summary>
        /// Private storage for the list
        /// </summary>
        private List<TItem> list;
        
        /// <summary>
        /// Initializes a new instance of the ODataPayloadElementCollection class
        /// </summary>
        /// <param name="list">The initial set of items for the collection</param>
        protected ODataPayloadElementCollection(params TItem[] list)
            : base()
        {
            if (list != null)
            {
                this.list = new List<TItem>(list);
            }
            else
            {
                this.list = new List<TItem>();
            }
        }

        /// <summary>
        /// Gets the count of items in the collection
        /// </summary>
        public int Count
        {
            get { return this.list.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether or not this collection is not read only
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a string representation of the element to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", this.ElementType, this.Count);
            }
        }

        /// <summary>
        /// Indexes into the collection to the given position. The setter will update
        /// </summary>
        /// <param name="index">The position to index to</param>
        /// <returns>The item at the given position</returns>
        public TItem this[int index]
        {
            get
            {
                return this.list[index];
            }

            set
            {
                // we let list[i] deal with any out-of-range issues
                this.list[index] = value;
            }
        }

        /// <summary>
        /// Adds the given item to the collection, and sets the collection as its parent
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Add(TItem item)
        {
            this.list.Add(item);
        }

        /// <summary>
        /// Removes all items from the collection, and sets the parent of each item to null
        /// </summary>
        public void Clear()
        {
            this.list.Clear();
        }

        /// <summary>
        /// Checks if the given item is in the collection
        /// </summary>
        /// <param name="item">The item to look for</param>
        /// <returns>True if the item is in the collection, false otherwise</returns>
        public bool Contains(TItem item)
        {
            return this.list.Contains(item);
        }

        /// <summary>
        /// Not implemented, do not invoke
        /// </summary>
        /// <param name="array">The array to copy to</param>
        /// <param name="arrayIndex">The index to start at</param>
        public void CopyTo(TItem[] array, int arrayIndex)
        {
            this.list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the enumerator for the collection
        /// </summary>
        /// <returns>An enumerator</returns>
        public IEnumerator<TItem> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator for the collection
        /// </summary>
        /// <returns>An enumerator</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        /// <summary>
        /// Returns the index of the given item
        /// </summary>
        /// <param name="item">The item to look for</param>
        /// <returns>The index of the given item</returns>
        public int IndexOf(TItem item)
        {
            return this.list.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item at the given position, and sets the collection as the item's parent
        /// </summary>
        /// <param name="index">The index to insert at</param>
        /// <param name="item">The item to insert</param>
        public void Insert(int index, TItem item)
        {
            this.list.Insert(index, item);
        }

        /// <summary>
        /// Removes the given item from the collection, and sets its parent to null
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True if the item was removed, false otherwise</returns>
        public bool Remove(TItem item)
        {
            return this.list.Remove(item);
        }

        /// <summary>
        /// Removes the element at the given index, and sets its parent to null
        /// </summary>
        /// <param name="index">The index of the item to remove</param>
        public void RemoveAt(int index)
        {
            this.list.RemoveAt(index);
        }
        
        /// <summary>
        /// Adds the given element to the collection. Throws an invalid operation exception if the type does not match.
        /// </summary>
        /// <param name="element">The element to add</param>
        public override void Add(ODataPayloadElement element)
        {
            Type itemType = typeof(TItem);
            Type newElementType = element.GetType();

            ExceptionUtilities.Assert(
                newElementType.Equals(itemType) || (itemType.IsAbstract() && itemType.IsAssignableFrom(newElementType)),
                "Cannot add element of type '" + newElementType.Name + "' to a collection of type '" + itemType.Name + "'");

            this.Add(element as TItem);
        }
    }
}
