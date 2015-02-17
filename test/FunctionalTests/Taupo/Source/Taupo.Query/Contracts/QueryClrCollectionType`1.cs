//---------------------------------------------------------------------
// <copyright file="QueryClrCollectionType`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents a Clr collection type in a QueryType hierarchy. 
    /// </summary>
    /// <typeparam name="TQueryType">The element type of the QueryClrCollectionType </typeparam> 
    public class QueryClrCollectionType<TQueryType> : QueryCollectionType<TQueryType>, IQueryClrType where TQueryType : QueryType
    {
        /// <summary>
        /// Initializes a new instance of the QueryClrCollectionType class.
        /// </summary>
        /// <param name="clrType">Wrapped CLR type.</param>
        /// <param name="elementType">Type of a single element in the collection.</param>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        public QueryClrCollectionType(Type clrType, TQueryType elementType, IQueryEvaluationStrategy evaluationStrategy)
            : base(elementType, evaluationStrategy)
        {
            ExceptionUtilities.CheckArgumentNotNull(clrType, "clrType");
            ExceptionUtilities.CheckArgumentNotNull(elementType, "elementType");
            this.ClrType = clrType;
        }

        /// <summary> 
        /// Gets the ClrType which will be used in generated types for this collection type
        /// </summary>
        public Type ClrType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the string representation of a given query type.
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}({1})", this.ClrType.Name, this.ElementType.StringRepresentation);
            }
        }
    }
}
