//---------------------------------------------------------------------
// <copyright file="ODataEdmPropertyAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData
{
    /// <summary>Represents an annotation to hold information for a particular property.</summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Edm", Justification = "Camel-casing in class name.")]
    public sealed class ODataEdmPropertyAnnotation
    {
        /// <summary> Gets the behavior for readers when reading property with null value. </summary>
        /// <returns>The behavior for readers when reading property with null value.</returns>
        public ODataNullValueBehaviorKind NullValueReadBehaviorKind
        {
            get;
            set;
        }
    }
}
