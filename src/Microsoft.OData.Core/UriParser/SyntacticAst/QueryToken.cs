//---------------------------------------------------------------------
// <copyright file="QueryToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    #region Namespaces

    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData;

    #endregion

    /// <summary>
    /// Base class for all lexical tokens of OData query.
    /// </summary>
    public abstract class QueryToken
    {
        /// <summary>
        /// Empty list of arguments.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2105:ArrayFieldsShouldNotBeReadOnly",
            Justification = "Modeled after Type.EmptyTypes")] public static readonly QueryToken[] EmptyTokens =
                new QueryToken[0];

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public abstract QueryTokenKind Kind { get; }

        /// <summary>
        /// Accept a <see cref="ISyntacticTreeVisitor{T}"/> to walk a tree of <see cref="QueryToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public abstract T Accept<T>(ISyntacticTreeVisitor<T> visitor);
    }
}