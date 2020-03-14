//---------------------------------------------------------------------
// <copyright file="CsdlReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Csdl.Reader;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies.Community.V1;
using Microsoft.OData.Edm.Vocabularies.V1;

namespace Microsoft.OData.Edm.Csdl
{
    /// <summary>
    /// 
    /// </summary>
    public class CsdlSerializerOptions
    {
        internal static readonly CsdlSerializerOptions DefaultOptions = new CsdlSerializerOptions();

        public CsdlSerializerOptions()
        {
            IgnoreUnexpectedAttributesAndElements = true;
            IsIeee754Compatible = false;
            ReadAsImmutableModel = true;
        }

        public IEnumerable<EdmError> Errors { get; set; }

        /// <summary>
        /// Ignore the unexpected attributes and elements.
        /// </summary>
        public bool IgnoreUnexpectedAttributesAndElements { get; set; }

        public bool ThrowOnUnexpectedAttributesAndElements { get; set; }

        /// <summary>
        /// Gets/sets a value indicating whether the writer write large integers as strings.
        /// The IEEE754Compatible=true parameter indicates that the service MUST serialize Edm.Int64 and Edm.Decimal numbers as strings.
        /// This is in conformance with [RFC7493]. If not specified, or specified as IEEE754Compatible=false, all numbers MUST be serialized as JSON numbers.
        /// </summary>
        public bool IsIeee754Compatible { get; set; }

        /// <summary>
        /// True to read the CSDL and return "EdmModel"
        /// False to read the CSDL and return "CsdlSemanticsModel"
        /// </summary>
        public bool ReadAsImmutableModel { get; set; }

        /// <summary>
        /// Defines whether JSON should pretty print which includes:
        /// indenting nested JSON tokens, adding new lines, and adding white space between property names and values.
        /// By default, the JSON is serialized without any extra white space.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this property is set after serialization or deserialization has occurred.
        /// </exception>
        public bool Indented
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the maximum depth allowed when serializing or deserializing JSON, with the default (i.e. 0) indicating a max depth of 64.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this property is set after serialization or deserialization has occurred.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the max depth is set to a negative value.
        /// </exception>
        public int? MaxDepth
        {
            get;
            set;
        }

        internal JsonReaderOptions GetJsonReaderOptions()
        {
            return new JsonReaderOptions
            {
                IsIeee754Compatible = IsIeee754Compatible,
                ReadAsImmutableModel = ReadAsImmutableModel,
                IgnoreUnexpectedAttributesAndElements = IgnoreUnexpectedAttributesAndElements
            };
        }
    }


}
