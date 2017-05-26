//---------------------------------------------------------------------
// <copyright file="FunctionParameterAliasToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    using System;
    using Microsoft.OData.Edm;

    /// <summary>
    /// A token to represent a parameter alias in a function call.
    /// </summary>
    internal sealed class FunctionParameterAliasToken : QueryToken
    {
        /// <summary>
        /// Creates a FunctionParameterAliasToken
        /// </summary>
        /// <param name="alias">the alias being used for the parameter.</param>
        public FunctionParameterAliasToken(string alias)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(alias, "alias");
            this.Alias = alias;
        }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// Gets the kind of this token
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.FunctionParameterAlias; }
        }

        /// <summary>
        /// The expected edm type of this parameter.
        /// </summary>
        internal IEdmTypeReference ExpectedParameterType { get; set; }

        /// <summary>
        /// Accept a <see cref="ISyntacticTreeVisitor{T}"/> to walk a tree of <see cref="QueryToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(ISyntacticTreeVisitor<T> visitor)
        {
            throw new NotImplementedException();
        }
    }
}