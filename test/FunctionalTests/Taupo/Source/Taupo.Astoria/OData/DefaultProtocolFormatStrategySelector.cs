//---------------------------------------------------------------------
// <copyright file="DefaultProtocolFormatStrategySelector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Default payload formatting selector
    /// </summary>
    [ImplementationName(typeof(IProtocolFormatStrategySelector), "Default", HelpText = "Default payload serialization strategy selector")]
    public class DefaultProtocolFormatStrategySelector : IProtocolFormatStrategySelector
    {
        /// <summary>
        /// Gets or sets the strategy to use for xml
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public XmlProtocolFormatStrategy XmlStrategy { get; set; }

        /// <summary>
        /// Gets or sets the strategy to use for json
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public JsonProtocolFormatStrategy JsonStrategy { get; set; }

        /// <summary>
        /// Gets or sets the strategy to use for raw $value requests that have text-based content types
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public TextValueProtocolFormatStrategy TextValueStrategy { get; set; }

        /// <summary>
        /// Gets or sets the strategy to use for raw $value requests that have binary-based content types
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public BinaryValueProtocolFormatStrategy BinaryValueStrategy { get; set; }

        /// <summary>
        /// Gets or sets the strategy to use for $count requests
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public CountProtocolFormatStrategy CountStrategy { get; set; }

        /// <summary>
        /// Gets or sets the strategy to use for html form requests
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public HtmlFormProtocolFormatStrategy HtmlFormStrategy { get; set; }

        /// <summary>
        /// Returns the strategy to use for serializing/deserialzing the given content type
        /// </summary>
        /// <param name="contentType">The content type</param>
        /// <param name="uri">The request uri</param>
        /// <returns>A serialization strategy</returns>
        public virtual IProtocolFormatStrategy GetStrategy(string contentType, ODataUri uri)
        {
            if (uri != null)
            {
                // if its a named stream or an MLE, handle the returned payload as a binary stream
                if (uri.IsNamedStream() || uri.IsMediaResource())
                {
                    return this.BinaryValueStrategy;
                }

                // if its a raw $count request, we need to use a different strategy
                if (uri.IsCount() && IsPlainTextMimeType(contentType))
                {
                    return this.CountStrategy;
                }
            }

            if (IsXmlMimeType(contentType))
            {
                return this.XmlStrategy;
            }
            
            if (IsJsonMimeType(contentType))
            {
                return this.JsonStrategy;
            }

            if (IsTextBasedMimeType(contentType))
            {
                return this.TextValueStrategy;
            }

            if (IsHtmlFormMimeType(contentType))
            {
                return this.HtmlFormStrategy;
            }

            return this.BinaryValueStrategy;
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

        private static bool IsPlainTextMimeType(string contentType)
        {
            return contentType.StartsWith(MimeTypes.TextPlain, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsHtmlMimeType(string contentType)
        {
            return contentType.StartsWith(MimeTypes.TextHtml, StringComparison.Ordinal);
        }

        private static bool IsTextBasedMimeType(string contentType)
        {
            return IsPlainTextMimeType(contentType) || IsHtmlMimeType(contentType) || IsXmlMimeType(contentType) || IsJsonMimeType(contentType);
        }

        private static bool IsHtmlFormMimeType(string contentType)
        {
            return contentType.StartsWith(MimeTypes.ApplicationFormUrlEncoded, StringComparison.OrdinalIgnoreCase);
        }
    }
}
