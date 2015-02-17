//---------------------------------------------------------------------
// <copyright file="ExpectedExceptionTypeMessageVerifier`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Reflection;
    using System.Resources;
    using System.Text.RegularExpressions;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Represents an expected exception type and message.
    /// </summary>
    /// <typeparam name="TException">The type of the expected exception.</typeparam>
    public class ExpectedExceptionTypeMessageVerifier<TException> : ExpectedExceptionVerifier where TException : Exception
    {
        private static Regex stringFormatPattern = new Regex(@"{\d+}");

        /// <summary>
        /// Initializes a new instance of the ExpectedExceptionTypeMessageVerifier class.
        /// </summary>
        /// <param name="innerExpectedException">The expected inner exception.</param>
        /// <param name="messagePattern">A string holding a regex pattern to match against exception messages.</param>
        public ExpectedExceptionTypeMessageVerifier(ExpectedExceptionVerifier innerExpectedException, string messagePattern)
            : base(innerExpectedException)
        {
            if (messagePattern == null)
            {
                messagePattern = string.Empty;
            }

            this.MessageRegex = new Regex(stringFormatPattern.Replace(messagePattern, ".*"));            
        }

        /// <summary>
        /// Gets the Regex pattern of the expected exception message.
        /// </summary>
        public Regex MessageRegex { get; private set; }

        /// <summary>
        /// Verifies that the exception type inherits from the expected type, and the message matches the given Regex.
        /// </summary>
        /// <param name="actualException">The exception to verify.</param>
        /// <returns>Whether the exception matches what is expected.</returns>
        public override bool VerifyException(Exception actualException)
        {
            if (actualException == null)
            {
                return false;
            }

            if (!typeof(TException).IsAssignableFrom(actualException.GetType()))
            {
                return false;
            }

            return this.MessageRegex.IsMatch(actualException.Message);
        }
    }
}
