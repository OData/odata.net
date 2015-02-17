//---------------------------------------------------------------------
// <copyright file="ReusableStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.OptionalProviders
{
    using System;
    using System.IO;

    /// <summary>
    /// Memory stream that can be reused after disposing, which will simply set the position back to the start
    /// </summary>
    internal class ReusableStream : MemoryStream, IDisposable
    {
        /// <summary>
        /// Implementation of the Dispose() function
        /// </summary>
        void IDisposable.Dispose()
        {
            // Resets the stream
            this.Position = 0;
        }

        /// <summary>
        /// Resets the stream 
        /// </summary>
        /// <param name="disposing">Whether or not to dispose managed resources</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2215: Dispose methods should call base class dispose", Justification = "Not necessary to call base.Dispose() in this implementation")]
        protected override void Dispose(bool disposing)
        {
            ((IDisposable)this).Dispose();
        }
    }
}