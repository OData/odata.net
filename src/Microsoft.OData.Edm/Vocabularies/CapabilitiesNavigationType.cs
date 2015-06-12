//---------------------------------------------------------------------
// <copyright file="CapabilitiesNavigationType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies.V1
{
    /// <summary>
    /// Enumerates the navigation type can apply on navigation restrictions.
    /// </summary>
    public enum CapabilitiesNavigationType
    {
        /// <summary>
        /// Navigation properties can be recursively navigated.
        /// </summary>
        Recursive,

        /// <summary>
        /// Navigation properties can be navigated to a single level.
        /// </summary>
        Single,

        /// <summary>
        /// Navigation properties are not navigable.
        /// </summary>
        None
    }
}
