//---------------------------------------------------------------------
// <copyright file="ExpectedException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Represents an expected exception test result
    /// </summary>
    public class ExpectedException
    {
        private readonly static Type[] ExceptionTypesThatMustSpecifyResources = new[]
        {
            typeof(ODataException),
            typeof(ODataContentTypeException),
        };

        /// <summary>
        /// Initializes a new instance of the ExpectedException class. 
        /// </summary>
        /// <param name="exceptionType">The Type of exception expected.</param>
        /// <remarks>This constructor is only expected to be used for verifying system or test exceptions without validating the exception message.</remarks>
        public ExpectedException(Type exceptionType)
        {
            ExceptionUtilities.Assert(
                !ExceptionTypesThatMustSpecifyResources.Contains(exceptionType),
                "This constructor can not be used for exceptions of type '" + exceptionType.Name + "'");

            this.ExpectedExceptionType = exceptionType;
        }

        /// <summary>
        /// Initializes a new instance of the ExpectedException class. 
        /// </summary>
        /// <param name="exceptionType">The Type of exception expected.</param>
        /// <param name="errorMessage">The expected error message</param>
        /// <param name="exactMessageMatch">Whether or not to match the resource string exactly. Default is true.</param>
        public ExpectedException(Type exceptionType, ExpectedErrorMessage errorMessage, bool exactMessageMatch = true)
        {
            ExceptionUtilities.CheckArgumentNotNull(exceptionType, "exceptionType");
            ExceptionUtilities.CheckArgumentNotNull(errorMessage, "errorMessage");
            ExceptionUtilities.Assert(typeof(Exception).IsAssignableFrom(exceptionType), "The type passed as exceptionType must be an exception type");

            this.ExpectedExceptionType = exceptionType;
            this.ExpectedMessage = errorMessage;
            this.ExactMatch = exactMessageMatch;
        }

        /// <summary>
        /// Gets or sets the expected exception message.
        /// </summary>
        public ExpectedErrorMessage ExpectedMessage { get; private set; }

        /// <summary>
        /// Gets or sets the type of expected exception.
        /// </summary>
        public Type ExpectedExceptionType { get; private set; }

        /// <summary>
        /// Gets or sets the value indicating whether or not to verify an exact match of the exception message, or just
        /// that the message is contained within the actual exception message.
        /// </summary>
        public bool ExactMatch { get; private set; }

        /// <summary>
        /// Gets or sets a delegate providing futher verification of the actual exception.
        /// </summary>
        public Action<Exception> CustomVerification { get; set; }
    }
}
