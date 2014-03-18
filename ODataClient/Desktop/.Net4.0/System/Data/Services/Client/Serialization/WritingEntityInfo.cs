//---------------------------------------------------------------------
// <copyright file="WritingEntityInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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