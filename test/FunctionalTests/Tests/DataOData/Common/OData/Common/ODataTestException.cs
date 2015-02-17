//---------------------------------------------------------------------
// <copyright file="ODataTestException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    #endregion Namespaces

    /// <summary>
    /// Exception class used by ODataLib tests.
    /// </summary>
    public class ODataTestException : Exception
    {
        /// <summary>
        /// Creates a ODataLib test exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ODataTestException(string message)
            : base(message)
        {
        }
    }
}
