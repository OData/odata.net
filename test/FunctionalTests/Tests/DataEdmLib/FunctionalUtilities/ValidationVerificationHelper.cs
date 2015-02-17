//---------------------------------------------------------------------
// <copyright file="ValidationVerificationHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.OData.Framework.Common;
    using Microsoft.Test.OData.Framework.Verification;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Helper to verify EdmErrors
    /// </summary>
    public static class ValidationVerificationHelper
    {
        private static StringResourceVerifier stringResourceVerifier = new StringResourceVerifier(new AssemblyResourceLookup(typeof(IEdmModel).Assembly));
        
        public static void Verify(IEnumerable<ExpectedEdmError> expectedErrors, IEnumerable<EdmError> actualErrors)
        {
            Assert.AreEqual(expectedErrors.Count(), actualErrors.Count(), "Error count does not match!");

            IList<ExpectedEdmError> expectedErrorsList = expectedErrors.ToList();
            foreach (var a in actualErrors)
            {
                ExpectedEdmError matchingExpected = expectedErrorsList.FirstOrDefault(e => IsMatch(e, a));

                Assert.IsNotNull(matchingExpected, "There is no match for the actual error: {0}", a);
                expectedErrorsList.Remove(matchingExpected);
            }
        }

        public static bool IsMatch(ExpectedEdmError expectedError, EdmError actualError)
        {
            if (expectedError.ErrorCode != actualError.ErrorCode)
            {
                return false;
            }

            return stringResourceVerifier.IsMatch(expectedError.MessageResourceKey, actualError.ErrorMessage, false, expectedError.MessageArguments.ToArray());
        }
    }
}
