//---------------------------------------------------------------------
// <copyright file="ODataUri.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Augments a standard URI with OData-protocol-level metadata
    /// </summary>
    public class ODataUri
    {
        /// <summary>
        /// Initializes a new instance of the ODataUri class
        /// </summary>
        public ODataUri()
            : this(Enumerable.Empty<ODataUriSegment>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the ODataUri class
        /// </summary>
        /// <param name="segments">The initial segments for the uri</param>
        public ODataUri(params ODataUriSegment[] segments)
            : this((IEnumerable<ODataUriSegment>)segments)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ODataUri class
        /// </summary>
        /// <param name="segments">The initial segments for the uri</param>
        public ODataUri(IEnumerable<ODataUriSegment> segments)
        {
            ExceptionUtilities.CheckArgumentNotNull(segments, "segments");
            this.Segments = new List<ODataUriSegment>(segments);
            this.CustomQueryOptions = new Dictionary<string, string>();
            this.ExpandSegments = new ODataUriSegmentPathCollection();
            this.SelectSegments = new ODataUriSegmentPathCollection();
        }

        /// <summary>
        /// Gets or sets the value of the '$inlinecount' query option. Null value indicates the option is ommitted entirely.
        /// </summary>
        public string InlineCount { get; set; }
        
        /// <summary>
        /// Gets or sets the value of the '$skiptoken' query option. Null value indicates the option is ommitted entirely.
        /// </summary>
        public string SkipToken { get; set; }
        
        /// <summary>
        /// Gets or sets the value of the '$top' query option. Null value indicates the option is ommitted entirely.
        /// </summary>
        public int? Top { get; set; }

        /// <summary>
        /// Gets or sets the value of the '$skip' query option. Null value indicates the option is ommitted entirely.
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        /// Gets or sets the value of the '$orderby' query option. Null value indicates the option is ommitted entirely.
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// Gets or sets the value of the '$filter' query option. Null value indicates the option is ommitted entirely.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets the value of the '$format' query option. Null value indicates the option is ommitted entirely.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Gets the collection of expanded paths for the '$expand' query option. An empty collection indicates the option is ommitted entirely.
        /// </summary>
        public ODataUriSegmentPathCollection ExpandSegments { get; internal set; }

        /// <summary>
        /// Gets the collection of expanded paths for the '$select' query option. An empty collection indicates the option is ommitted entirely.
        /// </summary>
        public ODataUriSegmentPathCollection SelectSegments { get; internal set; }

        /// <summary>
        /// Gets the set of custom query options to add to the uri
        /// </summary>
        public IDictionary<string, string> CustomQueryOptions { get; internal set; }

        /// <summary>
        /// Gets the list of uri segments
        /// </summary>
        public IList<ODataUriSegment> Segments { get; internal set; }

        /// <summary>
        /// Gets the first segment of the uri or null if it has no segments
        /// </summary>
        public ODataUriSegment RootSegment
        {
            get
            {
                if (this.Segments.Count < 1)
                {
                    return null;
                }

                return this.Segments[0];
            }
        }

        /// <summary>
        /// Gets the last segment of the uri or null if it has no segments
        /// </summary>
        public ODataUriSegment LastSegment
        {
            get
            {
                if (this.Segments.Count < 1)
                {
                    return null;
                }

                return this.Segments[this.Segments.Count - 1];
            }
        }
    }
}
