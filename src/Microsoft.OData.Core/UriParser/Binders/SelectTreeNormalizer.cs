//---------------------------------------------------------------------
// <copyright file="SelectTreeNormalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Translate a select tree into the right format to be used with an expand tree.
    /// </summary>
    internal sealed class SelectTreeNormalizer
    {
        /// <summary>
        /// Normalize a SelectToken into something that can be used to trim an expand tree.
        /// </summary>
        /// <param name="treeToNormalize">The select token to normalize</param>
        /// <returns>Normalized SelectToken</returns>
        public static SelectToken NormalizeSelectTree(SelectToken treeToNormalize)
        {
            foreach (SelectTermToken term in treeToNormalize.SelectTerms)
            {
                term.PathToProperty = term.PathToProperty.Reverse();

                // we also need to call the select token normalizer for this level to reverse the select paths
                if (term.SelectOption != null)
                {
                    term.SelectOption = NormalizeSelectTree(term.SelectOption);
                }

                if (term.ExpandOption != null)
                {
                  //  term.ExpandOption = NormalizePaths(term.ExpandOption);
                }
            }

            // to normalize a select token we just need to invert its paths, so that
            // we match the ordering on an ExpandToken.
            return treeToNormalize;
        }
    }
}