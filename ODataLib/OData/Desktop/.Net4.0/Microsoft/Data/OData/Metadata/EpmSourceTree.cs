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

namespace Microsoft.Data.OData.Metadata
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Common;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// Tree representing the sourceName properties in all the EntityPropertyMappingAttributes for a type.
    /// </summary>
    internal sealed class EpmSourceTree
    {
        #region Fields
        /// <summary>
        /// Root of the tree.
        /// </summary>
        private readonly EpmSourcePathSegment root;
        
        /// <summary>
        /// <see cref="EpmTargetTree"/> corresponding to this tree.
        /// </summary>
        private readonly EpmTargetTree epmTargetTree;
        #endregion

        /// <summary>
        /// Constructor which creates an empty root.
        /// </summary>
        /// <param name="epmTargetTree">Target xml tree</param>
        internal EpmSourceTree(EpmTargetTree epmTargetTree)
        {
            DebugUtils.CheckNoExternalCallers();

            this.root = new EpmSourcePathSegment();
            this.epmTargetTree = epmTargetTree;
        }

        #region Properties
        /// <summary>
        /// Root of the tree
        /// </summary>
        internal EpmSourcePathSegment Root
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.root;
            }
        }
        #endregion

        /// <summary>
        /// Adds a path to the source and target tree which is obtained by looking at the EntityPropertyMappingAttribute in the <paramref name="epmInfo"/>
        /// </summary>
        /// <param name="epmInfo">EnitityPropertyMappingInfo holding the source path</param>
        internal void Add(EntityPropertyMappingInfo epmInfo)
        {
            DebugUtils.CheckNoExternalCallers();

            List<EpmSourcePathSegment> pathToCurrentSegment = new List<EpmSourcePathSegment>();
            EpmSourcePathSegment currentSourceSegment = this.Root;
            EpmSourcePathSegment foundSourceSegment = null;
            IEdmType currentType = epmInfo.ActualPropertyType;

            Debug.Assert(!string.IsNullOrEmpty(epmInfo.Attribute.SourcePath), "Invalid source path");
            string[] propertyPath = epmInfo.Attribute.SourcePath.Split('/');

            int propertyPathLength = propertyPath.Length;
            Debug.Assert(propertyPathLength > 0, "Must have been validated during EntityPropertyMappingAttribute construction");
            for (int sourcePropertyIndex = 0; sourcePropertyIndex < propertyPathLength; sourcePropertyIndex++)
            {
                string propertyName = propertyPath[sourcePropertyIndex];

                if (propertyName.Length == 0)
                {
                    throw new ODataException(ODataErrorStrings.EpmSourceTree_InvalidSourcePath(epmInfo.DefiningType.ODataFullName(), epmInfo.Attribute.SourcePath));
                }

                IEdmTypeReference nextPropertyTypeReference = GetPropertyType(currentType, propertyName);
                IEdmType nextPropertyType = nextPropertyTypeReference == null ? null : nextPropertyTypeReference.Definition;

                // If we don't find a property type this is an open property; check whether this is the last segment in the path
                // since otherwise this would not be an open primitive property and only open primitive properties are allowed.
                if (nextPropertyType == null && sourcePropertyIndex < propertyPathLength - 1)
                {
                    throw new ODataException(ODataErrorStrings.EpmSourceTree_OpenComplexPropertyCannotBeMapped(propertyName, currentType.ODataFullName()));
                }

                currentType = nextPropertyType;
                foundSourceSegment = currentSourceSegment.SubProperties.SingleOrDefault(e => e.PropertyName == propertyName);
                if (foundSourceSegment != null)
                {
                    currentSourceSegment = foundSourceSegment;
                }
                else
                {
                    EpmSourcePathSegment newSourceSegment = new EpmSourcePathSegment(propertyName);
                    currentSourceSegment.SubProperties.Add(newSourceSegment);
                    currentSourceSegment = newSourceSegment;
                }

                pathToCurrentSegment.Add(currentSourceSegment);
            }

            // The last segment is the one being mapped from by the user specified attribute.
            // It must be a primitive type - we don't allow mappings of anything else than primitive properties directly.
            // Note that we can only verify this for non-open properties, for open properties we must assume it's a primitive type
            // and we will make this check later during serialization again when we actually have the value of the property.
            if (currentType != null)
            {
                if (!currentType.IsODataPrimitiveTypeKind())
                {
                    throw new ODataException(ODataErrorStrings.EpmSourceTree_EndsWithNonPrimitiveType(currentSourceSegment.PropertyName));
                }
            }

            Debug.Assert(foundSourceSegment == null || foundSourceSegment.EpmInfo != null, "Can't have a leaf node in the tree without EpmInfo.");

            // Two EpmAttributes with same PropertyName in the same type, this could be a result of inheritance
            if (foundSourceSegment != null)
            {
                Debug.Assert(object.ReferenceEquals(foundSourceSegment, currentSourceSegment), "currentSourceSegment variable should have been updated already to foundSourceSegment");

                // Check for duplicates on the same entity type
                Debug.Assert(foundSourceSegment.SubProperties.Count == 0, "If non-leaf, it means we allowed complex type to be a leaf node");
                if (foundSourceSegment.EpmInfo.DefiningTypesAreEqual(epmInfo))
                {
                    throw new ODataException(ODataErrorStrings.EpmSourceTree_DuplicateEpmAttributesWithSameSourceName(epmInfo.DefiningType.ODataFullName(), epmInfo.Attribute.SourcePath));
                }

                // In case of inheritance, we need to remove the node from target tree which was mapped to base type property
                this.epmTargetTree.Remove(foundSourceSegment.EpmInfo);
            }

            epmInfo.SetPropertyValuePath(pathToCurrentSegment.ToArray());
            currentSourceSegment.EpmInfo = epmInfo;

            this.epmTargetTree.Add(epmInfo);
        }

        /// <summary>
        /// Validates the source tree.
        /// </summary>
        /// <param name="entityType">The entity type for which the validation is performed.</param>
        internal void Validate(IEdmEntityType entityType)
        {
            DebugUtils.CheckNoExternalCallers();
            Validate(this.Root, entityType);
        }

        /// <summary>
        /// Validates the specified segment and all its subsegments.
        /// </summary>
        /// <param name="pathSegment">The path segment to validate.</param>
        /// <param name="type">The type of the property represented by this segment (null for open properties).</param>
        private static void Validate(EpmSourcePathSegment pathSegment, IEdmType type)
        {
            Debug.Assert(pathSegment != null, "pathSegment != null");

            foreach (EpmSourcePathSegment subSegment in pathSegment.SubProperties)
            {
                IEdmTypeReference subSegmentTypeReference = GetPropertyType(type, subSegment.PropertyName);
                IEdmType subSegmentType = subSegmentTypeReference == null ? null : subSegmentTypeReference.Definition;

                Validate(subSegment, subSegmentType);
            }
        }

        /// <summary>
        /// Returns the type of the property on the specified type.
        /// </summary>
        /// <param name="type">The type to look for the property on.</param>
        /// <param name="propertyName">The name of the property to look for.</param>
        /// <returns>The type of the property specified.</returns>
        private static IEdmTypeReference GetPropertyType(IEdmType type, string propertyName)
        {
            Debug.Assert(propertyName != null, "propertyName != null");

            IEdmStructuredType structuredType = type as IEdmStructuredType;
            IEdmProperty property = structuredType == null ? null : structuredType.FindProperty(propertyName);

            if (property != null)
            {
                IEdmTypeReference propertyType = property.Type;
                if (propertyType.IsNonEntityCollectionType())
                {
                    throw new ODataException(ODataErrorStrings.EpmSourceTree_CollectionPropertyCannotBeMapped(propertyName, type.ODataFullName()));
                }
                
                if (propertyType.IsStream())
                {
                    throw new ODataException(ODataErrorStrings.EpmSourceTree_StreamPropertyCannotBeMapped(propertyName, type.ODataFullName()));
                }
                
                if (propertyType.IsSpatial())
                {
                    throw new ODataException(ODataErrorStrings.EpmSourceTree_SpatialTypeCannotBeMapped(propertyName, type.ODataFullName()));
                }
                
                return property.Type;
            }

            if (type != null && !type.IsOpenType())
            {
                throw new ODataException(ODataErrorStrings.EpmSourceTree_MissingPropertyOnType(propertyName, type.ODataFullName()));
            }

            return null;
        }
    }
}
