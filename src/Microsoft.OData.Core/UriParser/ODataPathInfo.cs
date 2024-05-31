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
            ODataPathSegment targetSegment = odataPath.LastSegment;

            if (targetSegment != null)
            {
                // use next to last segment if the last one is Key or Count Segment
                if (targetSegment is KeySegment || targetSegment is CountSegment)
                {
                    if (odataPath.Count > 1)
                    {
                        targetSegment = odataPath.Segments[odataPath.Count - 2];
                    }
                }

                this.targetNavigationSource = targetSegment.TargetEdmNavigationSource;
                this.targetEdmType = targetSegment.EdmType;
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
