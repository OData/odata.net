//---------------------------------------------------------------------
// <copyright file="RemoveWildcardVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using Microsoft.OData.Client.ALinq.UriParser;

    /// <summary>
    /// Remove any wildcard characters from a given path.
    /// </summary>
    internal class RemoveWildcardVisitor : IPathSegmentTokenVisitor
    {
        /// <summary>
        /// The previous token, used as a cursor so that we can cut off the
        /// next pointer of the previous token if we find a wildcard.
        /// </summary>
        private PathSegmentToken previous = null;

        /// <summary>
        /// Translate a SystemToken, this is illegal so this always throws.
        /// </summary>
        /// <param name="tokenIn">The SystemToken to translate.</param>
        public void Visit(SystemToken tokenIn)
        {
            throw new NotSupportedException(Strings.ALinq_IllegalSystemQueryOption(tokenIn.Identifier));
        }

        /// <summary>
        /// Translate a NonSystemToken.
        /// </summary>
        /// <param name="tokenIn">The NonSystemToken to translate.</param>
        public void Visit(NonSystemToken tokenIn)
        {
            if (tokenIn.Identifier != UriHelper.ASTERISK.ToString())
            {
                if (tokenIn.NextToken == null)
                {
                    return;
                }

                previous = tokenIn;
                tokenIn.NextToken.Accept(this);
            }
            else
            {
                previous.NextToken = null;
                return;
            }
        }
    }
}