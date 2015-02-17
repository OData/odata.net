//---------------------------------------------------------------------
// <copyright file="NavigationPropertyCardinalityAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    #endregion Namespaces

    /// <summary>
    /// An annotation which adds the information about cardinality to a navigation property.
    /// </summary>
    public class NavigationPropertyCardinalityAnnotation : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// true if the navigation property is a collection, false if it's a singleton, null if it's not known.
        /// </summary>
        public bool? IsCollection { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return "Navigation property cardinality: " + (this.IsCollection.HasValue ? (this.IsCollection.Value ? "Collection" : "Singleton") : "Unknown");
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new NavigationPropertyCardinalityAnnotation { IsCollection = this.IsCollection };
        }
    }
}
