//---------------------------------------------------------------------
// <copyright file="ODataErrorDetail.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------


namespace Microsoft.OData
{
    #region Namespaces
    using System.Text;
    using Microsoft.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Class representing an error detail.
    /// </summary>
    public sealed class ODataErrorDetail
    {
        /// <summary>Gets or sets the error code to be used in payloads.</summary>
        /// <returns>The error code to be used in payloads.</returns>
        public string Code { get; set; }

        /// <summary>Gets or sets the error message.</summary>
        /// <returns>The error message.</returns>
        public string Message { get; set; }

        /// <summary>Gets or sets the target of the particular error.</summary>
        /// <returns>For example, the name of the property in error</returns>
        public string Target { get; set; }

        /// <summary>
        /// Serialization to Json format string.
        /// </summary>
        /// <returns>The string in Json format</returns>
        internal string ToJsonString()
        {
            StringBuilder builder = new StringBuilder();

            // `code` and `message` must be included
            string code = this.Code == null ? string.Empty : JsonValueUtils.GetEscapedJsonString(this.Code);
            string message = this.Message == null ? string.Empty : JsonValueUtils.GetEscapedJsonString(this.Message);

            builder.Append('{');
            builder.Append('"').Append(JsonConstants.ODataErrorCodeName).Append("\":");
            builder.Append('"').Append(code).Append('"');
            builder.Append(",\"").Append(JsonConstants.ODataErrorMessageName).Append("\":");
            builder.Append('"').Append(message).Append('"');

            if (this.Target != null)
            {
                builder.Append(",\"").Append(JsonConstants.ODataErrorTargetName).Append("\":");
                builder.Append("\"").Append(JsonValueUtils.GetEscapedJsonString(this.Target)).Append('"');
            }

            builder.Append('}');

            return builder.ToString();
        }
    }
}
