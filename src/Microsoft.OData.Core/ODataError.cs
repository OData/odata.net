//---------------------------------------------------------------------
// <copyright file="ODataError.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Class representing an error payload.
    /// </summary>
    [DebuggerDisplay("{ErrorCode}: {Message}")]
#if ORCAS
    [Serializable]
#endif
    public sealed class ODataError : ODataAnnotatable
    {
        /// <summary>Gets or sets the error code to be used in payloads.</summary>
        /// <returns>The error code to be used in payloads.</returns>
        public string ErrorCode
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
            return string.Format(CultureInfo.InvariantCulture,
                "{{\"error\":{{" +
                "\"code\":\"{0}\"," +
                "\"message\":\"{1}\"," +
                "\"target\":\"{2}\"," +
                "\"details\":{3}," +
                "\"innererror\":{4}" +
                " }}}}",
                this.ErrorCode == null ? ""  : JsonValueUtils.GetEscapedJsonString(this.ErrorCode),
                this.Message  == null ? ""  : JsonValueUtils.GetEscapedJsonString(this.Message),
                this.Target == null ? "" : JsonValueUtils.GetEscapedJsonString(this.Target),
                this.Details == null ? "{}" : GetJsonStringForDetails(),
                this.InnerError == null ? "{}" : this.InnerError.ToJson());
        }

        /// <summary>
        /// Convert the Details property to Json format string.
        /// </summary>
        /// <returns>Json format string representing collection.</returns>
        private string GetJsonStringForDetails()
        {
            return "[" + String.Join(",", this.Details.Select(i => i.ToJson()).ToArray()) + "]";
        }
}
}
