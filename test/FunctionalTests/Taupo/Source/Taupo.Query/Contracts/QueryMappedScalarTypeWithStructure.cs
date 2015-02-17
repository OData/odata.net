//---------------------------------------------------------------------
// <copyright file="QueryMappedScalarTypeWithStructure.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Represents a scalar type with properties or methods in query type hierarchy.
    /// </summary>
    public class QueryMappedScalarTypeWithStructure : QueryMappedScalarType, IQueryTypeWithProperties, IQueryTypeWithMethods
    {
        /// <summary>
        /// Initializes a new instance of the QueryMappedScalarTypeWithStructure class.
        /// </summary>
        /// <param name="modelType">The conceptual model type.</param>
        /// <param name="storeType">The store model type.</param>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        public QueryMappedScalarTypeWithStructure(SpatialDataType modelType, SpatialDataType storeType, IQueryEvaluationStrategy evaluationStrategy)
            : base(modelType, storeType, evaluationStrategy)
        {
            this.Properties = new List<QueryProperty>();
            this.Methods = new List<Function>();
            this.IsReadOnly = false;
        }

        /// <summary>
        /// Gets a value indicating whether the spatial type has been completely built
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// Gets the collection of memeber properties.
        /// </summary>
        public IList<QueryProperty> Properties { get; private set; }

        /// <summary>
        /// Gets the collection of member methods.
        /// </summary>
        public IList<Function> Methods { get; private set; }

        /// <summary>
        /// Gets the string representation of a given QueryMappedScalarTypeWithStructure type.
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "Mapped Scalar Type With Structure, Model='{0}', Store='{1}', Clr='{2}'", this.ModelType, this.StoreType, this.ClrType);
            }
        }

        /// <summary>
        /// Invoke this method to specify this spatial type has been completly built and it's Properties collection should become read-only.
        /// </summary>
        /// <returns>This object (suitable for chaining calls together).</returns>
        public QueryMappedScalarTypeWithStructure MakeReadOnly()
        {
            this.Properties = this.Properties.ToList().AsReadOnly();
            this.Methods = this.Methods.ToList().AsReadOnly();
            this.IsReadOnly = true;
            return this;
        }

        /// <summary>
        /// Adds the specified property to the member properties.
        /// </summary>
        /// <param name="properties">The properties to add.</param>
        public void Add(IEnumerable<QueryProperty> properties)
        {
            ExceptionUtilities.CheckArgumentNotNull(properties, "properties");
            ExceptionUtilities.Assert(this.IsReadOnly == false, "Spatial type cannot be changed since it is read only.");

            foreach (var property in properties)
            {
                this.Properties.Add(property);
            }
        }

        /// <summary>
        /// Adds the specified methods to the member methods.
        /// </summary>
        /// <param name="methods">The methods to add.</param>
        public void Add(IEnumerable<Function> methods)
        {
            ExceptionUtilities.CheckArgumentNotNull(methods, "methods");
            ExceptionUtilities.Assert(this.IsReadOnly == false, "Spatial cannot be changed since it is read only.");

            foreach (var method in methods)
            {
                this.Methods.Add(method);
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
            return visitor.Visit(this);
        }
    }
}