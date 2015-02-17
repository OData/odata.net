//---------------------------------------------------------------------
// <copyright file="LinqLambdaType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents a type of lambda expression in a QueryType hierarchy. 
    /// </summary>
    public class LinqLambdaType : QueryType
    {
        /// <summary>
        /// Initializes a new instance of the LinqLambdaType class.
        /// </summary>
        /// <param name="bodyType">Type of the lambda body.</param>
        /// <param name="parameterTypes">List of the lambda parameter types.</param>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        internal LinqLambdaType(QueryType bodyType, IEnumerable<QueryType> parameterTypes, IQueryEvaluationStrategy evaluationStrategy)
            : base(evaluationStrategy)
        {
            this.BodyType = bodyType;
            this.ParameterTypes = parameterTypes.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the type of the lambda body.
        /// </summary>
        public QueryType BodyType { get; private set; }

        /// <summary>
        /// Gets the collection of lambda parameter types.
        /// </summary>
        public ReadOnlyCollection<QueryType> ParameterTypes { get; private set; }

        /// <summary>
        /// Gets the string representation of a given query type.
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return "Lambda Type((" + string.Join(", ", this.ParameterTypes.Select(c => c.StringRepresentation).ToArray()) + ") => " + this.BodyType.StringRepresentation + ")";
            }
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this query type.</param>
        /// <returns>The result of visiting this query type.</returns>
        public override TResult Accept<TResult>(IQueryTypeVisitor<TResult> visitor)
        {
            throw new TaupoNotSupportedException("Don not support Visitor for linq lambda type yet.");
        }

        /// <summary>
        /// Gets the non-strongly typed collection type for this type.
        /// </summary>
        /// <returns>
        /// Instance of <see cref="QueryCollectionType"/> which is a collection of this type.
        /// </returns>
        protected override QueryCollectionType CreateCollectionTypeInternal()
        {
            throw new TaupoNotSupportedException("Cannot create collections of lambda types.");
        }

        /// <summary>
        /// Gets the non-strongly typed null value.
        /// </summary>
        /// <returns>Null value.</returns>
        protected override QueryValue GetNullValueInternal()
        {
            throw new TaupoNotSupportedException("Lambda type does not have NULL value.");
        }

        /// <summary>
        /// Creates the non-strongly typed error value.
        /// </summary>
        /// <param name="evaluationError">The evaluation error.</param>
        /// <returns>Created error value.</returns>
        protected override QueryValue CreateErrorValueInternal(QueryError evaluationError)
        {
            throw new TaupoNotSupportedException("Lambda type cannot have error information.");
        }
    }
}
