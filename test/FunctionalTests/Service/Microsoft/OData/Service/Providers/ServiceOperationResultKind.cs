//---------------------------------------------------------------------
// <copyright file="ServiceOperationResultKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    /// <summary>
    /// Use this type to describe the kind of results returned by a service
    /// operation.
    /// </summary>
    public enum ServiceOperationResultKind
    {
        /// <summary>
        /// A single direct value which cannot be further composed.
        /// </summary>
        DirectValue,

        /// <summary>
        /// An enumeration of values which cannot be further composed.
        /// </summary>
        Enumeration,

        /// <summary>
        /// A queryable object which returns multiple elements.
        /// </summary>
        QueryWithMultipleResults,

        /// <summary>
        /// A queryable object which returns a single element.
        /// </summary>
        QueryWithSingleResult,

        /// <summary>
        /// No result return.
        /// </summary>
        Void,
    }
}
