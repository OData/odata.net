//---------------------------------------------------------------------
// <copyright file="ODataEntryPayloadOrderObjectModelAnnotation.cs" company="Microsoft">
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
    /// An OData object model annotation for entries to capture order in which payload was read.
    /// </summary>
    public sealed class ODataEntryPayloadOrderObjectModelAnnotation
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
        /// Adds a property on the entry to the end of the payload order items.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        public void AddEntryProperty(string propertyName)
        {
            this.AddItem(propertyName);
        }

        /// <summary>
        /// Adds an ODataProperty to the end of the payload order items.
        /// </summary>
        /// <param name="property">The property.</param>
        public void AddODataProperty(ODataProperty property)
        {
            this.AddItem("Property_" + property.Name);
        }

        /// <summary>
        /// Adds an ODataNestedResourceInfo to the end of the payload order items.
        /// </summary>
        /// <param name="navigationLink">The navigation link.</param>
        public void AddODataNavigationLink(ODataNestedResourceInfo navigationLink)
        {
            this.AddItem("NavigationLink_" + navigationLink.Name);
        }

        /// <summary>
        /// Adds an ODataAction to the end of the payload order items.
        /// </summary>
        /// <param name="action">The action.</param>
        public void AddAction(ODataAction action)
        {
            this.AddItem("Action_" + action.Metadata.OriginalString);
        }

        /// <summary>
        /// Adds an ODataFunction to the end of the payload order items.
        /// </summary>
        /// <param name="function">The function.</param>
        public void AddFunction(ODataFunction function)
        {
            this.AddItem("Function_" + function.Metadata.OriginalString);
        }

        /// <summary>
        /// Adds an item representing the end of the StartResource state.
        /// </summary>
        public void AddStartResource()
        {
            this.AddItem("__StartResource__");
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
