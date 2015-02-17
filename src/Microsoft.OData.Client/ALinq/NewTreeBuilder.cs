//---------------------------------------------------------------------
// <copyright file="NewTreeBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using Microsoft.OData.Client.ALinq.UriParser;

    /// <summary>
    /// Construct a new copy of an existing tree
    /// </summary>
    internal class NewTreeBuilder : IPathSegmentTokenVisitor<PathSegmentToken>
    {
        /// <summary>
        /// Visit a SystemToken
        /// </summary>
        /// <param name="tokenIn">The SystemToken to visit</param>
        /// <returns>Always throws, since a SystemToken is illegal in a select or expand path.</returns>
        public PathSegmentToken Visit(SystemToken tokenIn)
        {
            throw new NotSupportedException(Strings.ALinq_IllegalSystemQueryOption(tokenIn.Identifier));
        }

        /// <summary>
        /// Visit a NonSystemToken
        /// </summary>
        /// <param name="tokenIn">The non system token to visit</param>
        /// <returns>A new copy of the input token.</returns>
        public PathSegmentToken Visit(NonSystemToken tokenIn)
        {
            if (tokenIn == null)
            {
                return null;
            }
            else
            {
                PathSegmentToken subToken = tokenIn.NextToken != null ? tokenIn.NextToken.Accept(this) : null;
                return new NonSystemToken(tokenIn.Identifier, tokenIn.NamedValues, subToken);
            }
        }
    }
}