//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Http
{
    #region Namespaces.

    using System;
    using System.IO;

    #endregion Namespaces.

    /// <summary>Provides a response from a Uniform Resource Identifier (URI).</summary>
    internal abstract class WebResponse : IDisposable
    {
        /// <summary>When overridden in a descendant class, gets the content length of data being received.</summary>
        public abstract long ContentLength
        { 
            get;
        }

        /// <summary>When overridden in a derived class, gets the content type of the data being received.</summary>
        public abstract string ContentType
        {
            get;
        }

        /// <summary>When overridden by a descendant class, closes the response stream.</summary>
        public abstract void Close();

        /// <summary>Gets the stream with the response contents.</summary>
        /// <returns>The stream with the response contents.</returns>
        public abstract Stream GetResponseStream();

        /// <summary>Releases resources.</summary>
        void IDisposable.Dispose()
        {
            this.Dispose(false);
        }

        /// <summary>Releases resources.</summary>
        /// <param name="disposing">Whether the dispose is being called explicitly rather than by the GC.</param>
        protected abstract void Dispose(bool disposing);
    }
}

