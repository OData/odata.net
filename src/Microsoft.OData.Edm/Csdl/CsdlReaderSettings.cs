//---------------------------------------------------------------------
// <copyright file="CsdlReaderSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl
{
    using System;
    using System.Xml;

    /// <summary>
    /// The base setting for CSDL reader
    /// </summary>
    public abstract class CsdlReaderSettingsBase
    {
        // Empty now
    }

    /// <summary>
    /// Settings used when parsing CSDL document.
    /// </summary>
    public sealed class CsdlReaderSettings : CsdlReaderSettingsBase
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CsdlReaderSettings()
        {
            this.IgnoreUnexpectedAttributesAndElements = false;
        }

        /// <summary>
        /// The function to load referenced model xml. If null, will stop loading the referenced models. Normally it should throw no exception.
        /// </summary>
        public Func<Uri, XmlReader> GetReferencedModelReaderFunc { get; set; }

        /// <summary>
        /// Ignore the unexpected attributes and elements in schema.
        /// </summary>
        public bool IgnoreUnexpectedAttributesAndElements { get; set; }
    }
}
