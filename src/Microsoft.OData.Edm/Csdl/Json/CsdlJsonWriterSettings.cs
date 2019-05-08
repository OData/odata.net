//---------------------------------------------------------------------
// <copyright file="CsdlJsonWriterSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Json
{
    /// <summary>
    /// Configuration settings for CSDL JSON writers.
    /// </summary>
    public class CsdlJsonWriterSettings
    {
        /// <summary>
        /// The default CsdlJsonWriterSettings.
        /// </summary>
        internal static CsdlJsonWriterSettings Default = new CsdlJsonWriterSettings
        {
            Indent = true,
            IsIeee754Compatible = false
        };

        /// <summary>
        /// Gets/sets a value indicating whether the writer write large integers as strings.
        /// The IEEE754Compatible=true parameter indicates that the service MUST serialize Edm.Int64 and Edm.Decimal numbers as strings.
        /// This is in conformance with [RFC7493]. If not specified, or specified as IEEE754Compatible=false, all numbers MUST be serialized as JSON numbers.
        /// </summary>
        public bool IsIeee754Compatible { get; set; }

        /// <summary>
        /// Gets/sets a value indicating whether the output is indented.
        /// </summary>
        public bool Indent { get; set; }
    }
}