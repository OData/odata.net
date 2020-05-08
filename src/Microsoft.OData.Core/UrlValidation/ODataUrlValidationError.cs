//---------------------------------------------------------------------
// <copyright file="ODataUrlValidationError.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.UriParser.Validation
{
    /// <summary>
    /// Severity of an OData Url Validation Error
    /// </summary>
    public enum Severity
    {
        Warning,
        Error
    }

    /// <summary>
    /// A class representing an error encountered while validating an OData Url.
    /// </summary>
    public class ODataUrlValidationError
    {
        private Dictionary<string, object> additionalInfo;

        /// <summary>
        /// The error code.
        /// </summary>
        public string ErrorCode;

        /// <summary>
        /// The error message.
        /// </summary>
        public string ErrorMessage;

        /// <summary>
        /// The severity of the error.
        /// </summary>
        public Severity Severity;

        /// <summary>
        /// Construct an instance of an ODataUrlValidationError given an error code, message, and severity.
        /// </summary>
        /// <param name="code">The error code of the error.</param>
        /// <param name="message">The error message.</param>
        /// <param name="severity">The severity of the error.</param>
        public ODataUrlValidationError(string code, string message, Severity severity)
        {
            this.ErrorCode = code;
            this.ErrorMessage = message;
            this.Severity = severity;
        }

        /// <summary>
        /// Dictionary of Extended error information.
        /// </summary>
        public Dictionary<string, object> AdditionalInfo
        {
            get
            {
                if (additionalInfo == null)
                {
                    additionalInfo = new Dictionary<string, object>();
                }

                return additionalInfo;
            }
        }
    }
}
