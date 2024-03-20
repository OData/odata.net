//---------------------------------------------------------------------
// <copyright file="CsdlXmlWriterSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl
{
    /// <summary>
    /// Provides settings to be used with <see cref="CsdlXmlWriter"/>.
    /// </summary>
    public sealed class CsdlXmlWriterSettings
    {
        /// <summary>
        /// Gets or sets the compatibility flags for the <see cref="CsdlXmlWriter"/> to maintain compatibility with legacy functionality.
        /// </summary>
        public EdmLibraryCompatibility LibraryCompatibility { get; set; }
    }
}
