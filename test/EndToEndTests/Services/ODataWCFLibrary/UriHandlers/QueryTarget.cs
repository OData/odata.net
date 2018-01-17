//---------------------------------------------------------------------
// <copyright file="QueryTarget.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;

namespace Microsoft.Test.OData.Services.ODataWCFService.UriHandlers
{
    public class QueryTarget : PathSegmentHandler
    {
        bool resolved = false;

        List<ODataPathSegment> parentSegments = new List<ODataPathSegment>();
        List<ODataPathSegment> childSegments = new List<ODataPathSegment>();

        List<ODataPathSegment> canonicalSegments = new List<ODataPathSegment>();

        private QueryTarget()
        {
            this.IsRawValue = false;
            this.IsReference = false;
            this.IsOperation = false;
        }

        /// <summary>
        /// The type of target resource by given request.
        /// </summary>
        public IEdmType Type { get; private set; }

        /// <summary>
        /// The type of element in target resource by given request.
        /// </summary>
        public IEdmType ElementType { get; private set; }

        /// <summary>
        /// The type kind of target resource by given request.
        /// </summary>
        public EdmTypeKind TypeKind
        {
            get
            {
                if (this.Type == null)
                {
                    return EdmTypeKind.None;
                }
                else
                {
                    return this.Type.TypeKind;
                }
            }
        }

        /// <summary>
        /// The type kind of target resource by given request.
        /// </summary>
        public EdmTypeKind ElementTypeKind
        {
            get
            {
                if (this.ElementType == null)
                {
                    return EdmTypeKind.None;
                }
                else
                {
                    return this.ElementType.TypeKind;
                }
            }
        }

        /// <summary>
        /// The entity (set) resource targeted by this request. 
        /// </summary>
        public IEdmNavigationSource NavigationSource { get; private set; }

        /// <summary>
        /// The property resource targeted by this request.
        /// </summary>
        public IEdmStructuralProperty Property { get; private set; }

        /// <summary>
        /// Indicate whether target resource is a raw value.
        /// </summary>
        public bool IsRawValue { get; private set; }

        /// <summary>
        /// Indicate whether target resource is a $ref.
        /// </summary>
        public bool IsReference { get; private set; }

        /// <summary>
        /// Indicate whether target resource is returned by operation.
        /// </summary>
        public bool IsOperation { get; private set; }

        public bool IsSingleton
        {
            get
            {
                return this.NavigationSource != null && this.NavigationSource.NavigationSourceKind() == EdmNavigationSourceKind.Singleton;
            }
        }

        public bool IsEntitySet
        {
            get
            {
                return this.NavigationSource != null && this.TypeKind == EdmTypeKind.Collection;
            }
        }

        public static QueryTarget Resolve(ODataPath path)
        {
            QueryTarget target = new QueryTarget();
            path.WalkWith(target);
            target.resolved = true;

            return target;
        }

        public override void Handle(TypeSegment segment)
        {
            this.ThrowIfResolved();

            this.Type = segment.EdmType;
            this.ElementType = this.GetElementType(this.Type);

            this.childSegments.Add(segment);

            this.canonicalSegments.Add(segment);
        }

        public override void Handle(NavigationPropertySegment segment)
        {
            this.ThrowIfResolved();

            this.NavigationSource = segment.NavigationSource;
            this.Property = null;
            this.Type = segment.EdmType;
            this.ElementType = this.GetElementType(this.Type);

            this.PushParentSegment();
            this.childSegments.Add(segment);

            this.canonicalSegments.Add(segment);
        }

        public override void Handle(EntitySetSegment segment)
        {
            this.ThrowIfResolved();

            this.NavigationSource = segment.EntitySet;
            this.Property = null;
            this.Type = segment.EdmType;
            this.ElementType = this.GetElementType(this.Type);

            this.PushParentSegment();
            this.childSegments.Add(segment);

            if (segment.EntitySet is IEdmEntitySet)
            {
                this.canonicalSegments.Clear();
            }

            this.canonicalSegments.Add(segment);
        }

        public override void Handle(SingletonSegment segment)
        {
            this.ThrowIfResolved();

            this.NavigationSource = segment.Singleton;
            this.Property = null;
            this.Type = segment.EdmType;
            this.ElementType = this.GetElementType(this.Type);

            this.PushParentSegment();
            this.childSegments.Add(segment);

            this.canonicalSegments.Clear();
            this.canonicalSegments.Add(segment);
        }

        public override void Handle(KeySegment segment)
        {
            this.ThrowIfResolved();

            this.Type = segment.EdmType;
            this.ElementType = this.GetElementType(this.Type);

            this.PushParentSegment();
            this.childSegments.Add(segment);

            this.canonicalSegments.Add(segment);
        }

        public override void Handle(PropertySegment segment)
        {
            this.ThrowIfResolved();

            this.NavigationSource = null;
            this.Property = segment.Property;
            this.Type = segment.EdmType;
            this.ElementType = this.GetElementType(this.Type);

            this.PushParentSegment();
            this.childSegments.Add(segment);

            this.canonicalSegments.Add(segment);
        }

        public override void Handle(OperationImportSegment segment)
        {
            this.ThrowIfResolved();

            this.NavigationSource = segment.EntitySet;
            this.Property = null;
            this.Type = segment.EdmType;
            this.ElementType = this.GetElementType(this.Type);

            this.IsOperation = true;

            this.PushParentSegment();
            this.childSegments.Add(segment);

            this.canonicalSegments.Add(segment);
        }

        public override void Handle(OperationSegment segment)
        {
            this.ThrowIfResolved();

            this.NavigationSource = segment.EntitySet;
            this.Property = null;
            this.Type = segment.EdmType;
            this.ElementType = this.GetElementType(this.Type);

            this.IsOperation = true;

            this.PushParentSegment();
            this.childSegments.Add(segment);

            this.canonicalSegments.Add(segment);
        }

        public override void Handle(DynamicPathSegment segment)
        {
            this.ThrowIfResolved();

            this.NavigationSource = null;
            this.Property = new EdmOpenStructuralProperty(segment.Identifier);
            this.Type = segment.EdmType;
            this.ElementType = this.GetElementType(this.Type);

            this.PushParentSegment();
            this.childSegments.Add(segment);

            this.canonicalSegments.Add(segment);
        }

        public override void Handle(CountSegment segment)
        {
            this.ThrowIfResolved();

            this.NavigationSource = null;
            this.Property = null;
            this.Type = segment.EdmType;
            this.ElementType = this.GetElementType(this.Type);

            this.IsRawValue = true;

            this.childSegments.Add(segment);

            this.canonicalSegments.Add(segment);
        }

        public override void Handle(NavigationPropertyLinkSegment segment)
        {
            this.ThrowIfResolved();

            this.NavigationSource = segment.NavigationSource; ;
            this.Property = null;
            this.Type = segment.EdmType;
            this.ElementType = this.GetElementType(this.Type);

            this.IsReference = true;


            this.PushParentSegment();
            this.childSegments.Add(segment);

            this.canonicalSegments.Add(segment);
        }

        public override void Handle(ValueSegment segment)
        {
            this.ThrowIfResolved();

            this.NavigationSource = null;
            this.Type = segment.EdmType;
            this.ElementType = this.GetElementType(this.Type);

            this.IsRawValue = true;

            this.childSegments.Add(segment);

            this.canonicalSegments.Add(segment);
        }

        public override void Handle(BatchSegment segment)
        {
            this.ThrowIfResolved();

            this.NavigationSource = null;
            this.Property = null;
            this.Type = null;
            this.ElementType = this.GetElementType(this.Type);

            this.childSegments.Add(segment);

            this.canonicalSegments.Add(segment);
        }

        public override void Handle(BatchReferenceSegment segment)
        {
            this.ThrowIfResolved();

            throw Utility.BuildException(HttpStatusCode.NotImplemented, "BatchReferenceSegment is not supported.", null);
        }

        public override void Handle(MetadataSegment segment)
        {
            this.ThrowIfResolved();

            this.NavigationSource = null;
            this.Property = null;
            this.Type = null;
            this.ElementType = this.GetElementType(this.Type);

            this.childSegments.Add(segment);

            this.canonicalSegments.Add(segment);
        }

        public Uri BuildContainerUri(Uri rootUri)
        {
            ODataPath path = new ODataPath(this.parentSegments);
            return new Uri(rootUri, string.Concat(path.WalkWith(PathSegmentToResourcePathTranslator.Instance)).TrimStart('/'));
        }

        /// <summary>
        /// Build canonical URI base on current query target
        /// </summary>
        /// <param name="rootUri">The root URI</param>
        /// <param name="keySegment">The additional key segment of current target.</param>
        /// <returns>The canonical URI</returns>
        public Uri BuildCanonicalUri(Uri rootUri, KeySegment keySegment)
        {
            List<ODataPathSegment> segments = new List<ODataPathSegment>(this.canonicalSegments);

            if (keySegment != null && this.IsEntitySet)
            {
                if (segments.Last() is TypeSegment)
                {
                    segments.Insert(segments.Count - 1, keySegment);
                }
                else
                {
                    segments.Add(keySegment);
                }
            }

            ODataPath path = new ODataPath(segments);
            return new Uri(rootUri, string.Concat(path.WalkWith(PathSegmentToResourcePathTranslator.Instance)).TrimStart('/'));
        }

        private void ThrowIfResolved()
        {
            if (this.resolved)
            {
                throw new InvalidOperationException("QueryTarget should not be resolved twice.");
            }
        }

        private IEdmType GetElementType(IEdmType type)
        {
            if (type != null && type.TypeKind == EdmTypeKind.Collection)
            {
                return ((IEdmCollectionType)type).ElementType.Definition;
            }

            return null;
        }

        private void PushParentSegment()
        {
            foreach (ODataPathSegment segment in this.childSegments)
            {
                this.parentSegments.Add(segment);
            }

            this.childSegments.Clear();
        }

        class EdmOpenStructuralProperty : IEdmStructuralProperty
        {
            private string name;

            public EdmOpenStructuralProperty(string name)
            {
                this.name = name;
            }

            public string DefaultValueString
            {
                get { throw new NotImplementedException(); }
            }

            public EdmPropertyKind PropertyKind
            {
                get { throw new NotImplementedException(); }
            }

            public IEdmTypeReference Type
            {
                get { throw new NotImplementedException(); }
            }

            public IEdmStructuredType DeclaringType
            {
                get { throw new NotImplementedException(); }
            }

            public string Name
            {
                get { return this.name; }
            }
        }
    }
}
