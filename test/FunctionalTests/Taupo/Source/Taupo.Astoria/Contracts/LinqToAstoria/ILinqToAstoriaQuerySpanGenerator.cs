//---------------------------------------------------------------------
// <copyright file="ILinqToAstoriaQuerySpanGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Generates the span by following Astoria client rules.
    /// </summary>
    [ImplementationSelector("LinqToAstoriaQuerySpanGenerator", DefaultImplementation = "Default")]
    public interface ILinqToAstoriaQuerySpanGenerator
    {
        /// <summary>
        /// Generates the span by following Astoria client rules.
        /// </summary>
        /// <param name="valueToSpan">The value to span (fully connected graph of entities).</param>
        /// <param name="expandedPaths">The expanded paths.</param>
        /// <param name="selectedPaths">The selected paths.</param>
        /// <returns>
        /// Clone of the input value trimmed to only include objects reachable via span paths.
        /// </returns>
        QueryValue GenerateSpan(QueryValue valueToSpan, IEnumerable<string> expandedPaths, IEnumerable<string> selectedPaths);
    }
}