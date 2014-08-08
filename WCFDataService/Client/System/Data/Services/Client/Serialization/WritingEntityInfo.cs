//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
