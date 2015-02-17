//---------------------------------------------------------------------
// <copyright file="AnyAllQueryToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Query
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Lexical token representing the Any/All Query
    /// </summary>
    public abstract class AnyAllQueryToken : QueryToken
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
        protected AnyAllQueryToken(QueryToken expression, string parameter, QueryToken parent)
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
    }
}
