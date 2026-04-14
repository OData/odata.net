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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1033:Interface methods should be callable by child types",
            Justification = "Implemented explicitly to satisfy IBaseEntityType without exposing DataServiceContext on the public API; derived types access the same value through the protected internal Context property.")]
        DataServiceContext IBaseEntityType.DataServiceContext
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
