//---------------------------------------------------------------------
// <copyright file="PrimitiveDataType.cs" company="Microsoft">
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

    /// <summary>
    /// Base class for all primitive data types.
    /// </summary>
    public abstract class PrimitiveDataType : DataType, IEquatable<PrimitiveDataType>
    {
        private static PrimitiveDataTypeFacet[] emptyFacetArray = new PrimitiveDataTypeFacet[0];
        private int? hashCode;

        /// <summary>
        /// Initializes a new instance of the PrimitiveDataType class.
        /// </summary>
        /// <param name="isNullable">If set to <c>true</c>, the type is nullable.</param>
        /// <param name="facets">The facets.</param>
        protected PrimitiveDataType(bool isNullable, IEnumerable<PrimitiveDataTypeFacet> facets)
            : base(isNullable)
        {
            this.Facets = facets ?? emptyFacetArray;
        }

        /// <summary>
        /// Gets the collection of all facets defined on the type.
        /// </summary>
        /// <value>The collection of facets.</value>
        public IEnumerable<PrimitiveDataTypeFacet> Facets { get; private set; }

        /// <summary>
        /// Tries to get the specified facet of the type.
        /// </summary>
        /// <typeparam name="TFacet">The type of the facet.</typeparam>
        /// <param name="facet">The facet.</param>
        /// <returns>True if the facet facet was present, false otherwise.</returns>
        public bool TryGetFacet<TFacet>(out TFacet facet)
        {
            facet = this.Facets.OfType<TFacet>().SingleOrDefault();
            return facet != null;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.GetCanonicalName();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (!this.hashCode.HasValue)
            {
                this.hashCode = this.GetCanonicalName().GetHashCode();
            }

            return this.hashCode.Value;
        }

        /// <summary>
        /// Gets the facet of this primitive type.
        /// </summary>
        /// <typeparam name="TFacet">The type of the facet.</typeparam>
        /// <returns>The facet associated with the type. If the facet cannot be retrieved, an exception is thrown.</returns>
        public TFacet GetFacet<TFacet>()
            where TFacet : PrimitiveDataTypeFacet
        {
            TFacet facet;

            if (!this.TryGetFacet(out facet))
            {
                throw new TaupoInvalidOperationException("Facet of type '" + typeof(TFacet).Name + "' not defined on " + this);
            }

            return facet;
        }

        /// <summary>
        /// Gets the facet value or returns a default value if the specified facet was not found.
        /// </summary>
        /// <typeparam name="TFacet">The type of the facet.</typeparam>
        /// <typeparam name="TValue">The type of the facet value.</typeparam>
        /// <param name="defaultValue">The default value to return if the facet is not found.</param>
        /// <returns>Facet value or default.</returns>
        public TValue GetFacetValue<TFacet, TValue>(TValue defaultValue)
            where TFacet : PrimitiveDataTypeFacet<TValue>
        {
            TFacet facet;

            if (!this.TryGetFacet(out facet))
            {
                return defaultValue;
            }

            return facet.Value;
        }

        /// <summary>
        /// Determines whether this data type has the specified facet.
        /// </summary>
        /// <typeparam name="TFacet">The type of the facet.</typeparam>
        /// <returns>
        /// A value of <c>true</c> if this instance has the specified facet; otherwise, <c>false</c>.
        /// </returns>
        public bool HasFacet<TFacet>()
            where TFacet : PrimitiveDataTypeFacet
        {
            TFacet facet;

            return this.TryGetFacet(out facet);
        }

        /// <summary>
        /// Accepts the specified visitor by calling its Visit method.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="visitor">The visitor.</param>
        /// <returns>Visitor-specific value.</returns>
        public abstract TValue Accept<TValue>(IPrimitiveDataTypeVisitor<TValue> visitor);

        /// <summary>
        /// Accepts the specified visitor by calling its Visit method.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="visitor">The visitor.</param>
        /// <returns>Visitor-specific value.</returns>
        public override TValue Accept<TValue>(IDataTypeVisitor<TValue> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Determines whether this data type is equal to another data type.
        /// </summary>
        /// <param name="other">The other data type.</param>
        /// <returns>True if this <see cref="DataType"/> is equal to the <paramref name="other"/>, false otherwise.</returns>
        public override bool Equals(DataType other)
        {
            PrimitiveDataType pdt = other as PrimitiveDataType;
            if (pdt == null)
            {
                return false;
            }

            return this.Equals(pdt);
        }

        /// <summary>
        /// Determines whether this primitive type is equal to another primitive type.
        /// </summary>
        /// <param name="other">The other primitive type.</param>
        /// <returns>True if this <see cref="PrimitiveDataType"/> is equal to the <paramref name="other"/>, false otherwise.</returns>
        public bool Equals(PrimitiveDataType other)
        {
            if (other == null)
            {
                return false;
            }

            if (this.GetHashCode() != other.GetHashCode())
            {
                return false;
            }

            return this.GetCanonicalName().Equals(other.GetCanonicalName());
        }

        internal DataType Combine(PrimitiveDataTypeFacet newFacet)
        {
            return this.Create(this.IsNullable, CombineFacets(this.Facets, newFacet));
        }

        /// <summary>
        /// Creates a new type based on this type with the specified nullability flag and facets.
        /// </summary>
        /// <param name="isNullable">Nullability flag for the new type.</param>
        /// <param name="facets">List of facets for the new type.</param>
        /// <returns>Newly created <see cref="PrimitiveDataType"/>.</returns>
        protected internal abstract PrimitiveDataType Create(bool isNullable, IEnumerable<PrimitiveDataTypeFacet> facets);

        private static IEnumerable<PrimitiveDataTypeFacet> CombineFacets(IEnumerable<PrimitiveDataTypeFacet> facets, PrimitiveDataTypeFacet newFacet)
        {
            List<PrimitiveDataTypeFacet> result = new List<PrimitiveDataTypeFacet>();

            bool inserted = false;

            // if the new facet is not volatile, remove all volatile facets from the list
            if (!newFacet.IsVolatile)
            {
                facets = facets.Where(f => !f.IsVolatile);
            }

            var enumerator = facets.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (!inserted)
                {
                    int comparison = string.Compare(enumerator.Current.GetType().FullName, newFacet.GetType().FullName, StringComparison.Ordinal);

                    if (comparison >= 0)
                    {
                        inserted = true;
                        result.Add(newFacet);
                        if (comparison == 0)
                        {
                            continue;
                        }
                    }
                }

                result.Add(enumerator.Current);
            }

            if (!inserted)
            {
                result.Add(newFacet);
            }

            return result;
        }

        /// <summary>
        /// Gets the canonical name of this data type.
        /// </summary>
        /// <returns>Type name, nullable flag + all the facets concatenated.</returns>
        private string GetCanonicalName()
        {
            DebuggerDisplayFacet debuggerDisplayFacet;

            StringBuilder sb = new StringBuilder();
            if (this.TryGetFacet(out debuggerDisplayFacet))
            {
                sb.Append(debuggerDisplayFacet.Value);
                if (this.IsNullable)
                {
                    sb.Append("(nullable)");
                }
            }
            else
            {
                sb.Append(this.GetType().Name);
                sb.Append("[");
                sb.Append("Nullable=");
                sb.Append(this.IsNullable);
                foreach (PrimitiveDataTypeFacet facet in this.Facets)
                {
                    sb.Append(",");
                    sb.Append(facet.ToString());
                }

                sb.Append("]");
            }

            return sb.ToString();
        }
    }
}
