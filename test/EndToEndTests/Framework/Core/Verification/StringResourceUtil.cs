//---------------------------------------------------------------------
// <copyright file="StringResourceUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Verification
{
    using System;
    using System.Reflection;
    using Microsoft.Test.OData.Framework.Common;

    /// <summary>
    /// Static helper methods for interacting with string resources.
    /// </summary>
    public static class StringResourceUtil
    {
        private static readonly Lazy<StringResourceVerifier> dataServicesStringResourceVerifier =  
            new Lazy<StringResourceVerifier>(() => new StringResourceVerifier(new AssemblyResourceLookup(Assembly.Load(new AssemblyName("Microsoft.OData.Service").FullName))));

        private static readonly Lazy<StringResourceVerifier> dataServicesClientStringResourceVerifier =
            new Lazy<StringResourceVerifier>(() => new StringResourceVerifier(new AssemblyResourceLookup(Assembly.Load(new AssemblyName("Microsoft.OData.Client").FullName))));

        private static readonly Lazy<StringResourceVerifier> odataLibStringResourceVerifier =
            new Lazy<StringResourceVerifier>(() => new StringResourceVerifier(new AssemblyResourceLookup(Assembly.Load(new AssemblyName("Microsoft.OData.Core").FullName))));

        /// <summary>
        /// Verifies an error message against a string resource from the Microsoft.OData.Service assembly.
        /// </summary>
        /// <param name="errorMessage">The error message to verify.</param>
        /// <param name="resourceIdentifier">The identifier of the string resource to verify against.</param>
        /// <param name="arguments">Argument values for the error message.</param>
        public static void VerifyDataServicesString(string errorMessage, string resourceIdentifier, params object[] arguments)
        {
            VerifyString(dataServicesStringResourceVerifier.Value, errorMessage, resourceIdentifier, arguments); 
        }

        /// <summary>
        /// Verifies an error message against a string resource from the Microsoft.OData.Client assembly.
        /// </summary>
        /// <param name="errorMessage">The error message to verify.</param>
        /// <param name="resourceIdentifier">The identifier of the string resource to verify against.</param>
        /// <param name="arguments">Argument values for the error message.</param>
        public static void VerifyDataServicesClientString(string errorMessage, string resourceIdentifier, params object[] arguments)
        {
            VerifyString(dataServicesClientStringResourceVerifier.Value, errorMessage, resourceIdentifier, arguments);
        }

        /// <summary>
        /// Verifies an error message against a string resource from the Microsoft.OData.Core assembly.
        /// </summary>
        /// <param name="errorMessage">The error message to verify.</param>
        /// <param name="resourceIdentifier">The identifier of the string resource to verify against.</param>
        /// <param name="isExactMatch">Determines whether the exception message must be exact match of the message in the resource file, or just contain it.</param>
        /// <param name="arguments">Argument values for the error message.</param>
        public static void VerifyODataLibString(string errorMessage, string resourceIdentifier, bool isExactMatch, params object[] arguments)
        {
            odataLibStringResourceVerifier.Value.VerifyMatch(resourceIdentifier, errorMessage, isExactMatch, arguments);
        }

        private static void VerifyString(StringResourceVerifier resourceVerifier, string errorMessage, string resourceIdentifier, params object[] messageArguments)
        {
            resourceVerifier.VerifyMatch(resourceIdentifier, errorMessage, messageArguments);
        }
    }
}
