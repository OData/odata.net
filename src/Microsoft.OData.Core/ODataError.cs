//---------------------------------------------------------------------
// <copyright file="ODataError.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Class representing an error payload.
    /// </summary>
    [DebuggerDisplay("{Code}: {Message}")]
    public sealed class ODataError : ODataAnnotatable
    {
        /// <summary>Gets or sets the error code to be used in payloads.</summary>
        /// <returns>The error code to be used in payloads.</returns>
        public string Code
        {
            get;
            set;
        }

        /// <summary>Gets or sets the error message.</summary>
        /// <returns>The error message.</returns>
        public string Message
        {
            get;
            set;
        }

        /// <summary>Gets or sets the target of the particular error.</summary>
        /// <returns>For example, the name of the property in error</returns>
        public string Target { get; set; }

        /// <summary>
        /// A collection of JSON objects that MUST contain name/value pairs for code and message, and MAY contain
        /// a name/value pair for target, as described above.
        /// </summary>
        /// <returns>The error details.</returns>
        public ICollection<ODataErrorDetail> Details { get; set; }

        /// <summary>Gets or sets the implementation specific debugging information to help determine the cause of the error.</summary>
        /// <returns>The implementation specific debugging information.</returns>
        public ODataInnerError InnerError
        {
            get;
            set;
        }

        /// <summary>
        /// Collection of custom instance annotations.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "We want to allow the same instance annotation collection instance to be shared across ODataLib OM instances.")]
        public ICollection<ODataInstanceAnnotation> InstanceAnnotations
        {
            get { return this.GetInstanceAnnotations(); }
            set { this.SetInstanceAnnotations(value); }
        }

        /// <summary>
        /// Serialization to Json format string representing the error object.
        /// </summary>
        /// <returns>The string in Json format.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            // `code` and `message` must be included
            string code = this.Code == null ? string.Empty : JsonValueUtils.GetEscapedJsonString(this.Code);
            string message = this.Message == null ? string.Empty : JsonValueUtils.GetEscapedJsonString(this.Message);

            builder.Append("{\"error\":{");
            builder.Append('"').Append(JsonConstants.ODataErrorCodeName).Append("\":");
            builder.Append('"').Append(code).Append('"');
            builder.Append(",\"").Append(JsonConstants.ODataErrorMessageName).Append("\":");
            builder.Append('"').Append(message).Append('"');

            if (this.Target != null)
            {
                builder.Append(",\"").Append(JsonConstants.ODataErrorTargetName).Append("\":");
                builder.Append('"').Append(JsonValueUtils.GetEscapedJsonString(this.Target)).Append('"');
            }

            if (this.Details != null)
            {
                builder.Append(",\"").Append(JsonConstants.ODataErrorDetailsName).Append("\":");
                builder.Append(GetJsonStringForDetails());
            }

            if (this.InnerError != null)
            {
                builder.Append(",\"").Append(JsonConstants.ODataErrorInnerErrorName).Append("\":");
                builder.Append(this.InnerError.ToJsonString());
            }

            builder.Append("}}");

            return builder.ToString();
        }

        /// <summary>
        /// Convert the Details property to Json format string.
        /// </summary>
        /// <returns>Json format string representing collection.</returns>
        private string GetJsonStringForDetails()
        {
            Debug.Assert(this.Details != null, "this.Details != null");

            StringBuilder builder = new StringBuilder();

            builder.Append('[');
            builder.AppendJoin(',', this.Details.Where(d => d != null).Select(d => d.ToJsonString()));
            builder.Append(']');

            return builder.ToString();
        }
}
}
