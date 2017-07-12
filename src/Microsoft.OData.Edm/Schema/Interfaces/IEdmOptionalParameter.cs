//---------------------------------------------------------------------
// <copyright file="IEdmOptionalParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an optional parameter of an EDM operation.
    /// </summary>
    public interface IEdmOptionalParameter : IEdmOperationParameter
    {
        /// <summary>
        /// Gets the default value of this property.
        /// </summary>
        string DefaultValueString { get; }
    }
}
