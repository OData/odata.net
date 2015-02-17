//---------------------------------------------------------------------
// <copyright file="XmlData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Data
{
    #region Namespaces

    using System;
    using System.Text;

    #endregion Namespaces

    /// <summary>Provides information about interesting XML values.</summary>
    public sealed class XmlData
    {
        /// <summary>XML namespace for data services.</summary>
        public const string DataWebNamespace = "http://docs.oasis-open.org/odata/ns/data";

        /// <summary>XML namespace for data service annotations.</summary>
        public const string DataWebMetadataNamespace = "http://docs.oasis-open.org/odata/ns/metadata";

        /// <summary>XML namespace for edm.</summary>
        public const string DataWebEdmNamespace = "http://docs.oasis-open.org/odata/ns/edm";
    }
}
