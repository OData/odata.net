//---------------------------------------------------------------------
// <copyright file="QueryProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents a member (property) of a QueryStructuralType.
    /// </summary>
    public abstract class QueryProperty
    {
        /// <summary>
        /// Initializes a new instance of the QueryProperty class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyType">The property type.</param>
        protected QueryProperty(string propertyName, QueryType propertyType)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(propertyName, "propertyName");
            ExceptionUtilities.CheckArgumentNotNull(propertyType, "propertyType");

            this.Name = propertyName;
            this.PropertyType = propertyType;
        }

        /// <summary>
        /// Gets the member name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether member is a primary key.
        /// </summary>
        public bool IsPrimaryKey { get; protected set; }

        /// <summary>
        /// Gets the property type.
        /// </summary>
        /// <value>The property type.</value>
        public QueryType PropertyType { get; private set; }

        /// <summary>
        /// Creates a strongly-typed QueryProperty of T
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns>Strongly typed QueryProperty of T.</returns>
        public static QueryProperty Create(string propertyName, QueryType propertyType)
        {
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            ExceptionUtilities.CheckArgumentNotNull(propertyType, "propertyType");

            // TODO: Create and use an implementation of IQueryTypeVisitor<QueryProperty> instead of using IF statements
            var scalarType = propertyType as QueryScalarType;
            if (scalarType != null)
            {
                return Create(propertyName, scalarType);
            }

            var streamType = propertyType as QueryStreamType;
            if (streamType != null)
            {
                return Create(propertyName, streamType);
            }

            var structuralType = propertyType as QueryStructuralType;
            if (structuralType != null)
            {
                return Create(propertyName, structuralType);
            }

            var referenceType = propertyType as QueryReferenceType;
            if (referenceType != null)
            {
                return Create(propertyName, referenceType);
            }

            var collectionType = propertyType as QueryCollectionType;
            if (collectionType != null)
            {
                Type type = collectionType.GetType();
                var queryPropertyClass = typeof(QueryProperty<>).MakeGenericType(type);

                return (QueryProperty)Activator.CreateInstance(queryPropertyClass, propertyName, propertyType);
            }

            throw new TaupoNotSupportedException(
                string.Format(CultureInfo.InvariantCulture, "This property type is not supported: '{0}'.", propertyType.StringRepresentation));
        }

        /// <summary>
        /// Creates a strongly-typed QueryProperty of T
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns>Strongly typed QueryProperty of T.</returns>
        public static QueryProperty<QueryReferenceType> Create(string propertyName, QueryReferenceType propertyType)
        {
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            ExceptionUtilities.CheckArgumentNotNull(propertyType, "propertyType");

            return new QueryProperty<QueryReferenceType>(propertyName, propertyType);
        }

        /// <summary>
        /// Creates a strongly-typed QueryProperty of T
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns>Strongly typed QueryProperty of T.</returns>
        public static QueryProperty<QueryScalarType> Create(string propertyName, QueryScalarType propertyType)
        {
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            ExceptionUtilities.CheckArgumentNotNull(propertyType, "propertyType");

            return new QueryProperty<QueryScalarType>(propertyName, propertyType);
        }

        /// <summary>
        /// Creates a strongly-typed QueryProperty of T
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns>Strongly typed QueryProperty of T.</returns>
        public static QueryProperty<QueryStreamType> Create(string propertyName, QueryStreamType propertyType)
        {
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            ExceptionUtilities.CheckArgumentNotNull(propertyType, "propertyType");

            return new QueryProperty<QueryStreamType>(propertyName, propertyType);
        }

        /// <summary>
        /// Creates a strongly-typed QueryProperty of T
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns>Strongly typed QueryProperty of T.</returns>
        public static QueryProperty<QueryStructuralType> Create(string propertyName, QueryStructuralType propertyType)
        {
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            ExceptionUtilities.CheckArgumentNotNull(propertyType, "propertyType");

            return new QueryProperty<QueryStructuralType>(propertyName, propertyType);
        }

        /// <summary>
        /// Creates a strongly-typed QueryProperty of T
        /// </summary>
        /// <typeparam name="TElement">The type of the collection element.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns>Strongly typed QueryProperty of T.</returns>
        public static QueryProperty<QueryCollectionType<TElement>> Create<TElement>(string propertyName, QueryCollectionType<TElement> propertyType)
            where TElement : QueryType
        {
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            ExceptionUtilities.CheckArgumentNotNull(propertyType, "propertyType");

            return new QueryProperty<QueryCollectionType<TElement>>(propertyName, propertyType);
        }

        /// <summary>
        /// Marks this property to be part of the primary key.
        /// </summary>
        /// <returns>This property (useful for chaining calls together)</returns>
        public QueryProperty SetPrimaryKey()
        {
            return this.SetPrimaryKey(true);
        }

        /// <summary>
        /// Marks this property to be part of the primary key.
        /// </summary>
        /// <param name="isPrimaryKey">If set to <c>true</c>, the property will be marked as primary key.</param>
        /// <returns>
        /// This property (useful for chaining calls together)
        /// </returns>
        public QueryProperty SetPrimaryKey(bool isPrimaryKey)
        {
            this.IsPrimaryKey = isPrimaryKey;
            return this;
        }
    }
}
