//---------------------------------------------------------------------
// <copyright file="QueryClrSpatialType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Represents a clr spatial type in a QueryType hierarchy.
    /// </summary>
    public class QueryClrSpatialType : QueryClrPrimitiveType, IQueryClrType, IQueryTypeWithProperties, IQueryTypeWithMethods
    {
        /// <summary>
        /// Initializes a new instance of the QueryClrSpatialType class.
        /// </summary>
        /// <param name="clrType">Wrapped CLR type.</param>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        public QueryClrSpatialType(Type clrType, IQueryEvaluationStrategy evaluationStrategy)
            : base(clrType, evaluationStrategy)
        {
            this.Properties = new List<QueryProperty>();
            this.Methods = new List<Function>();
            this.DerivedTypes = new List<QueryClrSpatialType>();
            this.IsReadOnly = false;
        }

        /// <summary>
        /// Gets a value indicating whether the spatial type has been completely built
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// Gets the collection of type properties.
        /// </summary>
        public IList<QueryProperty> Properties { get; private set; }

        /// <summary>
        /// Gets the collection of type methods.
        /// </summary>
        public IList<Function> Methods { get; private set; }

        /// <summary>
        /// Gets the types that derive from this one.
        /// </summary>
        public IList<QueryClrSpatialType> DerivedTypes { get; private set; }

        /// <summary>
        /// Invoke this method to specify this spatial type has been completly built and it's Properties collection should become read-only.
        /// </summary>
        /// <returns>This object (suitable for chaining calls together).</returns>
        public QueryClrSpatialType MakeReadOnly()
        {
            this.Properties = this.Properties.ToList().AsReadOnly();
            this.Methods = this.Methods.ToList().AsReadOnly();
            this.DerivedTypes = this.DerivedTypes.ToList().AsReadOnly();
            this.IsReadOnly = true;
            return this;
        }

        /// <summary>
        /// Adds the specified property to the type properties.
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
        /// Adds the specified methods to the type methods.
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
        /// Creates the scalar value for this type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// Newly created value.
        /// </returns>
        public override QueryScalarValue CreateValue(object value)
        {
            if (value == null)
            {
                return base.CreateValue(null);
            }

            var instanceType = value.GetType();
            var specificType = this.DerivedTypes.OrderBy(t => t.DerivedTypes.Count).FirstOrDefault(t => t.ClrType.IsAssignableFrom(instanceType));
            if (specificType != null)
            {
                return specificType.CreateValue(value);
            }

            return base.CreateValue(value);
        }
    }
}