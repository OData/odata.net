//---------------------------------------------------------------------
// <copyright file="FluentAssertionsExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests
{
    using FluentAssertions;
    using FluentAssertions.Collections;

    /// <summary>
    /// Contains generic fluent assertion extensions.
    /// </summary>
    internal static class FluentAssertionsExtensions
    {
        /// <summary>
        /// Combines a ContainInOrder and Count().Should().Be(#) statement.
        /// </summary>
        public static GenericCollectionAssertions<T> ContainExactly<T>(this GenericCollectionAssertions<T> list, T[] expectedElements)
        {
            list.ContainInOrder(expectedElements);
            list.HaveCount(expectedElements.Length);
            return list;
        }
    }
}