//---------------------------------------------------------------------
// <copyright file="JsonReaderOptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;

namespace Microsoft.OData.Edm.Csdl.Json
{
    /// <summary>
    /// Configuration settings for CSDL JSON reader.
    /// </summary>
    public class JsonReaderOptions
    {
        /// <summary>
        /// The default CsdlJsonWriterSettings.
        /// </summary>
        internal static JsonReaderOptions Default = new JsonReaderOptions
        {
            IsIeee754Compatible = false
        };

        public JsonReaderOptions()
        {
            PropertyConvertTo = (s) => s;
            PropertyConvertFrom = (s) => s;
        }

        /// <summary>
        /// Gets/sets a value indicating whether the writer write large integers as strings.
        /// The IEEE754Compatible=true parameter indicates that the service MUST serialize Edm.Int64 and Edm.Decimal numbers as strings.
        /// This is in conformance with [RFC7493]. If not specified, or specified as IEEE754Compatible=false, all numbers MUST be serialized as JSON numbers.
        /// </summary>
        public bool IsIeee754Compatible { get; set; }

        // <summary>
        /// True to read the CSDL and return "EdmModel"
        /// False to read the CSDL and return "CsdlSemanticsModel"
        /// </summary>
        public bool ReadAsImmutableModel { get; set; }

        public bool IgnoreUnexpectedAttributesAndElements { get; set; }

        public Func<Uri, TextReader> ReferencedModelFunc { get; set; } // Url -> TextReader

        public Func<string, string> PropertyConvertTo { get; set; }
        public Func<string, string> PropertyConvertFrom { get; set; }
    }
}