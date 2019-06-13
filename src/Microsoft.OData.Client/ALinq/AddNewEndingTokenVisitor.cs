//---------------------------------------------------------------------
// <copyright file="AddNewEndingTokenVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using Microsoft.OData.Client.ALinq.UriParser;

    /// <summary>
    /// Traverse the tree to the last token, then add a new token there if provided.
    /// </summary>
    internal class AddNewEndingTokenVisitor : IPathSegmentTokenVisitor
    {
        /// <summary>
        /// The new token to add to the tree
        /// </summary>
        private readonly PathSegmentToken newTokenToAdd;

        /// <summary>
        /// Create a new AddNewEndingTokenVisitor, with the new token to add at the end.
        /// </summary>
        /// <param name="newTokenToAdd">a new token to add at the end of the path, can be null</param>
        public AddNewEndingTokenVisitor(PathSegmentToken newTokenToAdd)
        {
            this.newTokenToAdd = newTokenToAdd;
        }

        /// <summary>
        /// Traverse a SystemToken. Always throws because a SystemToken is illegal in this case.
        /// </summary>
        /// <param name="tokenIn">The system token to traverse</param>
        public void Visit(SystemToken tokenIn)
        {
            throw new NotSupportedException(Strings.ALinq_IllegalSystemQueryOption(tokenIn.Identifier));
        }

        /// <summary>
        /// Traverse a NonSystemToken.
        /// </summary>
        /// <param name="tokenIn">The NonSystemToken to traverse.</param>
        public void Visit(NonSystemToken tokenIn)
        {
            if (tokenIn.NextToken == null)
            {
                if (newTokenToAdd != null)
                {
                    tokenIn.NextToken = newTokenToAdd;
                }
            }
            else
            {
                tokenIn.NextToken.Accept(this);
            }
        }
    }
}