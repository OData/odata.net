//---------------------------------------------------------------------
// <copyright file="ExceptionVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Contracts;
    #endregion Namespaces

    /// <summary>
    /// This class verifies actual exceptions against expectations, including matching resource strings.
    /// </summary>
    [ImplementationName(typeof(IExceptionVerifier), "Default")]
    public class ExceptionVerifier : IExceptionVerifier
    {
        /// <summary>
        /// Gets or sets the assertion handler.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public AssertionHandler AssertionHandler { get; set; }

        /// <summary>
        /// Verifies that the actual exception matches expectations including type, message (from resources) and parameters.
        /// </summary>
        /// <param name="expected">The expected exception details.</param>
        /// <param name="actual">The actual exception to verify.</param>
        public void VerifyExceptionResult(ExpectedException expected, Exception actual)
        {
            ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
            ExceptionUtilities.CheckArgumentNotNull(actual, "actual");

            this.AssertionHandler.AreEqual(expected.ExpectedExceptionType, actual.GetType(), "Unexpected exception type caught");

            // Resource lookup not supported on Silverlight or Phone platforms.
            if (expected.ExpectedMessage != null)
            {
                expected.ExpectedMessage.VerifyMatch(null, actual.Message, expected.ExactMatch);
            }

            if (expected.CustomVerification != null)
            {
                expected.CustomVerification(actual);
            }
        }
    }
}