//---------------------------------------------------------------------
// <copyright file="IDataServiceEntityFrameworkProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces
    using System.Collections.Generic;
    #endregion

    /// <summary>Schema version compliance of the metadata.</summary>
    public enum MetadataEdmSchemaVersion
    {
        /// <summary>EDM v4.0 compliant</summary>
        Version4Dot0,
    }

    /// <summary>
    /// Interface used for the discovery of properties that will be part of the ETag for entities belonging
    /// to the given resource set and resource type, also used for obtaining metadata EDM version.
    /// </summary>
    public interface IDataServiceEntityFrameworkProvider
    {
        /// <summary>
        /// Gets the metadata schema version.
        /// </summary>
        MetadataEdmSchemaVersion EdmSchemaVersion
        {
            get;
        }

        /// <summary>
        /// Given a resource container and resource type, gets the list of ResourceProperties that
        /// are part of the ETag.
        /// </summary>
        /// <param name="containerName">Resource set name.</param>
        /// <param name="resourceType">Resource type of entities in the resource container.</param>
        /// <returns>Collection of properties that are part of the ETag.</returns>
        IList<ResourceProperty> GetETagProperties(string containerName, ResourceType resourceType);
    }
}
