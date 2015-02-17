//---------------------------------------------------------------------
// <copyright file="QueryStructuralType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents a structural type (type that has Properties) in a QueryType hierarchy.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public abstract class QueryStructuralType : QueryType, IQueryTypeWithProperties, IEnumerable
    {
        private QueryStructuralType parent;
        private object queryCollectionType;

        /// <summary>
        /// Initializes a new instance of the QueryStructuralType class.
        /// </summary>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        protected QueryStructuralType(IQueryEvaluationStrategy evaluationStrategy)
            : base(evaluationStrategy)
        {
            this.Properties = new List<QueryProperty>();
            this.DerivedTypes = new List<QueryStructuralType>();
            this.IsReadOnly = false;
        }

        /// <summary>
        /// Gets a value indicating whether the structural type has been completely built
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// Gets the collection of type properties.
        /// </summary>
        public IList<QueryProperty> Properties { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is a value type.
        /// </summary>
        /// <value>
        /// Value <c>true</c> if this instance is value type; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsValueType
        {
            get { return false; }
        }

        /// <summary>
        /// Gets or sets the direct parent structural type if the structural type is derived from another structural type. Null if this structural type is a base type.
        /// </summary>
        public QueryStructuralType Parent
        {
            get
            {
                return this.parent;
            }

            set
            {
                this.AssertNotReadOnly();
                this.parent = value;
            }
        }

        /// <summary>
        /// Gets the list of all derived types.
        /// </summary>
        public IList<QueryStructuralType> DerivedTypes { get; private set; }

        /// <summary>
        /// Gets the list of immediate children of the type.
        /// </summary>
        public IEnumerable<QueryStructuralType> Children
        {
            get { return this.DerivedTypes.Where(c => c.Parent == this); }
        }

        /// <summary>
        /// Gets the null value for a given type.
        /// </summary>
        public new QueryStructuralValue NullValue
        {
            get { return new QueryStructuralValue(this, true, null, this.EvaluationStrategy); }
        }

        /// <summary>
        /// Get the most basic type of this type.
        /// </summary>
        /// <returns>Base most type of the given type</returns>
        public QueryStructuralType GetRootType()
        {
            var baseType = this;
            while (baseType.Parent != null)
            {
                baseType = baseType.Parent;
            }

            return baseType;
        }

        /// <summary>
        /// Adds the specified property to the type Properties.
        /// </summary>
        /// <param name="property">The property to add.</param>
        public void Add(QueryProperty property)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");

            this.AssertNotReadOnly();
            this.Properties.Add(property);
        }

        /// <summary>
        /// Adds the specified properties to the type Properties.
        /// </summary>
        /// <param name="properties">The properties.</param>
        public void AddProperties(IEnumerable<QueryProperty> properties)
        {
            ExceptionUtilities.CheckArgumentNotNull(properties, "properties");

            this.AssertNotReadOnly();
            foreach (var prop in properties)
            {
                this.Properties.Add(prop);
            }
        }

        /// <summary>
        /// Invoke this method to specify this structural type has been completly built and it's Properties collection should become read-only.
        /// </summary>
        /// <returns>This object (suitable for chaining calls together).</returns>
        public QueryStructuralType MakeReadOnly()
        {
            this.Properties = this.Properties.ToList().AsReadOnly();
            this.DerivedTypes = this.DerivedTypes.ToList().AsReadOnly();
            this.IsReadOnly = true;
            return this;
        }

        /// <summary>
        /// Creates the typed collection where element type is the current structural type.
        /// </summary>
        /// <returns>Collection of the given type.</returns>
        public new QueryCollectionType<QueryStructuralType> CreateCollectionType()
        {
            if (this.queryCollectionType == null)
            {
                this.queryCollectionType = QueryCollectionType.Create(this);
            }

            return (QueryCollectionType<QueryStructuralType>)this.queryCollectionType;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            throw ExceptionUtilities.CreateIEnumerableNotImplementedException();
        }

        /// <summary>
        /// Creates the new instance of the structural type.
        /// </summary>
        /// <returns>Instance of newly created <see cref="QueryStructuralValue"/> with all member values uninitialized (null)</returns>
        public virtual QueryStructuralValue CreateNewInstance()
        {
            return new QueryStructuralValue(this, false, null, this.EvaluationStrategy);
        }

        /// <summary>
        /// Creates the error value of this type.
        /// </summary>
        /// <param name="evaluationError">The evaluation error.</param>
        /// <returns>Created error value.</returns>
        public new QueryStructuralValue CreateErrorValue(QueryError evaluationError)
        {
            return new QueryStructuralValue(this, true, evaluationError, this.EvaluationStrategy);
        }

        /// <summary>
        /// Gets the non-strongly typed collection type for this type.
        /// </summary>
        /// <returns>
        /// Instance of <see cref="QueryCollectionType"/> which is a collection of this type.
        /// </returns>
        protected override QueryCollectionType CreateCollectionTypeInternal()
        {
            return this.CreateCollectionType();
        }

        /// <summary>
        /// Gets the non-strongly typed null value.
        /// </summary>
        /// <returns>Null value.</returns>
        protected override QueryValue GetNullValueInternal()
        {
            return this.NullValue;
        }

        /// <summary>
        /// Creates the non-strongly typed error value.
        /// </summary>
        /// <param name="evaluationError">The evaluation error.</param>
        /// <returns>Created error value.</returns>
        protected override QueryValue CreateErrorValueInternal(QueryError evaluationError)
        {
            return this.CreateErrorValue(evaluationError);
        }

        private void AssertNotReadOnly()
        {
            if (this.IsReadOnly)
            {
                throw new TaupoInvalidOperationException("Structural type cannot be changed since it is read only.");
            }
        }
    }
}
