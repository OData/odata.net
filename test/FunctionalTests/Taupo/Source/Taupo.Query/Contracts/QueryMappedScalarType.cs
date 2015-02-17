//---------------------------------------------------------------------
// <copyright file="QueryMappedScalarType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Represents a scalar type in a QueryType hierarchy which is a pair of model type and a store type.
    /// </summary>
    public class QueryMappedScalarType : QueryScalarType, IQueryClrType
    {
        /// <summary>
        /// Initializes a new instance of the QueryMappedScalarType class.
        /// </summary>
        /// <param name="modelType">The model type.</param>
        /// <param name="storeType">The store type.</param>
        /// <param name="clrType">The clr type corresponding to the store type.</param>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        public QueryMappedScalarType(PrimitiveDataType modelType, PrimitiveDataType storeType, Type clrType, IQueryEvaluationStrategy evaluationStrategy)
            : base(evaluationStrategy)
        {
            ExceptionUtilities.CheckArgumentNotNull(modelType, "modelType");
            ExceptionUtilities.CheckArgumentNotNull(storeType, "storeType");

            this.ModelType = modelType;
            this.StoreType = storeType;

            this.ClrType = clrType;
        }

        /// <summary>
        /// Initializes a new instance of the QueryMappedScalarType class.
        /// </summary>
        /// <param name="modelType">The model type.</param>
        /// <param name="storeType">The store type.</param>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        public QueryMappedScalarType(PrimitiveDataType modelType, PrimitiveDataType storeType, IQueryEvaluationStrategy evaluationStrategy)
            : base(evaluationStrategy)
        {
            ExceptionUtilities.CheckArgumentNotNull(modelType, "modelType");
            ExceptionUtilities.CheckArgumentNotNull(storeType, "storeType");

            this.ModelType = modelType;
            this.StoreType = storeType;

            var clrType = modelType.GetFacetValue<PrimitiveClrTypeFacet, Type>(null);

            if (clrType != null && modelType.IsNullable && clrType.IsValueType())
            {
                clrType = typeof(Nullable<>).MakeGenericType(clrType);
            }

            this.ClrType = clrType;
        }

        /// <summary>
        /// Gets the store type.
        /// </summary>
        /// <value>The store type.</value>
        public PrimitiveDataType StoreType { get; private set; }

        /// <summary>
        /// Gets the model type.
        /// </summary>
        /// <value>The model type.</value>
        public PrimitiveDataType ModelType { get; private set; }

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
                return string.Format(CultureInfo.InvariantCulture, "Mapped Scalar Type, Model='{0}', Store='{1}', Clr='{2}'", this.ModelType, this.StoreType, this.ClrType);
            }
        }

        /// <summary>
        /// Determines whether the type can be assigned from another.
        /// </summary>
        /// <param name="queryType">Type to assign from.</param>
        /// <returns>True if assignment is possible, false otherwise.</returns>
        public override bool IsAssignableFrom(QueryType queryType)
        {
            if (object.ReferenceEquals(this, queryType))
            {
                return true;
            }

            // if it is not a mapped scalar type, then it is not assignable
            var mappedScalarType = queryType as QueryMappedScalarType;
            if (mappedScalarType == null)
            {
                return false;
            }

            var targetSpatialDataType = mappedScalarType.ModelType as SpatialDataType;
            var spatialDataType = this.ModelType as SpatialDataType;

            // The spatial types have the same clrtype - DbGeometry or DbGeography but the strong types aren't assignable from each other.  For example a Point isn't assignable from a LineString.
            if (spatialDataType != null && targetSpatialDataType != null)
            {
                var edmTypeName = spatialDataType.GetFacetValue<EdmTypeNameFacet, string>(string.Empty);                
                if (edmTypeName.Equals("Geometry", StringComparison.OrdinalIgnoreCase))
                {
                    return EdmDataTypes.IsGeometricSpatialType(targetSpatialDataType);
                }

                if (edmTypeName.Equals("Geography", StringComparison.OrdinalIgnoreCase))
                {
                    return !EdmDataTypes.IsGeometricSpatialType(targetSpatialDataType);
                }

                var targetEdmTypeName = targetSpatialDataType.GetFacetValue<EdmTypeNameFacet, string>(string.Empty);
                return edmTypeName == targetEdmTypeName;
            }
            
            // otherwise, fall back to CLR semantics for assignment
            return this.ClrType.IsAssignableFrom(mappedScalarType.ClrType);
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