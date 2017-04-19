//---------------------------------------------------------------------
// <copyright file="BatchContentType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Net;
using System.Text;

namespace Microsoft.OData.Client
{
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Batch content type header information.
    /// </summary>
    public sealed class BatchContentType
    {
        public static string ApplicationJson = "application/json";

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
        private readonly string contentType;

        /// <summary>
        /// Content-Type parameters.
        /// </summary>
        private IList<KeyValuePair<string, string>> parameters;

        public BatchContentType(string contentType)
            : this(contentType, null)
        {
        }

        public BatchContentType(string contentType, IList<KeyValuePair<string, string>> parameters)
        {
            ValidateType(contentType);
            this.contentType = contentType;
            this.parameters = parameters;
        }

        private static void ValidateType(string contentType)
        {
            if (!(contentType.Equals(XmlConstants.MimeMultiPartMixed) || contentType.Equals(ApplicationJson)))
            {
                throw Error.InvalidOperation(Strings.Batch_UnsupportedBatchContentType(
                    contentType,
                    XmlConstants.MimeMultiPartMixed,
                    ApplicationJson));
            }
        }

        /// <summary>
        /// Get the value of content type.
        /// </summary>
        public string ContentType
        {
            get { return this.contentType; }
        }

        /// <summary>
        /// Get the value of parameters for enumeration.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Parameters
        {
            get { return this.parameters; }
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
            if (this.contentType.Equals(XmlConstants.MimeMultiPartMixed))
            {
                return string.Format(
                    CultureInfo.InvariantCulture, "{0}; {1}={2}_{3}",
                    XmlConstants.MimeMultiPartMixed,
                    XmlConstants.HttpMultipartBoundary,
                    XmlConstants.HttpMultipartBoundaryBatch,
                    Guid.NewGuid());
            }
            else if (this.contentType.Equals(ApplicationJson))
            {
                if (this.parameters.Count == 0)
                {
                    return ApplicationJson;
                }
                else
                {
                    StringBuilder sb = new StringBuilder(ApplicationJson);
                    foreach (var pair in this.parameters)
                    {
                        sb.Append(";")
                          .Append(string.Format(CultureInfo.InvariantCulture, "{0}={1}", pair.Key, pair.Value));
                    }
                    return sb.ToString();
                }
            }

            throw Error.InvalidOperation(Strings.Batch_UnsupportedBatchContentType(
                this.contentType,
                XmlConstants.MimeMultiPartMixed,
                ApplicationJson));
        }
    }
}
