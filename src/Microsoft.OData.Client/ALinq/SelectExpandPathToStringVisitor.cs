//---------------------------------------------------------------------
// <copyright file="SelectExpandPathToStringVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using Microsoft.OData.Client.ALinq.UriParser;
    using Microsoft.OData;

    /// <summary>
    /// Translate from an expand path token to a string.
    /// </summary>
    internal class SelectExpandPathToStringVisitor : IPathSegmentTokenVisitor<string>
    {
        /// <summary>
        /// The beginning of a sub-select clause
        /// </summary>
        public const string SelectClause = "($select=";

        /// <summary>
        /// The beginning of a sub-expand clause
        /// </summary>
        public const string StartingExpandClause = "($expand=";

        /// <summary>
        /// The beginning of an expand clause for the current level.
        /// </summary>
        public const string NonStartingExpandClause = "$expand=";

        /// <summary>
        /// Translate a system token, always throws.
        /// </summary>
        /// <param name="tokenIn">the system token.</param>
        /// <returns>Always throws, because a system token is illegal in this case.</returns>
        public string Visit(SystemToken tokenIn)
        {
            throw new NotSupportedException(Strings.ALinq_IllegalSystemQueryOption(tokenIn.Identifier));
        }

        /// <summary>
        /// Translate a NonSystemToken into a select or expand string.
        /// </summary>
        /// <param name="tokenIn">the non sytem token to translate</param>
        /// <returns>The string representation of a given NonSystemToken</returns>
        public string Visit(NonSystemToken tokenIn)
        {
            if (tokenIn.NextToken == null)
            {
                return tokenIn.Identifier;
            }
            else
            {
                if (tokenIn.NextToken.IsStructuralProperty)
                {
                    PathSegmentToken firstNonStructuralProperty;
                    string selectClauses = WriteNextStructuralProperties(tokenIn.NextToken, out firstNonStructuralProperty);
                    if (firstNonStructuralProperty != null)
                    {
                        return tokenIn.Identifier + SelectClause + selectClauses + UriHelper.SEMICOLON + NonStartingExpandClause + firstNonStructuralProperty.Accept(this) + UriHelper.RIGHTPAREN;
                    }
                    else
                    {
                        return tokenIn.Identifier + SelectClause + selectClauses + UriHelper.RIGHTPAREN;
                    }
                }
                else
                {
                    return tokenIn.Identifier + StartingExpandClause + tokenIn.NextToken.Accept(this) + UriHelper.RIGHTPAREN;
                }
            }
        }

        /// <summary>
        /// Follow a chain of structrual properties until we hit a non-structural property
        /// </summary>
        /// <param name="firstStructuralProperty">the first structural property we hit</param>
        /// <param name="firstNonStructuralProperty">the first non structural property we hit</param>
        /// <returns>a comma separated list of structural properties</returns>
        private static string WriteNextStructuralProperties(PathSegmentToken firstStructuralProperty, out PathSegmentToken firstNonStructuralProperty)
        {
            firstNonStructuralProperty = firstStructuralProperty;
            string stringToWrite = "";
            while (firstNonStructuralProperty.IsStructuralProperty)
            {
                if (firstNonStructuralProperty.NextToken != null)
                {
                    if (firstNonStructuralProperty.NextToken.IsStructuralProperty)
                    {
                        stringToWrite += firstNonStructuralProperty.Identifier + ",";
                    }
                    else
                    {
                        stringToWrite += firstNonStructuralProperty.Identifier;
                    }

                    firstNonStructuralProperty = firstNonStructuralProperty.NextToken;
                }
                else
                {
                    stringToWrite += firstNonStructuralProperty.Identifier;
                    firstNonStructuralProperty = null;
                    return stringToWrite;
                }
            }

            return stringToWrite;
        }
    }
}