//---------------------------------------------------------------------
// <copyright file="ILinqToAstoriaQueryEvaluationStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Evaluation strategy for Linq to Astoria query expressions.
    /// </summary>
    public interface ILinqToAstoriaQueryEvaluationStrategy : IQueryEvaluationStrategy
    {
        /// <summary>
        /// Gets a value indicating whether order of collection value is predictable.
        /// </summary>
        bool IsCollectionOrderPredictable
        {
            get;
        }
    }
}