//---------------------------------------------------------------------
// <copyright file="DefaultProtocolFormatNormalizerSelector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.OData;

    [ImplementationName(typeof(IProtocolFormatNormalizerSelector), "Default",
    HelpText = "Default payload normalization selector")]
    /// <summary>
    /// Gets the normalizer based on content type
    /// </summary>
    public class DefaultProtocolFormatNormalizerSelector : IProtocolFormatNormalizerSelector
    {
        /// <summary>
        /// Gets or sets the normalizer to use for json
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public JsonPayloadNormalizer JsonNormalizer { get; set; }

        /// <summary>
        /// Gets or sets the normalizer to use for xml
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public XmlPayloadNormalizer XmlNormalizer { get; set; }

        /// <summary>
        /// Returns the strategy to use for serializing/deserialzing the given content type
        /// </summary>
        /// <param name="contentType">The content type</param>
        /// <param name="uri">The request uri</param>
        /// <returns>A serialization strategy</returns>
        public IODataPayloadElementNormalizer GetNormalizer(string contentType)
        {
            if (IsXmlMimeType(contentType))
            {
                return this.XmlNormalizer;
            }

            if (IsJsonMimeType(contentType))
            {
                return this.JsonNormalizer;
            }

            return null;
        }

        private static bool IsXmlMimeType(string contentType)
        {
            return contentType.StartsWith(MimeTypes.ApplicationAtomXml, StringComparison.OrdinalIgnoreCase)
                || contentType.StartsWith(MimeTypes.ApplicationXml, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsJsonMimeType(string contentType)
        {
            return contentType.StartsWith(MimeTypes.ApplicationJson, StringComparison.OrdinalIgnoreCase);
        }
    }
}
