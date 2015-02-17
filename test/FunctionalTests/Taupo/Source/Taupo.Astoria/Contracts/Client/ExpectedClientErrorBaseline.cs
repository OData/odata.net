//---------------------------------------------------------------------
// <copyright file="ExpectedClientErrorBaseline.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Contract for the expected Exceptions that occur in the client
    /// </summary>
    public class ExpectedClientErrorBaseline
    {
        /// <summary>
        /// Initializes a new instance of the ExpectedClientErrorBaseline class.
        /// </summary>
        /// <param name="expectedExceptionType">The expected exception type</param>
        /// <param name="hasServiceSpecificMessage">A value inidcating whether the error message is from the server</param>
        /// <param name="expectedMessage">The expected error message</param>
        public ExpectedClientErrorBaseline(Type expectedExceptionType, bool hasServiceSpecificMessage, ExpectedErrorMessage expectedMessage)
        {
            ExceptionUtilities.CheckArgumentNotNull(expectedExceptionType, "expectedExceptionType");
            ExceptionUtilities.CheckArgumentNotNull(expectedMessage, "expectedMessage");

            this.ExpectedExceptionType = expectedExceptionType;
            this.HasServerSpecificExpectedMessage = hasServiceSpecificMessage;
            this.ExpectedExceptionMessage = expectedMessage;
        }

        /// <summary>
        /// Gets an Expected Exception type to ensure it matches against
        /// </summary>
        public Type ExpectedExceptionType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the ExpectedExceptionMessage is a server error or a client error
        /// If its a server error then this indicates the server error will be wrapped in an OData Error payload 
        /// </summary>
        public bool HasServerSpecificExpectedMessage { get; private set; }

        /// <summary>
        /// Gets the Expected Exception Message for the Client
        /// </summary>
        public ExpectedErrorMessage ExpectedExceptionMessage { get; private set; }
    }
}
