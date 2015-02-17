//---------------------------------------------------------------------
// <copyright file="TestMatrixCombinationCallbackAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Attribute for designating a callback for a particular combination generated in a test matrix
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public sealed class TestMatrixCombinationCallbackAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the TestMatrixCombinationCallbackAttribute class.
        /// </summary>
        /// <param name="callbackMethodName">The name of the method to call for each combination</param>
        public TestMatrixCombinationCallbackAttribute(string callbackMethodName)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(callbackMethodName, "callbackMethodName");
            this.CallbackMethodName = callbackMethodName;
        }

        /// <summary>
        /// Gets the name of the method to call for each combination
        /// </summary>
        public string CallbackMethodName { get; private set; }
    }
}
