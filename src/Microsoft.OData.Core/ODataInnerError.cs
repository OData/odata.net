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
    using System.Text;
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
        /// <summary>Initializes a new instance of the <see cref="Microsoft.OData.ODataInnerError" /> class with default values.</summary>
        public ODataInnerError()
        {
            Properties = new Dictionary<string, ODataValue>();

            //// NOTE: we add empty elements if no information is provided for the message, error type and stack trace
            ////       to stay compatible with Astoria.
            Properties.Add(JsonConstants.ODataErrorInnerErrorMessageName, new ODataNullValue());
            Properties.Add(JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataNullValue());
            Properties.Add(JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataNullValue());
        }

        /// <summary>Initializes a new instance of the <see cref="Microsoft.OData.ODataInnerError" /> class with exception object.</summary>
        /// <param name="exception">The <see cref="System.Exception" /> used to create the inner error.</param>
        public ODataInnerError(Exception exception)
        {
            ExceptionUtils.CheckArgumentNotNull(exception, "exception");

            if (exception.InnerException != null)
            {
                this.InnerError = new ODataInnerError(exception.InnerException);
            }

            Properties = new Dictionary<string, ODataValue>();

            //// NOTE: we add empty elements if no information is provided for the message, error type and stack trace
            ////       to stay compatible with Astoria.
            Properties.Add(JsonConstants.ODataErrorInnerErrorMessageName, exception.Message.ToODataValue() ?? new ODataNullValue());
            Properties.Add(JsonConstants.ODataErrorInnerErrorTypeNameName, exception.GetType().FullName.ToODataValue() ?? new ODataNullValue());
            Properties.Add(JsonConstants.ODataErrorInnerErrorStackTraceName, exception.StackTrace.ToODataValue() ?? new ODataNullValue());
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="Microsoft.OData.ODataInnerError" /> class with a dictionary of property names and corresponding ODataValues.
        /// </summary>
        /// <param name="properties">Dictionary of string keys with ODataValue as value. Key string indicates the property name where as the value of the property is encapsulated in ODataValue.</param>
        public ODataInnerError(IDictionary<string, ODataValue> properties)
        {
            ExceptionUtils.CheckArgumentNotNull(properties, "properties");

            Properties = new Dictionary<string, ODataValue>(properties);
        }

        /// <summary>
        /// Properties to be written for the inner error.
        /// </summary>
        public IDictionary<string, ODataValue> Properties
        {
            get;
            private set;
        }

        /// <summary>Gets or sets the error message.</summary>
        /// <returns>The error message.</returns>
        public string Message
        {
            get { return GetStringValue(JsonConstants.ODataErrorInnerErrorMessageName); }
            set { SetStringValue(JsonConstants.ODataErrorInnerErrorMessageName, value); }
        }

        /// <summary>Gets or sets the type name of this error, for example, the type name of an exception.</summary>
        /// <returns>The type name of this error.</returns>
        public string TypeName
        {
            get { return GetStringValue(JsonConstants.ODataErrorInnerErrorTypeNameName); }
            set { SetStringValue(JsonConstants.ODataErrorInnerErrorTypeNameName, value); }
        }

        /// <summary>Gets or sets the stack trace for this error.</summary>
        /// <returns>The stack trace for this error.</returns>
        public string StackTrace
        {
            get { return GetStringValue(JsonConstants.ODataErrorInnerErrorStackTraceName); }
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
            StringBuilder sb = new StringBuilder();

            // Write some properties to enforce an order. 
            foreach (KeyValuePair<string, ODataValue> keyValuePair in Properties)
            {
                if (keyValuePair.Key == JsonConstants.ODataErrorInnerErrorMessageName ||
                    keyValuePair.Key == JsonConstants.ODataErrorInnerErrorStackTraceName ||
                    keyValuePair.Key == JsonConstants.ODataErrorInnerErrorTypeNameName ||
                    keyValuePair.Key == JsonConstants.ODataErrorInnerErrorInnerErrorName)
                {
                    continue;
                }

                sb.Append(",");
                sb.Append("\"").Append(keyValuePair.Key).Append("\"").Append(":");
                ODataJsonWriterUtils.ODataValueToString(sb, keyValuePair.Value);
            }

            return string.Format(CultureInfo.InvariantCulture,
                "{{" +
                "\"message\":\"{0}\"," +
                "\"type\":\"{1}\"," +
                "\"stacktrace\":\"{2}\"," +
                "\"innererror\":{3}{4}" +
                "}}",
                this.Message == null ? "" : JsonValueUtils.GetEscapedJsonString(this.Message),
                this.TypeName == null ? "" : JsonValueUtils.GetEscapedJsonString(this.TypeName),
                this.StackTrace == null ? "" : JsonValueUtils.GetEscapedJsonString(this.StackTrace),
                this.InnerError == null ? "{}" : this.InnerError.ToJson(),
                sb.ToString());
        }

        private string GetStringValue(string propertyKey)
        {
            if (Properties.ContainsKey(propertyKey))
            {
                return Properties[propertyKey].FromODataValue()?.ToString();
            }

            return string.Empty;
        }

        private void SetStringValue(string propertyKey, string value)
        {
            ODataValue odataValue = value == null ? new ODataNullValue() : value.ToODataValue();

            if (!Properties.ContainsKey(propertyKey))
            {
                Properties.Add(propertyKey, odataValue);
            }
            else
            {
                Properties[propertyKey] = odataValue;
            }
        }
    }
}
