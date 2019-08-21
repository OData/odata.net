//---------------------------------------------------------------------
// <copyright file="ODataInnerError.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using Microsoft.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Class representing implementation specific debugging information to help determine the cause of the error.
    /// </summary>
    [DebuggerDisplay("{Message}")]
#if ORCAS
    [Serializable]
#endif
    public sealed class ODataInnerError
    {
        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.ODataInnerError" /> class with default values.</summary>
        public ODataInnerError()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.ODataInnerError" /> class with exception object.</summary>
        /// <param name="exception">The <see cref="T:System.Exception" /> used to create the inner error.</param>
        public ODataInnerError(Exception exception)
            : this(exception, null)
        {

        }

        public ODataInnerError(Exception exception, string nestedObjectName)
        {
            ExceptionUtils.CheckArgumentNotNull(exception, "exception");

            if (exception.InnerException != null)
            {
                this.InnerError = new ODataInnerError(exception.InnerException, nestedObjectName);
            }

            Properties = new Dictionary<string, ODataValue>();
            Properties.Add(JsonConstants.ODataErrorInnerErrorMessageName, exception.Message.ToODataValue() ?? string.Empty.ToODataValue());
            Properties.Add(JsonConstants.ODataErrorInnerErrorTypeNameName, exception.GetType().FullName.ToODataValue() ?? string.Empty.ToODataValue());
            Properties.Add(JsonConstants.ODataErrorInnerErrorStackTraceName, exception.StackTrace.ToODataValue() ?? string.Empty.ToODataValue());
        }

        public ODataInnerError(IDictionary<string, ODataValue> properties, string nestedObjectName, ODataInnerError nestedObject)
        {
            Properties = properties;
            this.NestedObjectName = nestedObjectName;
            InnerError = nestedObject;
        }

        /// <summary>
        /// Nested object's name
        /// </summary>
        internal string NestedObjectName;

        /// <summary>
        /// Properties to be written for the inner error.
        /// </summary>
        public IDictionary<string, ODataValue> Properties
        {
            get;
            set;
        }

        /// <summary>Gets or sets the error message.</summary>
        /// <returns>The error message.</returns>
        public string Message
        {
            get { return LookUpStringValue(JsonConstants.ODataErrorInnerErrorMessageName); }
            set { SetStringValue(JsonConstants.ODataErrorInnerErrorMessageName, value); }
        }

        /// <summary>Gets or sets the type name of this error, for example, the type name of an exception.</summary>
        /// <returns>The type name of this error.</returns>
        public string TypeName
        {
            get { return LookUpStringValue(JsonConstants.ODataErrorInnerErrorTypeNameName); }
            set { SetStringValue(JsonConstants.ODataErrorInnerErrorTypeNameName, value); }
        }

        /// <summary>Gets or sets the stack trace for this error.</summary>
        /// <returns>The stack trace for this error.</returns>
        public string StackTrace
        {
            get { return LookUpStringValue(JsonConstants.ODataErrorInnerErrorStackTraceName); }
            set { SetStringValue(JsonConstants.ODataErrorInnerErrorStackTraceName, value); }
        }

        /// <summary>Gets or sets the nested implementation specific debugging information. </summary>
        /// <returns>The nested implementation specific debugging information.</returns>
        public ODataInnerError InnerError
        {
            get;
            set;
        }

        /// <summary>
        /// Serialization to Json format string.
        /// </summary>
        /// <returns>The string in Json format</returns>
        internal string ToJson()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{{" +
                "\"message\":\"{0}\"," +
                "\"type\":\"{1}\"," +
                "\"stacktrace\":\"{2}\"," +
                "\"innererror\":{3}" +
                "}}",
                this.Message == null ? "" : JsonValueUtils.GetEscapedJsonString(this.Message),
                this.TypeName == null ? "" : JsonValueUtils.GetEscapedJsonString(this.TypeName),
                this.StackTrace == null ? "" : JsonValueUtils.GetEscapedJsonString(this.StackTrace),
                this.InnerError == null ? "{}" : this.InnerError.ToJson());
        }

        private string LookUpStringValue(string v)
        {
            if (Properties == null)
            {
                return string.Empty;
            }

            foreach (KeyValuePair<string, ODataValue> pair in Properties)
            {
                if (pair.Key == v)
                {
                    string value = (string)pair.Value.FromODataValue();
                    return value;
                }
            }

            return string.Empty;
        }

        private void SetStringValue(string v, string value)
        {
            if (Properties == null)
            {
                Properties = new Dictionary<string, ODataValue>();
            }

            Properties[v] = value.ToODataValue();
        }
    }
}
