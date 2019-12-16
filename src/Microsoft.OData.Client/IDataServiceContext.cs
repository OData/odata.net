//---------------------------------------------------------------------
// <copyright file="IDataServiceContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    /// <summary>
    /// The <see cref="T:Microsoft.OData.Client.IDataServiceContext" /> represents the runtime context of the data service.
    /// </summary>
    public interface IDataServiceContext
    {

        /// <summary>Adds the specified object to the set of objects that the context is tracking.</summary>
        /// <param name="entitySetName">The name of the entity set to which the resource will be added.</param>
        /// <param name="entity">The object to be tracked by the context.</param>
        void AddObject(string entitySetName, object entity);

        /// <summary>Changes the state of the specified object to be deleted in the context class.</summary>
        /// <param name="entity">The tracked entity to be changed to the deleted state.</param>
        void DeleteObject(object entity);

        /// <summary>Changes the state of the specified object in the context class to <see cref="F:Microsoft.OData.Client.EntityStates.Modified" />.</summary>
        /// <param name="entity">The tracked entity to be assigned to the <see cref="F:Microsoft.OData.Client.EntityStates.Modified" /> state.</param>
        void UpdateObject(object entity);
    }
}