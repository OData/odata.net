//---------------------------------------------------------------------
// <copyright file="SystemSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a system-reserved segment like $count or $ref.
    /// </summary>
    public class SystemSegment : ODataUriSegment
    {
        /// <summary>
        /// Static dictionary of all system segments by name
        /// </summary>
        private static Dictionary<string, SystemSegment> systemSegmentInstances =
            new Dictionary<string, SystemSegment>()
            {
                { Endpoints.Batch, new SystemSegment(ODataUriSegmentType.Batch, Endpoints.Batch) },
                { Endpoints.Count, new SystemSegment(ODataUriSegmentType.Count, Endpoints.Count) },
                { Endpoints.Ref, new SystemSegment(ODataUriSegmentType.EntityReferenceLinks, Endpoints.Ref) },
                { Endpoints.Metadata, new SystemSegment(ODataUriSegmentType.Metadata, Endpoints.Metadata) },
                { Endpoints.Value, new SystemSegment(ODataUriSegmentType.Value, Endpoints.Value) },
                { Endpoints.SelectAll, new SystemSegment(ODataUriSegmentType.SelectAll, Endpoints.SelectAll) }
            };

        /// <summary>
        /// Private storage for the segment type
        /// </summary>
        private ODataUriSegmentType segmentType;

        /// <summary>
        /// Initializes a new instance of the SystemSegment class. 
        /// Constructor is private to prevent duplicate instances.
        /// </summary>
        /// <param name="type">The type for the segment</param>
        /// <param name="endpoint">The endpoint for the segment</param>
        private SystemSegment(ODataUriSegmentType type, string endpoint)
        {
            this.segmentType = type;
            this.Endpoint = endpoint;
        }

        /// <summary>
        /// Gets a singleton $batch system segment
        /// </summary>
        public static SystemSegment Batch
        {
            get
            {
                return systemSegmentInstances[Endpoints.Batch];
            }
        }

        /// <summary>
        /// Gets a singleton $count system segment
        /// </summary>
        public static SystemSegment Count
        {
            get
            {
                return systemSegmentInstances[Endpoints.Count];
            }
        }

        /// <summary>
        /// Gets a singleton $ref system segment
        /// </summary>
        public static SystemSegment EntityReferenceLinks
        {
            get
            {
                return systemSegmentInstances[Endpoints.Ref];
            }
        }

        /// <summary>
        /// Gets a singleton $metadata system segment
        /// </summary>
        public static SystemSegment Metadata
        {
            get
            {
                return systemSegmentInstances[Endpoints.Metadata];
            }
        }

        /// <summary>
        /// Gets a singleton $value system segment
        /// </summary>
        public static SystemSegment Value
        {
            get
            {
                return systemSegmentInstances[Endpoints.Value];
            }
        }

        /// <summary>
        /// Gets a singleton '*' segment
        /// </summary>
        public static SystemSegment SelectAll
        {
            get
            {
                return systemSegmentInstances[Endpoints.SelectAll];
            }
        }

        /// <summary>
        /// Gets the type of the segment
        /// </summary>
        public override ODataUriSegmentType SegmentType
        {
            get { return this.segmentType; }
        }

        /// <summary>
        /// Gets the string endpoint for the segment
        /// </summary>
        public string Endpoint { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not this segment is preceded by a slash
        /// </summary>
        protected internal override bool HasPrecedingSlash
        {
            get { return true; }
        }

        /// <summary>
        /// Finds the system segment with the given endpoint, if one exists
        /// </summary>
        /// <param name="endpoint">The endpoint name</param>
        /// <param name="segment">The segment, if one exists</param>
        /// <returns>True if a segment is found, otherwise false</returns>
        public static bool TryGet(string endpoint, out SystemSegment segment)
        {
            return systemSegmentInstances.TryGetValue(endpoint, out segment);
        }
    }
}
