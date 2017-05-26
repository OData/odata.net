//---------------------------------------------------------------------
// <copyright file="IEdmUnknownEntitySet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM unknown entity set.
    /// Unknown entity set can appear in following scenarios:
    /// 1. The target of navigation property is contained in other entity.
    /// 2. The target of navigation property comes from more than one entity set.
    /// 3. Other scenarios that the entity set is unknown.
    /// </summary>
    public interface IEdmUnknownEntitySet : IEdmEntitySetBase
    {
    }
}
