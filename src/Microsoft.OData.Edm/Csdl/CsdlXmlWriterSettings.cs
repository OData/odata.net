//---------------------------------------------------------------------
// <copyright file="CsdlXmlWriterSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl
{
    /// <summary>
    /// The options/setting for CSDL writer
    /// </summary>
    public class CsdlXmlWriterSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether the XML representation of the "Scale" attribute 
        /// should use lowercase for the value "Variable".
        /// </summary>
        public bool UseLowercaseScaleVariable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the XML representation of the "Srid" attribute 
        /// should use lowercase for the value "Variable".
        /// </summary>
        public bool UseLowercaseSridVariable { get; set; }
    }
}
