//---------------------------------------------------------------------
// <copyright file="VerificationUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts;
    #endregion Namespaces

    /// <summary>Helper methods for verifying results against expected data.</summary>
    public static class VerificationUtils
    {
        public static void VerifyEnumerationsAreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, AssertionHandler assert)
        {
            VerifyEnumerationsAreEqual<T>(
                expected,
                actual,
                (first, second, assertionHandler) => assertionHandler.AreEqual(first, second, "Items don't match."),
                (item) => item.ToString(),
                assert);
        }

        public static void VerifyEnumerationsAreEqual<T>(
            IEnumerable<T> expectedEnumeration,
            IEnumerable<T> actualEnumeration,
            Action<T, T, AssertionHandler> verifyItem,
            Func<T, string> itemToDebugString,
            AssertionHandler assert)
        {
            if (expectedEnumeration == null)
            {
                assert.IsNull(actualEnumeration, "The enumeration of items should have been null.");
                return;
            }
            else
            {
                assert.IsNotNull(actualEnumeration, "The enumeration of items should not have been null.");
            }

            try
            {
                var expectedEnumerator = expectedEnumeration.GetEnumerator();
                var actualEnumerator = actualEnumeration.GetEnumerator();
                while (expectedEnumerator.MoveNext())
                {
                    assert.IsTrue(
                        actualEnumerator.MoveNext(), 
                        "The actual enumeration has less items than the expected enumeration.\r\n" +
                        "Expected items: " + string.Join(", ", expectedEnumeration.Select(t => "<" + itemToDebugString(t) + ">")) + "\r\n" +
                        "Actual items: " + string.Join(", ", actualEnumeration.Select(t => "<" + itemToDebugString(t) + ">")));
                    verifyItem(expectedEnumerator.Current, actualEnumerator.Current, assert);
                }

                assert.IsFalse(
                    actualEnumerator.MoveNext(), 
                    "The expected enumeration has less items than the actual enumeration.\r\n" +
                    "Expected items: " + string.Join(", ", expectedEnumeration.Select(t => "<" + itemToDebugString(t) + ">")) + "\r\n" +
                    "Actual items: " + string.Join(", ", actualEnumeration.Select(t => "<" + itemToDebugString(t) + ">")));
            }
            catch (Exception)
            {
                assert.Warn("Expected items: " + string.Join(", ", expectedEnumeration.Select(t => "<" + itemToDebugString(t) + ">")));
                assert.Warn("Actual items: " + string.Join(", ", actualEnumeration.Select(t => "<" + itemToDebugString(t) + ">")));
                throw;
            }
        }
    }
}
