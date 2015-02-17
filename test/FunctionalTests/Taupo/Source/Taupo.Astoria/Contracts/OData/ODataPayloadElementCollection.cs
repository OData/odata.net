//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementCollection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Abstract base class for an arbitrary collection of payload elements
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Type represents a collection")]
    public abstract class ODataPayloadElementCollection : ODataPayloadElement
    {
        /// <summary>
        /// Adds an element to the collection
        /// </summary>
        /// <param name="element">The element to add</param>
        public abstract void Add(ODataPayloadElement element);
    }
}
