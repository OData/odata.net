//---------------------------------------------------------------------
// <copyright file="SelectExpandSyntacticUnifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System.Collections.Generic;

    /// <summary>
    /// Combine a top level select and expand token.
    /// </summary>
    internal static class SelectExpandSyntacticUnifier
    {
        /// <summary>
        /// Combine a top level select and expand token
        /// </summary>
        /// <param name="expand">the original expand token</param>
        /// <param name="select">the original select token</param>
        /// <returns>A new ExpandToken with the original select token embedded within a new top level expand token.</returns>
        public static ExpandToken Combine(ExpandToken expand, SelectToken select)
        {
            // build a new top level expand token embedding the top level select token.
            ExpandTermToken newTopLevelExpandTerm = new ExpandTermToken(new SystemToken(ExpressionConstants.It, null), select, expand);
            return new ExpandToken(new List<ExpandTermToken>() { newTopLevelExpandTerm });
        }
    }
}