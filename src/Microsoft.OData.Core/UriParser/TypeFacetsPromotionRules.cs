//---------------------------------------------------------------------
// <copyright file="TypeFacetsPromotionRules.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;

    /// <summary>
    /// Defines the promotion rules for type facets.
    /// </summary>
    public class TypeFacetsPromotionRules
    {
        /// <summary>
        /// Computes the promoted precision value for left and right.
        /// The default implementation works as follows:
        ///   1) if both left and right are null, return null;
        ///   2) if only one is null, return the other;
        ///   3) otherwise, return the larger.
        /// </summary>
        /// <param name="left">Left-hand-side precision value.</param>
        /// <param name="right">Right-hand-side precision value.</param>
        /// <returns>The promoted precision value.</returns>
        public virtual int? GetPromotedPrecision(int? left, int? right)
        {
            return left  == null ? right :
                   right == null ? left  :
                   Math.Max((int)left, (int)right);
        }

        /// <summary>
        /// Computes the promoted scale value for left and right.
        /// The default implementation works as follows:
        ///   1) if both left and right are null, return null;
        ///   2) if only one is null, return the other;
        ///   3) otherwise, return the larger.
        /// </summary>
        /// <param name="left">Left-hand-side scale value.</param>
        /// <param name="right">Right-hand-side scale value.</param>
        /// <returns>The promoted scale value.</returns>
        public virtual int? GetPromotedScale(int? left, int? right)
        {
            return left  == null ? right :
                   right == null ? left  :
                   Math.Max((int)left, (int)right);
        }
    }
}
