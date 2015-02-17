//---------------------------------------------------------------------
// <copyright file="ILinqToAstoriaExpressionEvaluator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;
    using Microsoft.Test.Taupo.Query.Linq;

    /// <summary>
    /// Evaluates query expressions for a given query data set.
    /// </summary>
    [ImplementationSelector("LinqToAstoriaEvaluator", DefaultImplementation = "Default")]
    public interface ILinqToAstoriaExpressionEvaluator : IQueryExpressionEvaluator
    {
        /// <summary>
        /// Temporarily replaces the evaluator's data-set with the one given
        /// </summary>
        /// <param name="temporary">The temporary query data-set</param>
        /// <returns>A token that, when disposed, will reset the query data-set back to the original.</returns>
        IDisposable WithTemporaryDataSet(IQueryDataSet temporary);
    }
}