//---------------------------------------------------------------------
// <copyright file="Endpoints.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// A collection of constants representing the various special URI endpoints of a data service
    /// </summary>
    public static class Endpoints
    {
        /// <summary>
        /// The '$batch' endpoint
        /// </summary>
        public const string Batch = "$batch";

        /// <summary>
        /// The '$count' endpoint
        /// </summary>
        public const string Count = "$count";

        /// <summary>
        /// The '$ref' endpoint
        /// </summary>
        public const string Ref = "$ref";

        /// <summary>
        /// The '$metadata' endpoint
        /// </summary>
        public const string Metadata = "$metadata";

        /// <summary>
        /// The '$value' endpoint
        /// </summary>
        public const string Value = "$value";

        /// <summary>
        /// The '*' endpoint used for selecting all non-navigation properties on a type
        /// </summary>
        public const string SelectAll = "*";
    }
}
