//---------------------------------------------------------------------
// <copyright file="QueryEntityType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Structural type which wraps a <see cref="EntityType"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class QueryEntityType : QueryStructuralType, IQueryClrType
    {
        /// <summary>
        /// Initializes a new instance of the QueryEntityType class.
        /// </summary>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="entitySet">The set that the entity belongs to</param>
        /// <param name="clrType">CLR type of the entity.</param>
        public QueryEntityType(IQueryEvaluationStrategy evaluationStrategy, EntityType entityType, EntitySet entitySet, Type clrType)
            : base(evaluationStrategy)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");

            this.EntityType = entityType;
            this.EntitySet = entitySet;
            this.ClrType = clrType;
        }

        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        public EntityType EntityType { get; private set; }

        /// <summary>
        /// Gets the set that the entity belongs to
        /// </summary>
        public EntitySet EntitySet { get; private set; }

        /// <summary>
        /// Gets or sets the CLR type information.
        /// </summary>
        public Type ClrType { get; set; }

        /// <summary>
        /// Gets the string representation of a given query type.
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return "EntityType(" + this.EntityType.FullName + ")";
            }
        }

        /// <summary>
        /// Creates reference type for this query entity type
        /// </summary>
        /// <returns>the reference type of this entity type</returns>
        public QueryReferenceType CreateReferenceType()
        {
            return new QueryReferenceType(this.EvaluationStrategy, this);
        }

        /// <summary>
        /// Creates the new instance of the entity type.
        /// </summary>
        /// <returns>Instance of newly created <see cref="QueryStructuralValue"/> with all member values uninitialized (null)</returns>
        public override QueryStructuralValue CreateNewInstance()
        {
            return new QueryEntityValue(this, false, null, this.EvaluationStrategy);
        }

        /// <summary>
        /// Determines whether the type can be assigned from another.
        /// </summary>
        /// <param name="queryType">Type to assign from.</param>
        /// <returns>True if assignment is possible, false otherwise.</returns>
        public override bool IsAssignableFrom(QueryType queryType)
        {
            ExceptionUtilities.CheckArgumentNotNull(queryType, "queryType");

            if (object.ReferenceEquals(this, queryType))
            {
                return true;
            }

            var other = queryType as QueryEntityType;
            if (other == null)
            {
                return false;
            }

            return this.DerivedTypes.Contains(other);
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this query type.</param>
        /// <returns>The result of visiting this query type.</returns>
        public override TResult Accept<TResult>(IQueryTypeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
