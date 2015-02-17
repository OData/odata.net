//---------------------------------------------------------------------
// <copyright file="ServiceRootSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// A segment representing the root of the service
    /// TODO: rename this class to 'BaseUriSegment' or something similar
    /// </summary>
    public class ServiceRootSegment : ODataUriSegment
    {
        /// <summary>
        /// Initializes a new instance of the ServiceRootSegment class
        /// </summary>
        /// <param name="rootUri">The service root uri</param>
        internal ServiceRootSegment(Uri rootUri)
            : base()
        {
            ExceptionUtilities.CheckArgumentNotNull(rootUri, "rootUri");
            this.Uri = rootUri;
        }

        /// <summary>
        /// Gets the service root uri
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// Gets the type of the segment
        /// </summary>
        public override ODataUriSegmentType SegmentType
        {
            get { return ODataUriSegmentType.ServiceRoot; }
        }

        /// <summary>
        /// Gets a value indicating whether or not this segment is preceded by a slash
        /// </summary>
        protected internal override bool HasPrecedingSlash
        {
            get { return false; }
        }
    }
}
