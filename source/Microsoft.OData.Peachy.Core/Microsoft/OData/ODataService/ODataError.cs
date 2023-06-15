namespace Microsoft.OData.ODataService
{
    using System;
    using System.Collections.Generic;
    using System.Net.NetworkInformation;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// The implementation of this class is based on <see href="https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#sec_ErrorResponseBody">the standard</see>, particularly:
    /// > details: optional, potentially empty collection of structured instances with code, message, and target following the rules above.
    /// </remarks>
    public sealed class ODataErrorDetail
    {
        public ODataErrorDetail(string code, string message, string? target)
        {
            // From the standard:
            // > code: required non-null ... string
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            // From the standard:
            // > code: required ... non-empty ... string
            if (code.Length == 0)
            {
                throw new ArgumentException(nameof(code));
            }

            // From the standard:
            // > code: required non-null ... string
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            // From the standard:
            // > code: required ... non-empty ... string
            if (message.Length == 0)
            {
                throw new ArgumentException(nameof(message));
            }

            this.code = code;
            this.message = message;
            this.target = target;
        }

        public string code { get; }

        public string message { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// From the standard:
        /// > target: optional nullable, potentially empty string
        /// </remarks>
        public string? target { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// The implementation of this class is based on <see href="https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#sec_ErrorResponseBody">the standard</see>.
    /// </remarks>
    public sealed class ODataError
    {
        public ODataError(string code, string message, string? target, IEnumerable<ODataErrorDetail>? details) //// TODO how to make target "optional"?
        {
            // From the standard:
            // > code: required non-null ... string
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            // From the standard:
            // > code: required ... non-empty ... string
            if (code.Length == 0)
            {
                throw new ArgumentException(nameof(code));
            }

            // From the standard:
            // > code: required non-null ... string
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            // From the standard:
            // > code: required ... non-empty ... string
            if (message.Length == 0)
            {
                throw new ArgumentException(nameof(message));
            }

            this.code = code;
            this.message = message;
            this.target = target;
            this.details = details;
            //// TODO inner error?
        }

        public string code { get; }

        public string message { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// From the standard:
        /// > target: optional nullable, potentially empty string
        /// </remarks>
        public string? target { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// From the standard:
        /// > details: optional, potentially empty collection
        /// </remarks>
        public IEnumerable<ODataErrorDetail>? details { get; }
    }
}
