//---------------------------------------------------------------------
// <copyright file="CollectionKindAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Annotation that contains information about collection kind of a member property
    /// </summary>
    public class CollectionKindAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the CollectionKindAnnotation class.
        /// </summary>
        /// <param name="kind">The collection kind.</param>
        public CollectionKindAnnotation(CollectionKind kind)
        {
            this.CollectionKind = kind;
        }

        /// <summary>
        /// Gets the collection kind
        /// </summary>
        public CollectionKind CollectionKind { get; private set; }
    }
}
