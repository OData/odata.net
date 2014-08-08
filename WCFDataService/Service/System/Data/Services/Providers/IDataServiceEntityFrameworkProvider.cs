//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services.Providers
{
    #region Namespaces
    using System.Collections.Generic;
    #endregion

    /// <summary>Schema version compliance of the metadata.</summary>
    public enum MetadataEdmSchemaVersion
    {
        /// <summary>EDM v1.0 compliant.</summary>
        Version1Dot0,

        /// <summary>EDM v1.1 compliant.</summary>
        Version1Dot1,

        /// <summary>EDM v1.2 compliant.</summary>
        Version1Dot2,

        /// <summary>EDM v2.0 compliant.</summary>
        Version2Dot0,

        /// <summary>EDM v2.2 compliant.</summary>
        Version2Dot2,

        /// <summary>EDM v3.0 compliant</summary>
        Version3Dot0,
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
