//---------------------------------------------------------------------
// <copyright file="EdmError.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.OData.Edm.Validation
{
    /// <summary>
    /// Represents a reportable error in EDM
    /// </summary>
    public class EdmError
    {
        private Dictionary<string, object> _additionalInfo;

        /// <summary>
        /// Initializes a new instance of the EdmError class.
        /// </summary>
        /// <param name="errorLocation">The location where the error occurred.</param>
        /// <param name="errorCode">An integer code representing the error.</param>
        /// <param name="errorMessage">A human readable message describing the error.</param>
        public EdmError(EdmLocation errorLocation, EdmErrorCode errorCode, string errorMessage)
         : this(errorLocation, errorCode, errorMessage, Severity.Undefined)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EdmError class.
        /// </summary>
        /// <param name="errorLocation">The location where the error occurred.</param>
        /// <param name="errorCode">An integer code representing the error.</param>
        /// <param name="errorMessage">A human readable message describing the error.</param>
        /// <param name="severity">An enum value describing the error severity.</param>
        public EdmError(EdmLocation errorLocation, EdmErrorCode errorCode, string errorMessage, Severity severity)
        {
            this.ErrorLocation = errorLocation;
            this.ErrorCode = errorCode;
            this.ErrorMessage = errorMessage;
            this.Severity = severity;
        }

        /// <summary>
        /// Gets the location of the error in the file in which it occurred.
        /// </summary>
        public EdmLocation ErrorLocation { get; private set; }

        /// <summary>
        /// Gets a string describing the error severity.
        /// </summary>
        public Severity Severity { get; private set; }

        /// <summary>
        /// Gets an integer code representing the error.
        /// </summary>
        public EdmErrorCode ErrorCode { get; private set; }

        /// <summary>
        /// Gets a human readable string describing the error.
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Dictionary of extended Error properties.
        /// </summary>
        public Dictionary<string, object> ExtendedProperties
        {
            get
            {
                if (_additionalInfo == null)
                {
                    _additionalInfo = new Dictionary<string, object>();
                }

                return _additionalInfo;
            }
        }

    /// <summary>
    /// Gets a string representation of the error.
    /// </summary>
    /// <returns>A string representation of the error.</returns>
    public override string ToString()
        {
            string baseString = Convert.ToString(this.ErrorCode, CultureInfo.InvariantCulture) + " : " + this.ErrorMessage;
            string severity = Convert.ToString(this.Severity, CultureInfo.InvariantCulture);

            if (this.Severity != Severity.Undefined)
            {
                if (this.ErrorLocation != null && !(this.ErrorLocation is ObjectLocation))
                {
                    return baseString + " : " + this.ErrorLocation.ToString() + " : " + severity;
                }

                return baseString + " : " + severity;
            }

            if (this.ErrorLocation != null && !(this.ErrorLocation is ObjectLocation))
            {
                return baseString + " : " + this.ErrorLocation.ToString();
            }

            return baseString;
        }
    }
}