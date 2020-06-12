//---------------------------------------------------------------------
// <copyright file="SelectTreeNormalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Translate a select tree into the right format.
    /// </summary>
    internal static class SelectTreeNormalizer
    {
        /// <summary>
        /// Normalize a <see cref="SelectToken"/>.
        /// </summary>
        /// <param name="selectToken">The select token to normalize</param>
        /// <returns>Normalized SelectToken</returns>
        public static SelectToken NormalizeSelectTree(SelectToken selectToken)
        {
            // To normalize the select tree we need to:
            // invert the path tree on each of its select term tokens
            selectToken = NormalizeSelectPaths(selectToken);

            return selectToken;
        }

        private static SelectToken NormalizeSelectPaths(SelectToken selectToken)
        {
            if (selectToken != null)
            {
                // iterate through each select term token, and reverse the tree in its path property
                foreach (SelectTermToken term in selectToken.SelectTerms)
                {
                    term.PathToProperty = term.PathToProperty.Reverse();

                    // we also need to call the select token normalizer for this level to reverse the select paths
                    if (term.SelectOption != null)
                    {
                        term.SelectOption = NormalizeSelectPaths(term.SelectOption);
                    }
                }
            }

            return selectToken;
        }
    }
}