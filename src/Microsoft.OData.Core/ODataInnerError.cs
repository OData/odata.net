//---------------------------------------------------------------------
// <copyright file="ODataInnerError.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Class representing implementation specific debugging information to help determine the cause of the error.
    /// </summary>
    public sealed class ODataInnerError
    {
        /// <summary>Initializes a new instance of the <see cref="ODataInnerError" /> class.</summary>
        public ODataInnerError()
        {
            Properties = new Dictionary<string, ODataValue>();
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
                if (!firstProperty)
                {
                    builder.Append(',');
                }
                else
                {
                    firstProperty = false;
                }

                builder.Append('"').Append(JsonValueUtils.GetEscapedJsonString(keyValuePair.Key)).Append('"').Append(':');
                ODataJsonWriterUtils.ODataValueToString(builder, keyValuePair.Value);
            }

            if (this.InnerError != null)
            {
                string innerErrorJsonString = this.InnerError.ToJsonString();
                if (!string.IsNullOrEmpty(innerErrorJsonString))
                {
                    if (!firstProperty)
                    {
                        builder.Append(',');
                    }
                    
                    builder.Append('"').Append(JsonConstants.ODataErrorInnerErrorName).Append("\":");
                    builder.Append(innerErrorJsonString);
                }
            }

            builder.Append('}');

            return builder.ToString();
        }
    }
}
