//---------------------------------------------------------------------
// <copyright file="AnnotationFilter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics;
using System.Linq;

namespace Microsoft.OData
{
    /// <summary>
    /// Filter class to determine whether or not to read an annotation.
    /// </summary>
    internal class AnnotationFilter
    {
        /// <summary>
        /// Filter that maches all annotation names.
        /// </summary>
        private static readonly AnnotationFilter IncludeAll = new IncludeAllFilter();

        /// <summary>
        /// Filter than maches no annotation names.
        /// </summary>
        private static readonly AnnotationFilter ExcludeAll = new ExcludeAllFilter();

        /// <summary>
        /// Separator for annotation filter patterns.
        /// </summary>
        private static readonly char[] AnnotationFilterPatternSeparator = new[] { ',' };

        /// <summary>
        /// Patterns to match, sorted in the order of higher to lower priorities to match.
        /// </summary>
        private readonly AnnotationFilterPattern[] prioritizedPatternsToMatch;

        /// <summary>
        /// Private constructor to create a filter from comma delimited patterns to match to include or exclude annotations.
        /// </summary>
        /// <param name="prioritizedPatternsToMatch">Patters to match to include or exclude annotations.</param>
        private AnnotationFilter(AnnotationFilterPattern[] prioritizedPatternsToMatch)
        {
#if DEBUG
            for (int idx = 1; idx < prioritizedPatternsToMatch.Length; idx++)
            {
                Debug.Assert(
                    prioritizedPatternsToMatch[idx - 1].CompareTo(prioritizedPatternsToMatch[idx]) < 1,
                    "The prioritizedPattersToMatch array should have been sorted in the order of higher to lower priorities to match.");
            }
#endif
            this.prioritizedPatternsToMatch = prioritizedPatternsToMatch;
        }

        /// <summary>
        /// Create a filter from comma delimited patterns to match to include or exclude annotations.
        /// </summary>
        /// <param name="filter">Comma delimited patterns to match to include or exclude annotations.</param>
        /// <returns>The newly created filter.</returns>
        internal static AnnotationFilter Create(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return ExcludeAll;
            }

            AnnotationFilterPattern[] prioritizedPatternsToMatch = filter
                .Split(AnnotationFilterPatternSeparator)
                .Select(pattern => AnnotationFilterPattern.Create(pattern.Trim()))
                .ToArray();

            AnnotationFilterPattern.Sort(prioritizedPatternsToMatch);

            if (prioritizedPatternsToMatch[0] == AnnotationFilterPattern.IncludeAllPattern)
            {
                return IncludeAll;
            }

            if (prioritizedPatternsToMatch[0] == AnnotationFilterPattern.ExcludeAllPattern)
            {
                return ExcludeAll;
            }

            return new AnnotationFilter(prioritizedPatternsToMatch);
        }

        /// <summary>
        /// Create a filter that inlcude all.
        /// </summary>
        /// <returns>The include all filter.</returns>
        internal static AnnotationFilter CreateInclueAllFilter()
        {
            return new IncludeAllFilter();
        }

        /// <summary>
        /// Returns true to indicate that the annotation with the name <paramref name="annotationName"/> should be read, false otherwise.
        /// </summary>
        /// <param name="annotationName">The name of the annotation in question.</param>
        /// <returns>Returns true to indicate that the annotation with the name <paramref name="annotationName"/> should be read, false otherwise.</returns>
        internal virtual bool Matches(string annotationName)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(annotationName, "annotationName");

            // Find the highest priority pattern that maches and return true only if that pattern is not exclude.
            foreach (AnnotationFilterPattern pattern in this.prioritizedPatternsToMatch)
            {
                if (pattern.Matches(annotationName))
                {
                    return !pattern.IsExclude;
                }
            }

            return false;
        }

        /// <summary>
        /// Filter to read all annotations.
        /// </summary>
        private sealed class IncludeAllFilter : AnnotationFilter
        {
            /// <summary>
            /// Private default constructor.
            /// </summary>
            internal IncludeAllFilter() : base(new AnnotationFilterPattern[0])
            {
            }

            /// <summary>
            /// Returns true to indicate that the annotation with the name <paramref name="annotationName"/> should be read, false otherwise.
            /// </summary>
            /// <param name="annotationName">The name of the annotation in question.</param>
            /// <returns>Returns true to indicate that the annotation with the name <paramref name="annotationName"/> should be read, false otherwise.</returns>
            internal override bool Matches(string annotationName)
            {
                ExceptionUtils.CheckArgumentStringNotNullOrEmpty(annotationName, "annotationName");
                return true;
            }
        }

        /// <summary>
        /// Filter to read no annotation.
        /// </summary>
        private sealed class ExcludeAllFilter : AnnotationFilter
        {
            /// <summary>
            /// Private default constructor.
            /// </summary>
            internal ExcludeAllFilter() : base(new AnnotationFilterPattern[0])
            {
            }

            /// <summary>
            /// Returns true to indicate that the annotation with the name <paramref name="annotationName"/> should be read, false otherwise.
            /// </summary>
            /// <param name="annotationName">The name of the annotation in question.</param>
            /// <returns>Returns true to indicate that the annotation with the name <paramref name="annotationName"/> should be read, false otherwise.</returns>
            internal override bool Matches(string annotationName)
            {
                ExceptionUtils.CheckArgumentStringNotNullOrEmpty(annotationName, "annotationName");
                return false;
            }
        }
    }
}
