//---------------------------------------------------------------------
// <copyright file="ODataPropertyStreamingContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm;

namespace Microsoft.OData
{
    /// <summary>
    /// Context information for reading an OData property
    /// </summary>
    public class ODataPropertyStreamingContext
    {
        /// <summary>
        /// The primitive type of the property being read, or null if unknown.
        /// </summary>
        public IEdmPrimitiveType PrimitiveType { get; internal set; }

        /// <summary>
        /// Indicates whether the value being read is a collection.
        /// </summary>
        public bool IsCollection { get; internal set; }

        /// <summary>
        /// The name of the property being read (null for values within a collection)
        /// </summary>
        public string PropertyName { get; internal set; }

        /// <summary>
        /// The EDM property being read (null for dynamic property or value within a collection)
        /// </summary>
        public IEdmProperty Property { get; internal set; }

        /// <summary>
        /// The custom annotations associated with this property.
        /// These are annotations that do not correspond to reserved OData annotations,
        /// regardless of whether the optional "odata." prefix is present.
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> CustomPropertyAnnotations { get; internal set; }
    }
}
