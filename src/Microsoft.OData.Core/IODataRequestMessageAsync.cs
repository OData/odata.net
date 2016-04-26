//---------------------------------------------------------------------
// <copyright file="IODataRequestMessageAsync.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if PORTABLELIB
namespace Microsoft.OData
{
    #region Namespaces
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading.Tasks;
    #endregion Namespaces

    /// <summary>
    /// Interface for asynchronous OData request messages.
    /// </summary>
    public interface IODataRequestMessageAsync : IODataRequestMessage
    {
        /// <summary>Asynchronously get the stream backing for this message.</summary>
        /// <returns>The stream for this message.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is intentionally a method.")]
        Task<Stream> GetStreamAsync();
    }
}
#endif
