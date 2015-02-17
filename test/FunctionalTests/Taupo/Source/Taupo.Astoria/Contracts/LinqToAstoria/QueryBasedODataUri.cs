//---------------------------------------------------------------------
// <copyright file="QueryBasedODataUri.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Augments the OData uri class with query expressions
    /// </summary>
    public class QueryBasedODataUri : ODataUri
    {
        /// <summary>
        /// Initializes a new instance of the QueryBasedODataUri class
        /// </summary>
        public QueryBasedODataUri()
            : base()
        {
            this.OrderByExpressions = new List<LinqOrderByExpression>();
            this.FilterExpressions = new List<LinqLambdaExpression>();
        }

        /// <summary>
        /// Gets the list of OrderByExpressions for the '$orderby' query option.
        /// </summary>
        public IList<LinqOrderByExpression> OrderByExpressions { get; private set; }

        /// <summary>
        /// Gets the list of expressions for the '$filter' query option
        /// </summary>
        public IList<LinqLambdaExpression> FilterExpressions { get; private set; }
    }
}
