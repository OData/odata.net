//---------------------------------------------------------------------
// <copyright file="IResourceLookup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Common
{
    /// <summary>
    /// Locates resources
    /// </summary>
    public interface IResourceLookup
    {
        /// <summary>
        /// Finds a specific string resource
        /// </summary>
        /// <param name="resourceKey">Key of the resource to be located</param>
        /// <returns>The localized resource value</returns>
        string LookupString(string resourceKey);
    }
}
