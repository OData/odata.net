//---------------------------------------------------------------------
// <copyright file="BatchContentType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Text;

    #endregion Namespaces

    /// <summary>
    /// Batch content type header information.
    /// </summary>
    public sealed class BatchContentType
    {
        /// <summary>
        /// Enum for the full type (main type & sub type) supported for batch.
        /// </summary>
        public enum BatchFullType
        {
            BatchApplicationJson,
            BatchMultipartMixed
        }

        /// <summary>
        /// Convenient public constant for application/json.
        /// </summary>
        public const BatchFullType TypeApplicationJson = BatchFullType.BatchApplicationJson;

        /// <summary>
        /// Convenient public constant for multipart/mixed.
        /// </summary>
        public const BatchFullType TypeMultipartMixed = BatchFullType.BatchMultipartMixed;

        /// <summary>
        /// MIME media full type string for application/json.
        /// </summary>
        public static string MimeApplicationJson = "application/json";

        /// <summary>
        /// MIME media full type string for multipart/mixed.
        /// </summary>
        public static string MimeMultipartMixed = "multipart/mixed";

        /// <summary>JSON Light parameter name for 'odata.metadata' parameter.</summary>
        public const string MimeMetadataParameterName = "odata.metadata";

        /// <summary>JSON Light parameter value 'full'.</summary>
        public static string MimeMetadataParameterValueFull = "full";

        /// <summary>JSON Light parameter value 'minimal'.</summary>
        public static string MimeMetadataParameterValueMinimal = "minimal";

        /// <summary>JSON Light parameter value 'none'.</summary>
        public static string MimeMetadataParameterValueNone = "none";

        /// <summary>JSON Light Parameter name for 'odata.streaming' parameter.</summary>
        public static string MimeStreamingParameterName = "odata.streaming";

        /// <summary>JSON Light parameter name for 'IEEE754Compatible' parameter.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ieee")]
        public static string MimeIeee754CompatibleParameterName = "IEEE754Compatible";

        /// <summary>JSON Light parameter value 'true'.</summary>
        public const string MimeParameterValueTrue = "true";

        /// <summary>JSON Light parameter value 'false'.</summary>
        public const string MimeParameterValueFalse = "false";

        /// <summary>
        /// Content-Type.
        /// </summary>
        private readonly BatchFullType fullType;

        /// <summary>
        /// Content-Type parameters.
        /// </summary>
        private IList<KeyValuePair<string, string>> parameters;

        /// <summary>
        /// Constructor without parameters for Content-Type header value.
        /// </summary>
        /// <param name="fullType">Enum value of the batch full Content-Type.</param>
        public BatchContentType(BatchFullType fullType)
            : this(fullType, null)
        {
        }

        /// <summary>
        /// Constructor with parameters for Content-Type header value.
        /// </summary>
        /// <param name="fullType">Enum value of the batch full Content-Type.</param>
        /// <param name="parameters">List of parameters for the header value.</param>
        private BatchContentType(BatchFullType fullType, IList<KeyValuePair<string, string>> parameters)
        {
            this.fullType = fullType;
            this.parameters = parameters;
        }

        /// <summary>
        /// Add a parametee name-value pair.
        /// </summary>
        /// <param name="parameterName">Name of the parameter to add.</param>
        /// <param name="parameterValue">Value of the parameter to add.</param>
        public BatchContentType AddParameter(string parameterName, string parameterValue)
        {
            if (this.parameters == null)
            {
                this.parameters = new List<KeyValuePair<string, string>>();
            }
            this.parameters.Add(new KeyValuePair<string, string>(parameterName, parameterValue));
            return this;
        }

        /// <summary>
        /// Construct the Content-Type header string value.
        /// </summary>
        /// <returns>A string.</returns>
        public string CreateMimeContentTypeValue()
        {
            string value = null;

            switch (this.fullType)
            {
                case TypeMultipartMixed:
                {
                    value = string.Format(
                        CultureInfo.InvariantCulture, "{0}; {1}={2}_{3}",
                        XmlConstants.MimeMultiPartMixed,
                        XmlConstants.HttpMultipartBoundary,
                        XmlConstants.HttpMultipartBoundaryBatch,
                        Guid.NewGuid());
                }
                break;

                case TypeApplicationJson:
                {
                    value = string.Format(CultureInfo.InvariantCulture, "{0}/{1}",
                        XmlConstants.MimeApplicationType,
                        XmlConstants.MimeJsonSubType);
                    if (this.parameters.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder(value);
                        foreach (var pair in this.parameters)
                        {
                            sb.Append(";")
                              .Append(string.Format(CultureInfo.InvariantCulture, "{0}={1}", pair.Key, pair.Value));
                        }
                        value = sb.ToString();
                    }
                }
                break;

                default:
                {
                    throw Error.InvalidOperation(Strings.Batch_UnsupportedBatchContentType(
                        this.fullType.ToString(),
                        XmlConstants.MimeMultiPartMixed,
                        MimeApplicationJson));
                }
            }

            return value;
        }
    }
}
