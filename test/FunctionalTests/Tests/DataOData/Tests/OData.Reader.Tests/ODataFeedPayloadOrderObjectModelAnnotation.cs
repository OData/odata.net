//---------------------------------------------------------------------
// <copyright file="ODataFeedPayloadOrderObjectModelAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.OData;
    #endregion Namespaces

    /// <summary>
    /// An OData object model annotation for feeds to capture order in which payload was read.
    /// </summary>
    public sealed class ODataFeedPayloadOrderObjectModelAnnotation
    {
        /// <summary>
        /// List of names of payload items in the order they were found in the payload.
        /// </summary>
        List<string> payloadItems = new List<string>();

        /// <summary>
        /// List of names of payload items in the order they were found in the payload.
        /// </summary>
        public IEnumerable<string> PayloadItems
        {
            get { return this.payloadItems; }
        }

        /// <summary>
        /// Adds a property on the feed to the end of the payload order items.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        public void AddFeedProperty(string propertyName)
        {
            this.AddItem(propertyName);
        }

        /// <summary>
        /// Adds an item representing the end of the StartFeed state.
        /// </summary>
        public void AddStartFeed()
        {
            this.AddItem("__StartFeed__");
        }

        /// <summary>
        /// Adds an item representing an entry in the feed.
        /// </summary>
        public void AddEntry(ODataResource entry)
        {
            this.AddItem("Entry_" + entry.Id);
        }

        /// <summary>
        /// Adds an item into the payload order.
        /// </summary>
        /// <param name="itemName">The item name to add.</param>
        public void AddItem(string itemName)
        {
            if (!this.payloadItems.Contains(itemName))
            {
                this.payloadItems.Add(itemName);
            }
        }
    }
}
