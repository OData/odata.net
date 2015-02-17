//---------------------------------------------------------------------
// <copyright file="IMimePart.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Http
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Interface for representing a part of a MIME body
    /// </summary>
    public interface IMimePart
    {
        /// <summary>
        /// Gets the headers for the MIME part
        /// </summary>
        IDictionary<string, string> Headers { get; }
    }
}
