//---------------------------------------------------------------------
// <copyright file="IODataLibPayloadElementComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Contract for payload element comparers.
    /// </summary>
    [ImplementationSelector("IODataLibPayloadElementComparer", DefaultImplementation = "ODataLibPayloadElementComparer", HelpText = "Comparer to compare to PayloadElement instances.")]
    public interface IODataLibPayloadElementComparer
    {
        /// <summary>
        /// Compares two ODataPayloadElement instances and asserts that they are equal.
        /// </summary>
        /// <param name="expected">The expected payload element.</param>
        /// <param name="observed">The actual, observed payload element.</param>
        void Compare(ODataPayloadElement expected, ODataPayloadElement observed);
    }
}
