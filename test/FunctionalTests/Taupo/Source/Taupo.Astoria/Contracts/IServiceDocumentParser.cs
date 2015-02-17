//---------------------------------------------------------------------
// <copyright file="IServiceDocumentParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Interface allows developers to get a dictionary of entity set name to Uri for a given OData Service
    /// </summary>
    [ImplementationSelector("ServiceDocumentParser", DefaultImplementation = "Default")]
    public interface IServiceDocumentParser
    {
        /// <summary>
        /// Parses the Service document and builds a Dictionary of entity set name to Uri 
        /// </summary>
        /// <param name="serviceDocument">The Service Document of an OData compliant service</param>
        /// <returns>A Dictionary of entity set name to Uri </returns>
        IEntitySetResolver ParseServiceDocument(XElement serviceDocument);
    }
}
