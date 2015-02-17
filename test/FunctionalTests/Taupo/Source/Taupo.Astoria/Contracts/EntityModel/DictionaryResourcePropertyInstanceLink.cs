//---------------------------------------------------------------------
// <copyright file="DictionaryResourcePropertyInstanceLink.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    
    /// <summary>
    /// Marker used to indicate when two properties are equivalent and can be represented by the same instance of ResourceProperty
    /// </summary>
    public class DictionaryResourcePropertyInstanceLink : CompositeAnnotation
    {
        /// <summary>
        /// Gets or sets the unique identifier
        /// </summary>
        public Guid UniqueId { get; set; }
    }
}