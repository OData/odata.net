﻿//---------------------------------------------------------------------
// <copyright file="RangeVariableToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    /// <summary>
    /// Lexical token representing the parameter for an Any/All query.
    /// </summary>
    public sealed class RangeVariableToken : QueryToken
    {
        /// <summary>
        /// The name of the Any/All parameter.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Create a new RangeVariableToken
        /// </summary>
        /// <param name="name">The name of the visitor for the Any/All query.</param>
        public RangeVariableToken(string name)
        {
            ExceptionUtils.CheckArgumentNotNull(name, "visitor");

            this.name = name;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.RangeVariable; }
        }

        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Accept a <see cref="ISyntacticTreeVisitor{T}"/> to walk a tree of <see cref="QueryToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(ISyntacticTreeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}