//---------------------------------------------------------------------
// <copyright file="AstoriaQueryStreamValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using System;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Result of a query evaluation which is a primitive value.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1036:OverrideMethodsOnComparableTypes", Justification = "By design.")]
    public sealed class AstoriaQueryStreamValue : QueryValue
    {
         /// <summary>
        /// Initializes a new instance of the AstoriaQueryStreamValue class.
        /// </summary>
        /// <param name="type">The type of the value.</param>
        /// <param name="value">The value.</param>
        /// <param name="evaluationError">The evaluation error.</param>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        public AstoriaQueryStreamValue(AstoriaQueryStreamType type, byte[] value,  QueryError evaluationError, IQueryEvaluationStrategy evaluationStrategy)
            : base(evaluationError, evaluationStrategy)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            this.Type = type;
            this.Value = value;
        }

        /// <summary>
        /// Gets the stream type.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Must be the same as the base class.")]
        public new AstoriaQueryStreamType Type { get; private set; }

        /// <summary>
        /// Gets or sets the stream value.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819", Justification = "Desired behavior")]
        public byte[] Value { get; set; }

        /// <summary>
        /// Gets or sets the ETag.
        /// </summary>
        /// <value>The ETag value.</value>
        public string ETag { get; set; }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        /// <value>The ContentType.</value>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the edit Link
        /// </summary>
        public Uri EditLink { get; set; }

        /// <summary>
        /// Gets or sets the self Link
        /// </summary>
        public Uri SelfLink { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is null.
        /// </summary>
        public override bool IsNull
        {
            get { return this.Value == null; }
        }

        /// <summary>
        /// Casts a <see cref="QueryStreamValue"/> to a <see cref="QueryStreamType"/>. The cast will return the value type cast to the new type.
        /// </summary>
        /// <param name="type">The type for the cast operation.</param>
        /// <returns><see cref="QueryStreamValue"/> which is cast to the appropriate type</returns>
        public override QueryValue Cast(QueryType type)
        {
            return type.CreateErrorValue(new QueryError("Cannot perform Cast on a stream value"));
        }

        /// <summary>
        /// Checks if a <see cref="QueryValue"/> is of a particular <see cref="QueryType"/>. This operation will return a true if the value is of the specified type.
        /// </summary>
        /// <param name="type">The type for the IsOf operation.</param>
        /// <param name="performExactMatch">Determines if an exact match needs to be performed.</param>
        /// <returns>A <see cref="QueryValue"/> containing true or false depending on whether the value is of the specified type or not.</returns>
        public override QueryValue IsOf(QueryType type, bool performExactMatch)
        {
            return type.CreateErrorValue(new QueryError("Cannot perform IsOf on a stream value"));
        }

        /// <summary>
        /// Converts the <see cref="QueryValue"/> to a particular <see cref="QueryType"/>.
        /// </summary>
        /// <param name="type">The type for the As operation.</param>
        /// <returns>The <see cref="QueryValue"/> converted to the specified type if successful. Returns null if this operation fails.</returns>
        public override QueryValue TreatAs(QueryType type)
        {
            return type.CreateErrorValue(new QueryError("Cannot perform TreatAs on a stream value"));
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this query value.</param>
        /// <returns>The result of visiting this query value.</returns>
        public override TResult Accept<TResult>(IQueryValueVisitor<TResult> visitor)
        {
            return ((IAstoriaQueryValueVisitor<TResult>)visitor).Visit(this);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.EvaluationError != null)
            {
                return "Stream Value Error=" + this.EvaluationError + ", Type=" + this.Type;
            }
            else
            {
                return "Stream Value=" + this.Value + ", Type=" + this.Type;
            }
        }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        /// <returns>Type of the value.</returns>
        protected override QueryType GetTypeInternal()
        {
            return this.Type;
        }
    }
}
