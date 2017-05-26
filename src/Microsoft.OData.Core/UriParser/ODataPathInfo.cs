//---------------------------------------------------------------------
// <copyright file="ODataPathInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
    internal class ODataPathInfo
    {
        private readonly IEdmType targetEdmType;
        private readonly IEdmNavigationSource targetNavigationSource;
        private readonly IEnumerable<ODataPathSegment> segments;

        public ODataPathInfo(ODataPath odataPath)
        {
            ODataPathSegment lastSegment = odataPath.LastSegment;
            ODataPathSegment previous = null;
            var segs = odataPath.GetEnumerator();
            int count = 0;
            while (++count < odataPath.Count && segs.MoveNext())
            {
            }

            previous = segs.Current;
            if (lastSegment != null)
            {
                // use previous segment if the last one is Key or Count Segment
                if (lastSegment is KeySegment || lastSegment is CountSegment)
                {
                    lastSegment = previous;
                }

                this.targetNavigationSource = lastSegment.TargetEdmNavigationSource;
                this.targetEdmType = lastSegment.TargetEdmType;
                if (this.targetEdmType != null)
                {
                    IEdmCollectionType collectionType = this.targetEdmType as IEdmCollectionType;
                    if (collectionType != null)
                    {
                        this.targetEdmType = collectionType.ElementType.Definition;
                    }
                }
            }

            this.segments = odataPath;
        }

        public ODataPathInfo(IEdmType targetEdmType, IEdmNavigationSource targetNavigationSource)
        {
            this.targetEdmType = targetEdmType;
            this.targetNavigationSource = targetNavigationSource;
            this.segments = new List<ODataPathSegment>();
        }


        public IEdmType TargetEdmType
        {
            get { return targetEdmType; }
        }

        public IEdmNavigationSource TargetNavigationSource
        {
            get { return targetNavigationSource; }
        }

        public IEnumerable<ODataPathSegment> Segments
        {
            get { return segments; }
        }

        public IEdmStructuredType TargetStructuredType
        {
            get { return (IEdmStructuredType)targetEdmType; }
        }
    }
}
