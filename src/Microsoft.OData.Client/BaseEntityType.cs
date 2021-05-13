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
    public class BaseEntityType : IBaseEntityType
    {
        /// <summary>
        /// DataServiceContext for query provider.
        /// </summary>
        protected internal DataServiceContext Context { get; set; }

        DataServiceContext IBaseEntityType.Context
        {
            get => Context;
            set => Context = value;
        }

        /// <summary>
        /// Entity descriptor containing entity stream links.
        /// </summary>
        protected internal EntityDescriptor EntityDescriptor { get; set; }

        EntityDescriptor IBaseEntityType.EntityDescriptor
        {
            get => EntityDescriptor;
            set => EntityDescriptor = value;
        }
    }
}
