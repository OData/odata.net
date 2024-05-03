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
    using System.Text;
    using Microsoft.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Class representing implementation specific debugging information to help determine the cause of the error.
    /// </summary>
    [DebuggerDisplay("{Message}")]
    public sealed class ODataInnerError
    {
        /// <summary>Initializes a new instance of the <see cref="ODataInnerError" /> class with default values.</summary>
        public ODataInnerError()
        {
            Properties = new Dictionary<string, ODataValue>
            {
                //// NOTE: we add empty elements if no information is provided for the message, error type and stack trace
                ////       to stay compatible with Astoria.
                { JsonConstants.ODataErrorInnerErrorMessageName, new ODataNullValue() },
                { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataNullValue() },
                { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataNullValue() }
            };
        }

        /// <summary>Initializes a new instance of the <see cref="ODataInnerError" /> class with exception object.</summary>
        /// <param name="exception">The <see cref="System.Exception" /> used to create the inner error.</param>
        public ODataInnerError(Exception exception)
        {
            ExceptionUtils.CheckArgumentNotNull(exception, "exception");

            if (exception.InnerException != null)
            {
                this.InnerError = new ODataInnerError(exception.InnerException);
            }

            Properties = new Dictionary<string, ODataValue>
            {
                //// NOTE: we add empty elements if no information is provided for the message, error type and stack trace
                ////       to stay compatible with Astoria.
                { JsonConstants.ODataErrorInnerErrorMessageName, exception.Message.ToODataValue() ?? new ODataNullValue() },
                { JsonConstants.ODataErrorInnerErrorTypeNameName, exception.GetType().FullName.ToODataValue() ?? new ODataNullValue() },
                { JsonConstants.ODataErrorInnerErrorStackTraceName, exception.StackTrace.ToODataValue() ?? new ODataNullValue() }
            };
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="ODataInnerError" /> class with a dictionary of property names and corresponding ODataValues.
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
        [Obsolete("This will be dropped in the 9.x release. The contents of inner error object should be service-defined. There are no required properties.")]
        public string Message
        {
            get { return GetStringValue(JsonConstants.ODataErrorInnerErrorMessageName); }
            set { SetStringValue(JsonConstants.ODataErrorInnerErrorMessageName, value); }
        }

        /// <summary>Gets or sets the type name of this error, for example, the type name of an exception.</summary>
        /// <returns>The type name of this error.</returns>
        [Obsolete("This will be dropped in the 9.x release. The contents of inner error object should be service-defined. There are no required properties.")]
        public string TypeName
        {
            get { return GetStringValue(JsonConstants.ODataErrorInnerErrorTypeNameName); }
            set { SetStringValue(JsonConstants.ODataErrorInnerErrorTypeNameName, value); }
        }

        /// <summary>Gets or sets the stack trace for this error.</summary>
        /// <returns>The stack trace for this error.</returns>
        [Obsolete("This will be dropped in the 9.x release. The contents of inner error object should be service-defined. There are no required properties.")]
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
        internal string ToJsonString()
        {
            StringBuilder builder = new StringBuilder();
            bool firstProperty = true;

            builder.Append('{');

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

                if (!firstProperty)
                {
                    builder.Append(',');
                }
                else
                {
                    firstProperty = false;
                }

                builder.Append('"').Append(keyValuePair.Key).Append('"').Append(':');
                ODataJsonWriterUtils.ODataValueToString(builder, keyValuePair.Value);
            }

            if (!firstProperty)
            {
                builder.Append(',');
            }

            string message = this.Message == null ? string.Empty : JsonValueUtils.GetEscapedJsonString(this.Message);
            string typeName = this.TypeName == null ? string.Empty : JsonValueUtils.GetEscapedJsonString(this.TypeName);
            string stackTrace = this.StackTrace == null ? string.Empty : JsonValueUtils.GetEscapedJsonString(this.StackTrace);

            builder.Append('"').Append(JsonConstants.ODataErrorInnerErrorMessageName).Append("\":");
            builder.Append('"').Append(message).Append('"');

            builder.Append(",\"").Append(JsonConstants.ODataErrorInnerErrorTypeNameName).Append("\":");
            builder.Append('"').Append(typeName).Append('"');

            builder.Append(",\"").Append(JsonConstants.ODataErrorInnerErrorStackTraceName).Append("\":");
            builder.Append('"').Append(stackTrace).Append('"');

            if (this.InnerError != null)
            {
                string innerErrorJsonString = this.InnerError.ToJsonString();
                builder.Append(",\"").Append(JsonConstants.ODataErrorInnerErrorName).Append("\":");
                builder.Append(innerErrorJsonString);
            }

            builder.Append('}');

            return builder.ToString();
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
