//---------------------------------------------------------------------
// <copyright file="AssertExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Tests
{
    /// <summary>
    /// Extension methods for assert.
    /// </summary>
    internal static class AssertExtensions
    {
        /// <summary>
        /// Verifies that the test code does not throw.
        /// </summary>
        /// <param name="testCode">A delegate to the code to be tested</param>
        public static void DoesNotThrow(this Action testCode)
        {
            Exception ex = Record.Exception(testCode);
            Assert.Null(ex);
        }

        /// <summary>
        /// Verifies that the exact exception calling the test code action is thrown.
        /// </summary>
        /// <typeparam name="T">The type of the exception expected to be thrown</typeparam>
        /// <param name="testCode">A delegate to the code to be tested</param>
        /// <param name="errorMessage">The expected error message.</param>
        public static void Throws<T>(this Action testCode, string errorMessage) where T : Exception
        {
            T exception = Assert.Throws<T>(testCode);
            Assert.Equal(errorMessage, exception.Message);
        }

        /// <summary>
        /// Verifies that the exact exception calling the async test code is thrown.
        /// </summary>
        /// <typeparam name="T">The type of the exception expected to be thrown</typeparam>
        /// <param name="testCode">An async delegate to the code to be tested</param>
        /// <param name="errorMessage">The expected error message.</param>
        public static async Task ThrowsAsync<T>(this Func<Task> testCode, string errorMessage) where T : Exception
        {
            T exception = await Assert.ThrowsAsync<T>(testCode);
            Assert.Equal(errorMessage, exception.Message);
        }

        /// <summary>
        /// Verifies that the exact exception or a derived exception type is thrown with the error message.
        /// </summary>
        /// <typeparam name="T">The type of the exception or a derived exception type expected to be thrown</typeparam>
        /// <param name="testCode">A delegate to the code to be tested</param>
        /// <param name="errorMessage">The expected error message.</param>
        public static void ThrowsAny<T>(this Action testCode, string errorMessage) where T : Exception
        {
            T exception = Assert.ThrowsAny<T>(testCode);
            Assert.Equal(errorMessage, exception.Message);
        }

        /// <summary>
        /// Verifies that the exact enumerable has the same order and items.
        /// </summary>
        /// <typeparam name="T">The type of the element.</typeparam>
        /// <param name="enumerable">The test enumerable.</param>
        /// <param name="expectedElements">The expected elements.</param>
        public static void ContainExactly<T>(this IEnumerable<T> enumerable, T[] expectedElements)
        {
            Assert.Equal(enumerable.Count(), expectedElements.Length);

            int index = 0;
            foreach (var item in enumerable)
            {
                Assert.Equal(expectedElements[index++], item);
            }
        }
    }
}
