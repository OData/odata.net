//---------------------------------------------------------------------
// <copyright file="ODataErrorDetail.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------


namespace Microsoft.OData
{
    #region
    using System.Globalization;
    #endregion

    /// <summary>
    /// Class representing an error detail.
    /// </summary>
    public sealed class ODataErrorDetail
    {
        /// <summary>Gets or sets the error code to be used in payloads.</summary>
        /// <returns>The error code to be used in payloads.</returns>
        public string ErrorCode { get; set; }

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
        internal string ToJson()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{{ \"errorcode\": \"{0}\", \"message\": \"{1}\", \"target\": \"{2}\" }}",
                this.ErrorCode ?? "", this.Message ?? "", this.Target ?? "");
        }
}
}
