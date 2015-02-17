//---------------------------------------------------------------------
// <copyright file="Enumerable.Ext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.DataDriven;

    /// <summary>
    /// IEnumerable extensions
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// To delimited string
        /// </summary>
        /// <param name="source">The source strings</param>
        /// <param name="separator">The separator</param>
        /// <returns>The string</returns>
        public static string ToDelimitedString(this IEnumerable<string> source, string separator)
        {
            return string.Join(separator, source.ToArray());
        }

        /// <summary>
        /// To data driven parameter data
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="collection">The item collecion</param>
        /// <returns>The parameter data</returns>
        public static ParameterData<T> ToParamData<T>(this IEnumerable<T> collection)
        {
            return DataDrivenTest.CreateData(collection.ToArray());
        }

        /// <summary>
        /// Get by title
        /// </summary>
        /// <param name="links">The links</param>
        /// <param name="title">The title</param>
        /// <returns>The link</returns>
        public static Link GetByTitle(this IEnumerable<Link> links, string title)
        {
            return links.SingleOrDefault(l => l.Name == title);
        }

        /// <summary>
        /// Get by rel
        /// </summary>
        /// <param name="links">The links</param>
        /// <param name="rel">The rel</param>
        /// <returns>The link</returns>
        public static Link GetByRel(this IEnumerable<Link> links, string rel)
        {
            return links.SingleOrDefault(l => l.Rel == rel);
        }
    }
}
