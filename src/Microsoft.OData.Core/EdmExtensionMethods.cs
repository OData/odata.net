//---------------------------------------------------------------------
// <copyright file="EdmExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace Microsoft.OData
{
    internal static class EdmExtensionMethods
    {
        /// <summary>
        /// Find the navigation target which is <paramref name="navigationProperty"/> of current <paramref name="navigationSource"/> targets.
        /// </summary>
        /// <param name="navigationSource">The navigation source to find.</param>
        /// <param name="navigationProperty">The navigation property</param>
        /// <param name="matchBindingPath">The function used to determine if the binding path matches.</param>
        /// <returns>The navigation target which matches the binding path.</returns>
        public static IEdmNavigationSource FindNavigationTarget(this IEdmNavigationSource navigationSource, IEdmNavigationProperty navigationProperty, Func<IEdmPathExpression, bool> matchBindingPath)
        {
            Debug.Assert(navigationSource != null);
            Debug.Assert(navigationProperty != null);
            Debug.Assert(matchBindingPath != null);

            if (navigationProperty.ContainsTarget)
            {
                return navigationSource.FindNavigationTarget(navigationProperty);
            }

            IEnumerable<IEdmNavigationPropertyBinding> bindings = navigationSource.FindNavigationPropertyBindings(navigationProperty);

            if (bindings != null)
            {
                foreach (var binding in bindings)
                {
                    if (matchBindingPath(binding.Path))
                    {
                        return binding.Target;
                    }
                }
            }

            return new UnknownEntitySet(navigationSource, navigationProperty);
        }

        /// <summary>
        /// Find the navigation target which is <paramref name="navigationProperty"/> of current <paramref name="navigationSource"/> targets.
        /// The function is specifically used in Uri parser.
        /// </summary>
        /// <param name="navigationSource">The navigation source to find.</param>
        /// <param name="navigationProperty">The navigation property</param>
        /// <param name="matchBindingPath">The function used to determine if the binding path matches.</param>
        /// <param name="parsedSegments">The parsed segments in path, which is used to match binding path.</param>
        /// <param name="bindingPath">The output binding path of the navigation property which matches the <paramref name="parsedSegments"/></param>
        /// <returns>The navigation target which matches the binding path.</returns>
        public static IEdmNavigationSource FindNavigationTarget(this IEdmNavigationSource navigationSource, IEdmNavigationProperty navigationProperty, Func<IEdmPathExpression, IReadOnlyList<ODataPathSegment>, bool> matchBindingPath, IReadOnlyList<ODataPathSegment> parsedSegments, out IEdmPathExpression bindingPath)
        {
            Debug.Assert(navigationSource != null);
            Debug.Assert(navigationProperty != null);
            Debug.Assert(matchBindingPath != null);
            Debug.Assert(parsedSegments != null);

            bindingPath = null;

            if (navigationProperty.ContainsTarget)
            {
                return navigationSource.FindNavigationTarget(navigationProperty);
            }

            IEnumerable<IEdmNavigationPropertyBinding> bindings = navigationSource.FindNavigationPropertyBindings(navigationProperty);

            if (bindings != null)
            {
                foreach (var binding in bindings)
                {
                    if (matchBindingPath(binding.Path, parsedSegments))
                    {
                        bindingPath = binding.Path;
                        return binding.Target;
                    }
                }
            }

            if (typeof(IEdmUnknownEntitySet).IsAssignableFrom(navigationSource.GetType()))
            {
                return new UnknownEntitySet(navigationSource, navigationProperty);
            }
            else
            {
                return navigationSource.FindNavigationTarget(navigationProperty);
            }
        }

        /// <summary>
        /// Decide whether <paramref name="currentNavigationSource"/> with type <paramref name="currentResourceType"/> should have key.
        /// </summary>
        /// <param name="currentNavigationSource">The navigation source to be evaluated.</param>
        /// <param name="currentResourceType">The resource type to be evaluated.</param>
        /// <returns>True if the navigation source should have key.</returns>
        public static bool HasKey(IEdmNavigationSource currentNavigationSource, IEdmStructuredType currentResourceType)
        {
            if (currentResourceType is IEdmComplexType)
            {
                return false;
            }

            if (currentNavigationSource is IEdmEntitySet)
            {
                return true;
            }

            var currentContainedEntitySet = currentNavigationSource as IEdmContainedEntitySet;
            if (currentContainedEntitySet != null && currentContainedEntitySet.NavigationProperty.Type.TypeKind() == EdmTypeKind.Collection)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parse an enum integral value to enum member.
        /// </summary>
        /// <param name="enumType">edm enum type</param>
        /// <param name="value">input integral value.</param>
        /// <param name="enumMember">parsed result.</param>
        /// <returns>true if parse succeeds, false if parse fails.</returns>
        public static bool TryParse(this IEdmEnumType enumType, long value, out IEdmEnumMember enumMember)
        {
            enumMember = null;
            foreach (IEdmEnumMember member in enumType.Members)
            {
                if (member.Value.Value == value)
                {
                    enumMember = member;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the given member name exists in the enum type.
        /// </summary>
        /// <param name="enumType">The enum type.</param>
        /// <param name="memberName">The member name to check.</param>
        /// <param name="comparison">The comparison type to use for string comparison. Default is Ordinal.</param>
        /// <returns>True if the member name exists in the enum type; otherwise, false.</returns>
        public static bool HasMember(this IEdmEnumType enumType, string memberName, StringComparison comparison = StringComparison.Ordinal)
        {
            return enumType.HasMember(memberName.AsSpan(), comparison);
        }

        /// <summary>
        /// Determines whether the specified enum type contains a member with the given name, using the specified string comparison.
        /// </summary>
        /// <param name="enumType">The <see cref="IEdmEnumType"/> to search for the member.</param>
        /// <param name="memberName">The name of the member to locate within the enum type.</param>
        /// <param name="exactMemberName">When this method returns, contains the <see cref="IEdmEnumMember"/> that matches the specified name, if a match is found; otherwise, null.</param>
        /// <param name="comparison">The comparison type to use for string comparison. Default is Ordinal.</param>
        /// <returns>True if the member name exists in the enum type; otherwise, false.</returns>
        public static IEdmEnumMember FindMember(this IEdmEnumType enumType, string memberName, StringComparison comparison = StringComparison.Ordinal)
        {
            return enumType.FindMember(memberName.AsSpan(), comparison);
        }

        /// <summary>
        /// Checks if the given member name exists in the enum type.
        /// </summary>
        /// <param name="enumType">The enum type to search for the member.</param>
        /// <param name="memberName">The name of the member to locate, represented as a <see cref="ReadOnlySpan{T}"/> of characters.</param>
        /// <param name="comparison">The <see cref="StringComparison"/> to use when comparing the member names. The default is <see cref="StringComparison.Ordinal"/>.</param>
        /// <returns>True if the member name exists in the enum type; otherwise, false.</returns>
        public static bool HasMember(this IEdmEnumType enumType, ReadOnlySpan<char> memberName, StringComparison comparison = StringComparison.Ordinal)
        {
            foreach (IEdmEnumMember member in enumType.Members)
            {
                if (memberName.Equals(member.Name.AsSpan(), comparison))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the given member name exists in the enum type.
        /// </summary>
        /// <param name="enumType">The enum type to search for the member.</param>
        /// <param name="memberName">The name of the member to locate, represented as a <see cref="ReadOnlySpan{T}"/> of characters.</param>
        /// <param name="exactMemberName">When this method returns, contains the <see cref="IEdmEnumMember"/> that matches the specified name, if a match is found; otherwise, null.</param>
        /// <param name="comparison">The <see cref="StringComparison"/> to use when comparing the member names. The default is <see cref="StringComparison.Ordinal"/>.</param>
        /// <returns>True if the member name exists in the enum type; otherwise, false.</returns>
        public static IEdmEnumMember FindMember(this IEdmEnumType enumType, ReadOnlySpan<char> memberName, StringComparison comparison = StringComparison.Ordinal)
        {
            foreach (IEdmEnumMember member in enumType.Members)
            {
                if (memberName.Equals(member.Name.AsSpan(), comparison))
                {
                    return member;
                }
            }

            return null;
        }

        /// <summary>
        /// Parses the specified integral value into a comma-separated string of flag names based on the members of the given EDM enum type.
        /// </summary>
        /// <param name="enumType">The EDM enum type containing the members to parse.</param>
        /// <param name="value">The integral value to parse into flag names.</param>
        /// <returns>A comma-separated string of flag names corresponding to the set bits in the specified value. Returns null otherwise.</returns>
        public static string ParseFlagsFromIntegralValue(this IEdmEnumType enumType, long value)
        {
            // Special handling for 0: return the name of the member whose value is 0 (e.g., "None"), if present.
            // In Flags Enum, 0 typically represents no flags set.
            if (value == 0)
            {
                return enumType.Members.FirstOrDefault(m => m.Value.Value == 0)?.Name;
            }

            List<string> result = new List<string>();
            List<IEdmEnumMember> members = enumType.Members.ToList();

            long remaining = value;

            // Iterate members in reverse order to match higher-value flags first.
            for (int index = members.Count - 1; index >= 0; index--)
            {
                long flagValue = members[index].Value.Value;
                if (flagValue != 0 && (remaining & flagValue) == flagValue)
                {
                    result.Add(members[index].Name);
                    remaining &= ~flagValue; // Remove matched bits     
                }
            }

            // Reverse the result to maintain original order and return as comma-separated string if all bits were matched.
            return result.Count > 0 && remaining == 0 ? string.Join(",", result.Reverse<string>()) : null;
        }

        /// <summary>
        /// Parses a comma-separated string of enum member names into a validated, formatted string containing only valid members of the specified <see cref="IEdmEnumType"/>.
        /// </summary>
        /// <param name="enumType">The EDM enum type to validate the enum member names against.</param>
        /// <param name="memberName">A comma-separated string containing the names of enum members to parse and validate.</param>
        /// <param name="comparison">The <see cref="StringComparison"/> to use when comparing the provided member names against the enum type's
        /// defined members.</param>
        /// <returns>A formatted string containing the validated enum member names, separated by commas and trimmed of whitespace. Otherwise, null or empty string.</returns>
        public static string ParseFlagsFromStringValue(this IEdmEnumType enumType, string memberName, StringComparison comparison)
        {
            ReadOnlySpan<char> memberNameSpan = memberName.AsSpan();

            // Check if string starts or ends with a comma, which is invalid.
            if (memberNameSpan.IsEmpty || memberNameSpan[0] == ',' || memberNameSpan[^1] == ',')
            {
                return null;
            }

            StringBuilder stringBuilder = new StringBuilder();
            int startIndex = 0, endIndex = 0;
            while (endIndex < memberName.Length)
            {
                while (endIndex < memberName.Length && memberName[endIndex] != ',')
                {
                    endIndex++;
                }

                ReadOnlySpan<char> currentValue = memberNameSpan[startIndex..endIndex].Trim();
                IEdmEnumMember edmEnumMember = enumType.FindMember(currentValue, comparison);
                if (edmEnumMember == null)
                {
                    return null;
                }

                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(',');
                }

                stringBuilder.Append(edmEnumMember.Name);
                startIndex = endIndex + 1;
                endIndex = startIndex;
            }

            return stringBuilder.Length == 0 ? null : stringBuilder.ToString();
        }

        /// <summary>
        /// Determines whether the specified integral value is a valid combination of flags for the given enumeration type.
        /// </summary>
        /// <param name="enumType">The enumeration type to validate against. Must represent a flags enumeration.</param>
        /// <param name="memberIntegralValue">The integral value to validate as a combination of flags.</param>
        /// <returns><see langword="true"/> if the specified value is a valid combination of the flags defined in the
        /// enumeration; otherwise, <see langword="false"/>.</returns>
        public static bool IsValidFlagsEnumValue(this IEdmEnumType enumType, long memberIntegralValue)
        {
            if(enumType == null || !enumType.IsFlags)
            {
                return false;
            }

            long allFlagsMask = 0;
            foreach (IEdmEnumMember member in enumType.Members)
            {
                allFlagsMask |= (long)member.Value.Value;
            }

            return (memberIntegralValue & ~allFlagsMask) == 0;
        }
    }
}
