//---------------------------------------------------------------------
// <copyright file="QueryVoidType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Wrappers;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Void Query Type
    /// </summary>
    public class QueryVoidType : QueryType
    {
        /// <summary>
        /// Initializes a new instance of the QueryVoidType class.
        /// </summary>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        public QueryVoidType(IQueryEvaluationStrategy evaluationStrategy)
            : base(evaluationStrategy)
        {
        }

        /// <summary>
        /// Gets the String Representation
        /// </summary>
        public override string StringRepresentation
        {
            get { return "QueryVoidType"; }
        }

        /// <summary>
        /// Accept on Type
        /// </summary>
        /// <typeparam name="TResult">Result Type</typeparam>
        /// <param name="visitor">Visitor to use</param>
        /// <returns>An Invalid Operation Exception</returns>
        public override TResult Accept<TResult>(IQueryTypeVisitor<TResult> visitor)
        {
            throw new TaupoInvalidOperationException("Void Type is not allowed");
        }

        /// <summary>
        /// Creates a Collection of void, InvalidOperation
        /// </summary>
        /// <returns>Throws TaupoInvalidOperationException</returns>
        protected override QueryCollectionType CreateCollectionTypeInternal()
        {
            throw new TaupoInvalidOperationException("Collection of Void is not allowed");
        }

        /// <summary>
        /// Gets Null Value
        /// </summary>
        /// <returns>Throws TaupoInvalidOperationException</returns>
        protected override QueryValue GetNullValueInternal()
        {
            throw new TaupoInvalidOperationException("Nullable of Void is not allowed");
        }

        /// <summary>
        /// Creates error value
        /// </summary>
        /// <param name="evaluationError">evaluation error</param>
        /// <returns>Throws TaupoInvalidOperationException</returns>
        protected override QueryValue CreateErrorValueInternal(QueryError evaluationError)
        {
            throw new TaupoInvalidOperationException("Error Evaluation of Void is not allowed");
        }
    }
}
