//---------------------------------------------------------------------
// <copyright file="LambdaToken.cs" company="Microsoft">
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
    /// Lexical token representing the Any/All Query
    /// </summary>
    public abstract class LambdaToken : QueryToken
    {
        /// <summary>
        /// The parent token.
        /// </summary>
        private readonly QueryToken parent;

        /// <summary>
        /// The parameter which denotes source type.
        /// </summary>
        private readonly string parameter;

        /// <summary>
        /// The expression component of Any.
        /// </summary>
        private readonly QueryToken expression;

        /// <summary>
        /// Create a AnyAllQueryToken given the expression, parameter, and parent
        /// </summary>
        /// <param name="expression">The associated expression.</param>
        /// <param name="parameter">The parameter denoting source type.</param>
        /// <param name="parent">The parent token.  Pass null if this property has no parent.</param>
        protected LambdaToken(QueryToken expression, string parameter, QueryToken parent)
        {
            this.expression = expression;
            this.parameter = parameter;
            this.parent = parent;
        }

        /// <summary>
        /// The parent token.
        /// </summary>
        public QueryToken Parent
        {
            get { return this.parent; }
        }

        /// <summary>
        /// The expression.
        /// </summary>
        public QueryToken Expression
        {
            get { return this.expression; }
        }

        /// <summary>
        /// The parameter.
        /// </summary>
        public string Parameter
        {
            get { return this.parameter; }
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