//---------------------------------------------------------------------
// <copyright file="SpatialDataType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// User defined data type
    /// </summary>
    public class SpatialDataType : PrimitiveDataType
    {
        /// <summary>
        /// Initializes a new instance of the SpatialDataType class.
        /// </summary>
        internal SpatialDataType()
            : this(true, new PrimitiveDataTypeFacet[] { })
        {
        }

        /// <summary>
        /// Initializes a new instance of the SpatialDataType class.
        /// </summary>
        /// <param name="isNullable">If set to <c>true</c>, the type is nullable.</param>
        /// <param name="facets">The facets.</param>
        internal SpatialDataType(bool isNullable, IEnumerable<PrimitiveDataTypeFacet> facets)
            : base(isNullable, facets)
        {
            this.Properties = new List<MemberProperty>();
            this.Methods = new List<Function>();
        }

        /// <summary>
        /// Gets list of public properties of the type. 
        /// </summary>
        public IList<MemberProperty> Properties { get; private set; }

        /// <summary>
        /// Gets list of public methods of the type. 
        /// </summary>
        public IList<Function> Methods { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="SpatialDataType" /> with the specified nullability.
        /// </summary>
        /// <param name="isNullable">If set to <c>true</c> the type will be nullable.</param>
        /// <returns>New instance of <see cref="SpatialDataType"/> with specified nullability.</returns>
        public SpatialDataType Nullable(bool isNullable)
        {
            var spatial = new SpatialDataType(isNullable, this.Facets)
                .WithProperties(this.Properties.ToArray())
                .WithMethods(this.Methods.ToArray());

            return spatial;
        }

        /// <summary>
        /// Adds given properties to <see cref="Properties"/> collection.
        /// </summary>
        /// <param name="properties">Properties to add.</param>
        /// <returns>This object (useful for chaining multiple calls).</returns>
        public SpatialDataType WithProperties(params MemberProperty[] properties)
        {
            ExceptionUtilities.CheckCollectionDoesNotContainNulls(properties, "properties");
            ExceptionUtilities.Assert(this.Properties.Count == 0, "This method can only be called on spatial type without any properties defined.");

            foreach (var p in properties)
            {
                this.Properties.Add(p);
            }

            return this;
        }

        /// <summary>
        /// Adds given methods to <see cref="Methods"/> collection.
        /// </summary>
        /// <param name="methods">methods to add.</param>
        /// <returns>This object (useful for chaining multiple calls).</returns>
        public SpatialDataType WithMethods(params Function[] methods)
        {
            ExceptionUtilities.CheckCollectionDoesNotContainNulls(methods, "methods");
            ExceptionUtilities.Assert(this.Methods.Count == 0, "This method can only be called on spatial type without any methods defined.");

            foreach (var m in methods)
            {
                this.Methods.Add(m);
            }

            return this;
        }

        /// <summary>
        /// Accepts the specified visitor by calling its Visit method.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="visitor">The visitor.</param>
        /// <returns>Visitor-specific value.</returns>
        public override TValue Accept<TValue>(IPrimitiveDataTypeVisitor<TValue> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SpatialDataType"/> which has the same facets as
        /// this instance, except for <see cref="SridFacet"/> which is set to the specified value.
        /// </summary>
        /// <param name="srid">The srid reference for the spatial data.</param>
        /// <returns>
        /// New instance of <see cref="SpatialDataType"/> with the new facet.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Srid", Justification = "This name is used in the product as well")]
        public SpatialDataType WithSrid(string srid)
        {
            return this.WithFacet(new SridFacet(srid));
        }

        /// <summary>
        /// Creates a new type based on this type with the specified nullability flag and facets.
        /// </summary>
        /// <param name="isNullable">Nullability flag for the new type.</param>
        /// <param name="facets">List of facets for the new type.</param>
        /// <returns>
        /// Newly created <see cref="SpatialDataType"/>.
        /// </returns>
        protected internal override PrimitiveDataType Create(bool isNullable, IEnumerable<PrimitiveDataTypeFacet> facets)
        {
            var spatial = new SpatialDataType(isNullable, facets)
                    .WithProperties(this.Properties.ToArray())
                    .WithMethods(this.Methods.ToArray());

            return spatial;
        }

        /// <summary>
        /// Determines whether the specified value is compatible with the type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// Currently throw TaupoNotSupportedException
        /// </returns>
        protected override bool IsValueCompatible(object value)
        {
            throw new TaupoNotSupportedException("Taupo does not support determining whether value is compatible with spatial data type yet.");
        }
    }
}
