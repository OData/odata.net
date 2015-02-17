//---------------------------------------------------------------------
// <copyright file="IODataPayloadElementComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for comparing two odata payload elements
    /// </summary>
    [ImplementationSelector("ODataPayloadElementComparer", DefaultImplementation = "Default")]
    public interface IODataPayloadElementComparer
    {
        /// <summary>
        /// Compares the two payload elements
        /// </summary>
        /// <param name="expected">The expected element</param>
        /// <param name="actual">The actual element</param>
        void Compare(ODataPayloadElement expected, ODataPayloadElement actual);
    }
}
