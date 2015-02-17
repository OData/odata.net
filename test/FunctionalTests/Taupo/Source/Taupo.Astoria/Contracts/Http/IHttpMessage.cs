//---------------------------------------------------------------------
// <copyright file="IHttpMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Http
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Read-only interface for HTTP-based messages
    /// </summary>
    public interface IHttpMessage : IMimePart
    {
        /// <summary>
        /// Returns the first line of the message
        /// </summary>
        /// <returns>The first line of the message</returns>
        string GetFirstLine();
    }
}
