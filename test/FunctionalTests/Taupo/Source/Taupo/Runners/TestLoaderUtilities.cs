//---------------------------------------------------------------------
// <copyright file="TestLoaderUtilities.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Runners
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// This class provides utility methods used when loading tests
    /// </summary>
    public static class TestLoaderUtilities
    {
        /// <summary>
        /// Gets the included test items for the given module and suite
        /// </summary>
        /// <param name="module">The test module data</param>
        /// <param name="suite">The test suite</param>
        /// <param name="logWriter">optional log writer in case something goes wrong</param>
        /// <returns>The included test items without the excluded items</returns>
        public static IEnumerable<TestItemData> GetIncludedVariations(TestModuleData module, TestSuite suite, ITestLogWriter logWriter)
        {
            ExceptionUtilities.CheckArgumentNotNull(suite, "suite");
            var included = suite.Items.Where(i => i.IsIncluded).Select(i => i.Name);
            var excluded = suite.Items.Where(i => !i.IsIncluded).Select(i => i.Name);
            return GetIncludedVariations(module, included, excluded, logWriter);
        }

        /// <summary>
        /// Gets the included variations for the given module
        /// </summary>
        /// <param name="module">The test module data</param>
        /// <param name="includedPaths">The included paths</param>
        /// <param name="excludedPaths">The excluded paths</param>
        /// <param name="logWriter">optional log writer in case something goes wrong</param>
        /// <returns>The included test items without the excluded items</returns>
        internal static IEnumerable<TestItemData> GetIncludedVariations(TestModuleData module, IEnumerable<string> includedPaths, IEnumerable<string> excludedPaths, ITestLogWriter logWriter)
        {
            ExceptionUtilities.CheckArgumentNotNull(module, "module");
            ExceptionUtilities.CheckArgumentNotNull(includedPaths, "includedPaths");
            ExceptionUtilities.CheckArgumentNotNull(excludedPaths, "excludedPaths");

            var allPossibleItems = module.GetAllChildrenRecursive().Where(i => i.IsVariation).ToList();

            var includedItems = new List<TestItemData>();
            bool anyIncludedPath = false;
            foreach (var includedPath in includedPaths)
            {
                anyIncludedPath = true;

                var toAdd = FilterItemsByPath(allPossibleItems, includedPath).ToList();
                if (toAdd.Count > 0)
                {
                    includedItems.AddRange(toAdd);

                    // TODO: should we remove them?
                    // It seems like it would make subsequent lookups faster and avoid the possibility of double-including something
                    toAdd.ForEach(i => allPossibleItems.Remove(i));
                }
                else if (logWriter != null)
                {
                    var safeString = includedPath.Replace('/', '\\');
                    logWriter.WriteLine(LogLevel.Warning, string.Format(CultureInfo.InvariantCulture, "No test items found for path '{0}' in module '{1}'", safeString, module.Metadata.Name));
                }
            }

            // special case, if no includes are specified, just take everything
            if (!anyIncludedPath)
            {
                includedItems = allPossibleItems;
            }

            foreach (var excludedPath in excludedPaths)
            {
                var toRemove = FilterItemsByPath(includedItems, excludedPath).ToList();
                toRemove.ForEach(i => includedItems.Remove(i));
            }

            return includedItems;
        }

        /// <summary>
        /// Filters the given set of test items to be only those that fall under the given path
        /// </summary>
        /// <param name="items">The items to filter</param>
        /// <param name="path">The path to filter by</param>
        /// <returns>The filtered items</returns>
        internal static IList<TestItemData> FilterItemsByPath(IList<TestItemData> items, string path)
        {
            // special case for root paths
            if (path == ".")
            {
                return items;
            }

            // might as well optimize this case
            if (items.Count == 0)
            {
                return items;
            }

            // normalize delimiter, since some suite files have it wrong
            path = path.Replace('\\', '/');

            Func<string, bool> isMatch;
            if (!path.Contains('*'))
            {
                isMatch = name => ItemPathMatches(name, path);
            }
            else
            {
                var regexPattern = Regex.Escape(path);
                regexPattern = regexPattern.Replace(Regex.Escape("*"), ".*");
                regexPattern = "^" + regexPattern + "$"; // note that this does not follow the same 'StartsWith' convention, though adding a '*' can easily get the same behavior
                var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
                isMatch = regex.IsMatch;
            }

            return items.Where(t => isMatch(GetFullPath(t))).ToList();
        }

        /// <summary>
        /// Helper method for deciding if a given item path matches the path that's being filtered.
        /// Returns true for perfect matches or prefix-matches where the prefix is a parent item's name
        /// Does not support partial matches within an item name
        /// </summary>
        /// <param name="itemPath">The item's path</param>
        /// <param name="pathToFilter">The path being filtered</param>
        /// <returns>True if the path matches, false otherwise</returns>
        internal static bool ItemPathMatches(string itemPath, string pathToFilter)
        {
            if (itemPath.Equals(pathToFilter, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (itemPath.StartsWith(pathToFilter, StringComparison.OrdinalIgnoreCase))
            {
                // if it is a prefix, it must fall on the parent node. Partial name matches are not allowed.
                return pathToFilter[pathToFilter.Length - 1] == '/' || itemPath[pathToFilter.Length] == '/';
            }

            return false;
        }

        private static string GetFullPath(TestItemData item)
        {
            // module isn't represented in the path
            if (item.Parent == null)
            {
                return string.Empty;
            }

            string name = item.Metadata.Name;

            string parentName = GetFullPath(item.Parent);
            if (!string.IsNullOrEmpty(parentName))
            {
                name = parentName + '/' + name;
            }

            return name;
        }
    }
}