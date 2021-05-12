//---------------------------------------------------------------------
// <copyright file="BaseEntityType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    /// <summary>
    /// Base type of entity type to include <see cref="Microsoft.OData.Client.DataServiceContext" /> for function and action invocation

    /// </summary>
    public interface IBaseEntityType
    {
        /// <summary>
        /// DataServiceContext for query provider.
        /// </summary>
        DataServiceContext Context { get; set; }

        /// <summary>
        /// Entity descriptor containing entity stream links.
        /// </summary>
        EntityDescriptor EntityDescriptor { get; set; }
    }
}
