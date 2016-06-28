//---------------------------------------------------------------------
// <copyright file="AnnotationFilterPattern.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Filter pattern class to determine whether an annotation name matches the pattern.
    /// </summary>
    internal abstract class AnnotationFilterPattern : IComparable<AnnotationFilterPattern>
    {
        /// <summary>
        /// The "*" pattern that includes all annotations.
        /// </summary>
        internal static readonly AnnotationFilterPattern IncludeAllPattern = new WildCardPattern(/*isExclude*/false);

        /// <summary>
        /// The "-*" pattern that excludes all annotations.
        /// </summary>
        internal static readonly AnnotationFilterPattern ExcludeAllPattern = new WildCardPattern(/*isExclude*/true);

        /// <summary>
        /// The pattern to match.
        /// </summary>
        protected readonly string Pattern;

        /// <summary>
        /// The '.' namespace separator.
        /// </summary>
        private const char NamespaceSeparator = '.';

        /// <summary>
        /// The '-' operator to indicate that the annotation should be excluded from read when it matches the pattern.
        /// </summary>
        private const char ExcludeOperator = '-';

        /// <summary>
        /// The wild card constant.
        /// </summary>
        private const string WildCard = "*";

        /// <summary>
        /// String constant for .*
        /// </summary>
        private const string DotStar = ".*";

        /// <summary>
        /// true if the annotation should be excluded from reading when its name matches this pattern; false otherwise.
        /// </summary>
        private readonly bool isExclude;

        /// <summary>
        /// Constructs a pattern instance to determine whether an annotation name matches the pattern.
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="isExclude">true if the annotation should be excluded from reading when its name matches this pattern; false otherwise.</param>
        private AnnotationFilterPattern(string pattern, bool isExclude)
        {
            Debug.Assert(!string.IsNullOrEmpty(pattern), "!string.IsNullOrEmpty(pattern)");
            this.isExclude = isExclude;
            this.Pattern = pattern;
        }

        /// <summary>
        /// true if the annotation should be excluded from reading when its name matches this pattern; false otherwise.
        /// </summary>
        internal virtual bool IsExclude
        {
            get
            {
                return this.isExclude;
            }
        }

        /// <summary>
        /// Compares the priority of current pattern with the priority of <paramref name="other"/>.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative priority of the patterns being compared. The return value has the following meanings:
        ///   -1 means this pattern has higher priority than <paramref name="other"/>.
        ///   0 means this pattern has the same priority as <paramref name="other"/>.
        ///   1 means this pattern has lower priority than <paramref name="other"/>.
        /// </returns>
        /// <param name="other">A pattern to compare with this pattern.</param>
        public int CompareTo(AnnotationFilterPattern other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");

            // If two pattern strings have the same priority, negation has higher priority.
            int priority = ComparePatternPriority(this.Pattern, other.Pattern);
            if (priority == 0)
            {
                return this.IsExclude == other.IsExclude ? 0 : (this.IsExclude ? -1 : 1);
            }

            return priority;
        }

        /// <summary>
        /// Creates a pattern instance to determine whether an annotation name matches the pattern.
        /// </summary>
        /// <param name="pattern">The pattern for this instance.</param>
        /// <returns>The newly created <see cref="AnnotationFilterPattern"/> instance.</returns>
        internal static AnnotationFilterPattern Create(string pattern)
        {
            ValidatePattern(pattern);

            bool isExclude = RemoveExcludeOperator(ref pattern);
            if (pattern == WildCard)
            {
                return isExclude ? ExcludeAllPattern : IncludeAllPattern;
            }

            if (pattern.EndsWith(DotStar, StringComparison.Ordinal))
            {
                Debug.Assert(pattern != DotStar, "pattern != DotStar");
                return new StartsWithPattern(pattern.Substring(0, pattern.Length - 1), isExclude);
            }

            return new ExactMatchPattern(pattern, isExclude);
        }

        /// <summary>
        /// Sorts the patterns in the array from highest to lowest priorities.
        /// </summary>
        /// <param name="pattersToSort">The source array to sort. When the method returns the items in this array instance will be rearragned.</param>
        internal static void Sort(AnnotationFilterPattern[] pattersToSort)
        {
            Array.Sort(pattersToSort);
        }

        /// <summary>
        /// Match the given annotation name against the pattern.
        /// </summary>
        /// <param name="annotationName">Annotation name in question.</param>
        /// <returns>Returns true if the given annotation name matches the pattern, false otherwise.</returns>
        internal abstract bool Matches(string annotationName);

        /// <summary>
        /// Compares the priority of <paramref name="pattern1"/> with <paramref name="pattern2"/>.
        /// </summary>
        /// <param name="pattern1">The left hand side pattern to compare.</param>
        /// <param name="pattern2">The right hand side pattern to compare.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative priority of the patterns being compared. The return value has the following meanings:
        ///   -1 means <paramref name="pattern1"/> has higher priority than <paramref name="pattern2"/>.
        ///   0 means <paramref name="pattern1"/> has same priority as <paramref name="pattern2"/>.
        ///   1 means <paramref name="pattern1"/> has lower priority than <paramref name="pattern2"/>.
        /// </returns>
        private static int ComparePatternPriority(string pattern1, string pattern2)
        {
            Debug.Assert(!string.IsNullOrEmpty(pattern1), "!string.IsNullOrEmpty(pattern1)");
            Debug.Assert(!string.IsNullOrEmpty(pattern2), "!string.IsNullOrEmpty(pattern2)");
            Debug.Assert(pattern1[0] != ExcludeOperator, "pattern1[0] != ExcludeOperator");
            Debug.Assert(pattern2[0] != ExcludeOperator, "pattern2[0] != ExcludeOperator");

            //// The relative priority of the pattern is base on the relative specificity of the patterns being compared. If pattern1 is under the namespace pattern2,
            //// pattern1 is more specific than pattern2 because pattern1 matches a subset of what pattern2 matches. We give higher priority to the pattern that is more specific.
            //// For example:
            ////  "ns.*" has higher priority than "*"
            ////  "ns.name" has higher priority than "ns.*"
            ////  "ns1.name" has same priority as "ns2.*"

            // Identical patterns have the same priority.
            if (pattern1 == pattern2)
            {
                return 0;
            }

            // WildCard is the least specific pattern and thus has the lowest priority.
            if (pattern1 == WildCard)
            {
                return 1;
            }

            if (pattern2 == WildCard)
            {
                return -1;
            }

            Debug.Assert(!pattern1.EndsWith(WildCard, StringComparison.Ordinal), "The trailing * should have already been stripped, e.g. ns.* => ns.");
            Debug.Assert(!pattern2.EndsWith(WildCard, StringComparison.Ordinal), "The trailing * should have already been stripped, e.g. ns.* => ns.");

            // If pattern1 starts with pattern2, pattern1 is either more specific than pattern2 or they belong to different namespaces.
            // Examples of pattern1 being more specific than pattern2: "ns.name" vs. "ns.", "ns.sub1.name" vs. "ns.sub1."
            // If they belong to different namespaces, technically they have the same relative priority and we should return 0. But it's fine
            // for our purpose to return -1 or 1 for simplicity sake since patterns of the same priority can be evaluated in any order.
            // Examples of pattern1 starts with pattern2 but there's no intersection in the set they match: "ns.name1.name2" vs "ns.name", "ns.name1.*" vs "ns.name"
            if (pattern1.StartsWith(pattern2, StringComparison.Ordinal))
            {
                return -1;
            }

            if (pattern2.StartsWith(pattern1, StringComparison.Ordinal))
            {
                return 1;
            }

            // Not under the same space.
            return 0;
        }

        /// <summary>
        /// Removes the exclude operator from the given pattern string.
        /// </summary>
        /// <param name="pattern">The input pattern to the method and will return the pattern without the exclude operator if it's found.</param>
        /// <returns>Returns true if the exclude operator is found and removed from the input pattern; false otherwise.</returns>
        private static bool RemoveExcludeOperator(ref string pattern)
        {
            Debug.Assert(!string.IsNullOrEmpty(pattern), "!string.IsNullOrEmpty(pattern)");
            if (pattern[0] == ExcludeOperator)
            {
                pattern = pattern.Substring(1);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validates the pattern.
        /// </summary>
        /// <param name="pattern">The pattern to validate.</param>
        private static void ValidatePattern(string pattern)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(pattern, "pattern");

            string patternWithoutMinusSign = pattern;
            RemoveExcludeOperator(ref patternWithoutMinusSign);

            if (patternWithoutMinusSign == WildCard)
            {
                return;
            }

            string[] segments = patternWithoutMinusSign.Split(NamespaceSeparator);
            int segmentCount = segments.Length;

            if (segmentCount == 1)
            {
                throw new ArgumentException(Strings.AnnotationFilterPattern_InvalidPatternMissingDot(pattern));
            }

            for (int idx = 0; idx < segmentCount; idx++)
            {
                string currentSegment = segments[idx];
                if (string.IsNullOrEmpty(currentSegment))
                {
                    throw new ArgumentException(Strings.AnnotationFilterPattern_InvalidPatternEmptySegment(pattern));
                }

                if (currentSegment != WildCard && currentSegment.Contains(WildCard))
                {
                    throw new ArgumentException(Strings.AnnotationFilterPattern_InvalidPatternWildCardInSegment(pattern));
                }

                bool isLastSegment = idx + 1 == segmentCount;
                if (currentSegment == WildCard && !isLastSegment)
                {
                    throw new ArgumentException(Strings.AnnotationFilterPattern_InvalidPatternWildCardMustBeInLastSegment(pattern));
                }
            }
        }

        /// <summary>
        /// The wild card pattern that matches everything.
        /// </summary>
        private sealed class WildCardPattern : AnnotationFilterPattern
        {
            /// <summary>
            /// Constructs the wild card pattern.
            /// </summary>
            /// <param name="isExclude">true if the annotation should be excluded from reading when its name matches this pattern; false otherwise.</param>
            internal WildCardPattern(bool isExclude) : base(WildCard, isExclude)
            {
            }

            /// <summary>
            /// Match the given annotation name against the pattern.
            /// </summary>
            /// <param name="annotationName">Annotation name in question.</param>
            /// <returns>Returns true if the given annotation name matches the pattern, false otherwise.</returns>
            internal override bool Matches(string annotationName)
            {
                Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
                return true;
            }
        }

        /// <summary>
        /// Pattern class to match any annotation name that starts with this pattern.
        /// </summary>
        private sealed class StartsWithPattern : AnnotationFilterPattern
        {
            /// <summary>
            /// Constructs the starts with pattern.
            /// </summary>
            /// <param name="pattern">The pattern to start with.</param>
            /// <param name="isExclude">true if the annotation should be excluded from reading when its name matches this pattern; false otherwise.</param>
            internal StartsWithPattern(string pattern, bool isExclude) : base(pattern, isExclude)
            {
                Debug.Assert(!string.IsNullOrEmpty(pattern), "!string.IsNullOrEmpty(pattern)");
                Debug.Assert(pattern.EndsWith(".", StringComparison.Ordinal), "pattern.EndsWith(\".\", StringComparison.Ordinal)");
            }

            /// <summary>
            /// Match the given annotation name against the pattern.
            /// </summary>
            /// <param name="annotationName">Annotation name in question.</param>
            /// <returns>Returns true if the given annotation name matches the pattern, false otherwise.</returns>
            internal override bool Matches(string annotationName)
            {
                Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
                return annotationName.StartsWith(this.Pattern, StringComparison.Ordinal);
            }
        }

        /// <summary>
        /// Pattern class to match an annotation name that is exactly the same as this pattern.
        /// </summary>
        private sealed class ExactMatchPattern : AnnotationFilterPattern
        {
            /// <summary>
            /// Constructs the exact match pattern.
            /// </summary>
            /// <param name="pattern">The exact pattern to match</param>
            /// <param name="isExclude">true if the annotation should be excluded from reading when its name matches this pattern; false otherwise.</param>
            internal ExactMatchPattern(string pattern, bool isExclude) : base(pattern, isExclude)
            {
                Debug.Assert(!string.IsNullOrEmpty(pattern), "!string.IsNullOrEmpty(pattern)");
            }

            /// <summary>
            /// Match the given annotation name against the pattern.
            /// </summary>
            /// <param name="annotationName">Annotation name in question.</param>
            /// <returns>Returns true if the given annotation name matches the pattern, false otherwise.</returns>
            internal override bool Matches(string annotationName)
            {
                Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
                return annotationName.Equals(this.Pattern, StringComparison.Ordinal);
            }
        }
    }
}
