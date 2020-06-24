//---------------------------------------------------------------------
// <copyright file="ODataUrlValidationMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.UriParser.Validation
{
    /// <summary>
    /// Severity of an OData Url Validation Message
    /// </summary>
    public enum Severity
    {
        /// <summary>
        /// Default / Undefined error Level
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Informational Message
        /// </summary>
        Info = 1,

        /// <summary>
        /// Warning
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Error
        /// </summary>
        Error = 3,
    }

    /// <summary>
    /// A class representing a message encountered while validating an OData Url.
    /// </summary>
    public class ODataUrlValidationMessage
    {
        private Dictionary<string, object> additionalInfo;

        /// <summary>
        /// Construct an instance of an ODataUrlValidationMessage given a message code, message, and severity.
        /// </summary>
        /// <param name="code">The message code of the error.</param>
        /// <param name="message">The message.</param>
        /// <param name="severity">The severity of the message.</param>
        public ODataUrlValidationMessage(string code, string message, Severity severity)
        {
            this.MessageCode = code;
            this.Message = message;
            this.Severity = severity;
        }

        /// <summary>
        /// The message code.
        /// </summary>
        public string MessageCode { get; }

        /// <summary>
        /// The message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The severity of the error.
        /// </summary>
        public Severity Severity { get; }

        /// <summary>
        /// Dictionary of extended message properties.
        /// </summary>
        public Dictionary<string, object> ExtendedProperties
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
