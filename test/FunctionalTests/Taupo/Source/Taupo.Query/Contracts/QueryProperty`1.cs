//---------------------------------------------------------------------
// <copyright file="QueryProperty`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Strongly typed query property
    /// </summary>
    /// <typeparam name="TType">The type of the type.</typeparam>
    public class QueryProperty<TType> : QueryProperty
        where TType : QueryType
    {
        /// <summary>
        /// Initializes a new instance of the QueryProperty class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyType">The property type.</param>
        public QueryProperty(string propertyName, TType propertyType)
            : base(propertyName, propertyType)
        {
        }

        /// <summary>
        /// Gets the member type.
        /// </summary>
        public new TType PropertyType
        {
            get { return (TType)base.PropertyType; }
        }

        /// <summary>
        /// Marks this property to be part of the primary key.
        /// </summary>
        /// <returns>This property (useful for chaining calls together)</returns>
        public new QueryProperty<TType> SetPrimaryKey()
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
        public new QueryProperty<TType> SetPrimaryKey(bool isPrimaryKey)
        {
            this.IsPrimaryKey = isPrimaryKey;
            return this;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "'" + this.Name + "' Type=" + this.PropertyType + (this.IsPrimaryKey ? " (PK)" : string.Empty);
        }
    }
}
