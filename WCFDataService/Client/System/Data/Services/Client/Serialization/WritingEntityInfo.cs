//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Client
{
    using System.Diagnostics;
    using System.Xml.Linq;

    /// <summary>
    /// To cache the entity instance as annotation for firing WritingEntity event
    /// </summary>
    internal sealed class WritingEntityInfo
    {
        /// <summary>Entity instance that is currently getting serialized.</summary>
        internal readonly object Entity;

        /// <summary>XDocument instance to cache the payload.</summary>
        internal readonly XDocument EntryPayload;

        /// <summary>RequestInfo instance.</summary>
        internal readonly RequestInfo RequestInfo;

        /// <summary>
        /// Creates a new instance of WritingEntityInfo
        /// </summary>
        /// <param name="entity">Entity instance that is currently getting serialized.</param>
        /// <param name="requestInfo">RequestInfo instance.</param>
        internal WritingEntityInfo(object entity, RequestInfo requestInfo)
        {
            Debug.Assert(entity != null, "entity != null");
            this.Entity = entity;
            this.EntryPayload = new XDocument();
            this.RequestInfo = requestInfo;
        }
    }
}
