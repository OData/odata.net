//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
