//---------------------------------------------------------------------
// <copyright file="ODataExpectedExceptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Utilities;
    #endregion Namespaces

    /// <summary>
    /// Factory class for creating ExpectedException instances.
    /// </summary>
    public static class ODataExpectedExceptions
    {
        private readonly static Dictionary<string, IStringResourceVerifier> resourceVerifierCache = new Dictionary<string, IStringResourceVerifier>();

        /// <summary>
        /// Creates an ExpectedException for ODataException typed exceptions.
        /// </summary>
        /// <param name="resourceKey">The resource key to look up the expected message.</param>
        /// <param name="messageArguments">Arguments for the expected exception message.</param>
        /// <returns>An ExpectedException for this expected ODataException.</returns>
        public static ExpectedException ODataException(string resourceKey, params string[] messageArguments)
        {
            return new ExpectedException(typeof(ODataException), GetResourceVerifier(DataFxAssemblyRef.OData).CreateExpectedError(resourceKey, messageArguments));
        }

        /// <summary>
        /// Creates an ExpectedException for ODataException typed exceptions where the message does not 
        /// need to be an exact match.
        /// </summary>
        /// <param name="resourceKey">The resource key to look up the expected message.</param>
        /// <param name="messageArguments">Arguments for the expected exception message.</param>
        /// <returns>An ExpectedException for this expected ODataException.</returns>
        public static ExpectedException ODataExceptionContains(string resourceKey, params string[] messageArguments)
        {
            return new ExpectedException(
                typeof(ODataException), 
                GetResourceVerifier(DataFxAssemblyRef.OData).CreateExpectedError(resourceKey, messageArguments),
                exactMessageMatch: false);
        }

        /// <summary>
        /// Creates an ExpectedException for ODataContentTypeException typed exceptions.
        /// </summary>
        /// <param name="resourceKey">The resource key to look up the expected message.</param>
        /// <param name="messageArguments">Arguments for the expected exception message.</param>
        /// <returns>An ExpectedException for this expected ODataException.</returns>
        public static ExpectedException ODataContentTypeException(string resourceKey, params string[] messageArguments)
        {
            return new ExpectedException(typeof(ODataContentTypeException), GetResourceVerifier(DataFxAssemblyRef.OData).CreateExpectedError(resourceKey, messageArguments));
        }

        /// <summary>
        /// Creates an ExpectedException for ODataErrorException typed exceptions.
        /// </summary>
        /// <param name="expectedODataError">The expected value of the exception's Error property.</param>
        /// <returns>An ExpectedException for this expected ODataException.</returns>
        public static ExpectedException ODataErrorException(ODataError expectedODataError)
        {
            var expectedException = new ExpectedException(typeof(ODataErrorException));
            expectedException.CustomVerification =
                (e) =>
                {
                    if (!ODataObjectModelValidationUtils.AreEqual(expectedODataError, ((ODataErrorException)e).Error))
                    {
                        throw new DataComparisonException("Expected ODataError instances to be equal.");
                    }
                };

            return expectedException;
        }

        /// <summary>
        /// Creates an ExpectedException for ODataErrorException typed exceptions.
        /// </summary>
        /// <param name="expectedODataError">The expected value of the exception's Error property.</param>
        /// <param name="resourceKey">The resource key to look up the expected message.</param>
        /// <param name="messageArguments">Arguments for the expected exception message.</param>
        /// <returns>An ExpectedException for this expected ODataException.</returns>
        public static ExpectedException ODataErrorException(ODataError expectedODataError, string resourceKey, params string[] messageArguments)
        {
            var expectedException = new ExpectedException(typeof(ODataErrorException), GetResourceVerifier(DataFxAssemblyRef.OData).CreateExpectedError(resourceKey, messageArguments));
            expectedException.CustomVerification = 
                (e) =>
                {
                    if (!ODataObjectModelValidationUtils.AreEqual(expectedODataError, ((ODataErrorException)e).Error))
                    {
                        throw new DataComparisonException("Expected ODataError instances to be equal.");
                    }
                };

            return expectedException;
        }

        /// <summary>
        /// Creates an ExpectedException for ArgumentException typed exceptions.
        /// </summary>
        /// <param name="resourceKey">The resource key to look up the expected message.</param>
        /// <param name="messageArguments">Arguments for the expected exception message.</param>
        /// <returns>An ExpectedException for this expected ArgumentException.</returns>
        /// <remarks>The resource string is a look-up for messages used to construct the ArgumentException. The standard message appended by ArgumentException will not be verified.</remarks>
        public static ExpectedException ArgumentException(string resourceKey, params string[] messageArguments)
        {
            return new ExpectedException(typeof(ArgumentException), GetResourceVerifier(DataFxAssemblyRef.OData).CreateExpectedError(resourceKey, messageArguments), false);
        }

        /// <summary>
        /// Creates an ExpectedException for ArgumentOutOfRangeException typed exceptions.
        /// </summary>
        /// <param name="resourceKey">The resource key to look up the expected message.</param>
        /// <param name="messageArguments">Arguments for the expected exception message.</param>
        /// <returns>An ExpectedException for this expected ArgumentOutOfRangeException.</returns>
        /// <remarks>The resource string is a look-up for messages used to construct the ArgumentOutOfRangeException. The standard message appended by ArgumentOutOfRangeException will not be verified.</remarks>
        public static ExpectedException ArgumentOutOfRangeException(string resourceKey, params string[] messageArguments)
        {
            return new ExpectedException(typeof(ArgumentOutOfRangeException), GetResourceVerifier(DataFxAssemblyRef.OData).CreateExpectedError(resourceKey, messageArguments), false);
        }

        /// <summary>
        /// Creates an ExpectedException for ArgumentNullException typed exceptions.
        /// </summary>
        /// <returns>An ExpectedException for this expected ArgumentNullException.</returns>
        public static ExpectedException ArgumentNullOrEmptyException()
        {
            return new ExpectedException(typeof(ArgumentNullException), GetResourceVerifier(DataFxAssemblyRef.OData).CreateExpectedError("ExceptionUtils_ArgumentStringNullOrEmpty"), false);
        }

        /// <summary>
        /// Creates an ExpectedException for ArgumentNullException typed exceptions.
        /// </summary>
        /// <returns>An ExpectedException for this expected ArgumentNullException.</returns>
        public static ExpectedException ArgumentNullException()
        {
            return new ExpectedException(typeof(ArgumentNullException));
        }

        /// <summary>
        /// Creates an ExpectedException for ArgumentNullException typed exceptions.
        /// </summary>
        /// <param name="resourceKey">The resource key to look up the expected message.</param>
        /// <param name="messageArguments">Arguments for the expected exception message.</param>
        /// <returns>An ExpectedException for this expected ArgumentNullException.</returns>
        /// <remarks>The resource string is a look-up for messages used to construct the ArgumentNullException. The standard message appended by ArgumentNullException will not be verified.</remarks>
        public static ExpectedException ArgumentNullException(string resourceKey, params string[] messageArguments)
        {
            return new ExpectedException(typeof(ArgumentNullException), GetResourceVerifier(DataFxAssemblyRef.OData).CreateExpectedError(resourceKey, messageArguments), false);
        }

        /// <summary>
        /// Creates an ExpectedException for ODataTestException typed exceptions.
        /// </summary>
        /// <returns>An ExpectedException for this expected ODataTestException.</returns>
        /// <remarks>Exception message is not verified for test exceptions.</remarks>
        public static ExpectedException TestException()
        {
            return new ExpectedException(typeof(ODataTestException));
        }

        /// <summary>
        /// Retrieves the resource verifier for the specified assembly.
        /// </summary>
        /// <param name="assemblyFullName">The full name of the assembly to retrieve.</param>
        /// <returns>The resource verifier for the assembly.</returns>
        private static IStringResourceVerifier GetResourceVerifier(string assemblyFullName)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(assemblyFullName, "assemblyFullName");

            IStringResourceVerifier verifier = null;
           
            // Resource lookup not supported on Silverlight or Phone platforms.
#if !SILVERLIGHT && !WINDOWS_PHONE
            if (!resourceVerifierCache.TryGetValue(assemblyFullName, out verifier))
            {
                Assembly assembly = Assembly.Load(new AssemblyName(assemblyFullName));
                var lookup = new ODataAssemblyResourceLookup(assembly);
                verifier = new StringResourceVerifier(lookup);
                resourceVerifierCache.Add(assemblyFullName, verifier);
            }
#endif

            return verifier;
        }
    }
}
