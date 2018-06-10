//---------------------------------------------------------------------
// <copyright file="ODataPathExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics;

namespace Microsoft.OData.UriParser
{
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;

    /// <summary>
    /// Extension methods for <see cref="ODataPath"/>. These method provide convenince functions.
    /// TODO: Implement this class and it's visitors. These are stubs.
    /// </summary>
    /// <remarks>
    /// The values that these methods compute are not cached.
    /// </remarks>
    internal static class ODataPathExtensions
    {
        /// <summary>
        /// Computes the <see cref="IEdmTypeReference"/> of the resource identified by this <see cref="ODataPath"/>.
        /// </summary>
        /// <param name="path">Path to compute the type for.</param>
        /// <returns>The <see cref="IEdmTypeReference"/> of the resource, or null if the path does not identify a
        /// resource with a type.</returns>
        public static IEdmTypeReference EdmType(this ODataPath path)
        {
            return path.LastSegment.EdmType.ToTypeReference();
        }

        /// <summary>
        /// Computes the <see cref="IEdmNavigationSource"/> of the resource identified by this <see cref="ODataPath"/>.
        /// </summary>
        /// <param name="path">Path to compute the set for.</param>
        /// <returns>The <see cref="IEdmNavigationSource"/> of the resource, or null if the path does not identify a
        /// resource that is part of a set.</returns>
        public static IEdmNavigationSource NavigationSource(this ODataPath path)
        {
            return path.LastSegment.TranslateWith(new DetermineNavigationSourceTranslator());
        }

        /// <summary>
        /// Computes whether or not the resource identified by this <see cref="ODataPath"/> is a collection.
        /// </summary>
        /// <param name="path">Path to perform the computation on.</param>
        /// <returns>True if the resource if a resource set or collection of primitive or complex types. False otherwise.</returns>
        public static bool IsCollection(this ODataPath path)
        {
            return path.LastSegment.TranslateWith(new IsCollectionTranslator());
        }

        /// <summary>
        /// Build a segment representing a navigation property.
        /// </summary>
        /// <param name="path">Path to perform the computation on.</param>
        /// <param name="navigationProperty">The navigation property this segment represents.</param>
        /// <param name="navigationSource">The navigation source of the entities targeted by this navigation property. This can be null.</param>
        /// <returns>The ODataPath with navigation property segment appended in the end.</returns>
        public static ODataPath AppendNavigationPropertySegment(this ODataPath path, IEdmNavigationProperty navigationProperty, IEdmNavigationSource navigationSource)
        {
            var newPath = new ODataPath(path);
            NavigationPropertySegment np = new NavigationPropertySegment(navigationProperty, navigationSource);
            newPath.Add(np);
            return newPath;
        }

        /// <summary>
        /// Build a segment representing a property.
        /// </summary>
        /// <param name="path">Path to perform the computation on.</param>
        /// <param name="property">The property this segment represents.</param>
        /// <returns>>The ODataPath with property segment appended in the end.</returns>
        public static ODataPath AppendPropertySegment(this ODataPath path, IEdmStructuralProperty property)
        {
            var newPath = new ODataPath(path);
            PropertySegment propertySegment = new PropertySegment(property);
            newPath.Add(propertySegment);
            return newPath;
        }

        /// <summary>
        /// Append the key segment in the end of ODataPath, the method does not modify current ODataPath instance,
        /// it returns a new ODataPath without ending type segment.
        /// If last segment is type cast, the key would be appended before type cast segment.
        /// </summary>
        /// <param name="path">Path to perform the computation on.</param>
        /// <param name="keys">The set of key property names and the values to be used in searching for the given item.</param>
        /// <param name="edmType">The type of the item this key returns.</param>
        /// <param name="navigationSource">The navigation source that this key is used to search.</param>
        /// <returns>The ODataPath with key segment appended</returns>
        public static ODataPath AppendKeySegment(this ODataPath path, IEnumerable<KeyValuePair<string, object>> keys, IEdmEntityType edmType, IEdmNavigationSource navigationSource)
        {
            var handler = new SplitEndingSegmentOfTypeHandler<TypeSegment>();
            path.WalkWith(handler);
            KeySegment keySegment = new KeySegment(keys, edmType, navigationSource);
            ODataPath newPath = handler.FirstPart;
            newPath.Add(keySegment);
            foreach (var segment in handler.LastPart)
            {
                newPath.Add(segment);
            }

            return newPath;
        }

        /// <summary>
        /// Remove the key segment in the end of ODataPath, the method does not modify current ODataPath instance,
        /// it returns a new ODataPath without ending type segment.
        /// If last segment is type cast, the key before type cast segment would be removed.
        /// </summary>
        /// <param name="path">Path to perform the computation on.</param>
        /// <returns>The ODataPath without key segment removed</returns>
        public static ODataPath TrimEndingKeySegment(this ODataPath path)
        {
            var typeHandler = new SplitEndingSegmentOfTypeHandler<TypeSegment>();
            var keyHandler = new SplitEndingSegmentOfTypeHandler<KeySegment>();
            path.WalkWith(typeHandler);
            typeHandler.FirstPart.WalkWith(keyHandler);
            ODataPath newPath = keyHandler.FirstPart;
            foreach (var segment in typeHandler.LastPart)
            {
                newPath.Add(segment);
            }

            return newPath;
        }

        /// <summary>
        /// Remove the type-cast segment in the end of ODataPath, the method does not modify current ODataPath instance,
        /// it returns a new ODataPath without ending type segment.
        /// </summary>
        /// <param name="path">Path to perform the computation on.</param>
        /// <returns>The ODataPath without type-cast in the end</returns>
        public static ODataPath TrimEndingTypeSegment(this ODataPath path)
        {
            var handler = new SplitEndingSegmentOfTypeHandler<TypeSegment>();
            path.WalkWith(handler);
            return handler.FirstPart;
        }

        /// <summary>
        /// Computes whether or not the ODataPath targets at an individual property.
        /// </summary>
        /// <param name="path">Path to perform the computation on.</param>
        /// <returns>True if the the ODataPath targets at an individual property. False otherwise.</returns>
        public static bool IsIndividualProperty(this ODataPath path)
        {
            ODataPathSegment lastNonTypeCastSegment = path.TrimEndingTypeSegment().LastSegment;
            return lastNonTypeCastSegment is PropertySegment || lastNonTypeCastSegment is DynamicPathSegment;
        }

        /// <summary>
        /// Computes whether or not the ODataPath targets at an unknown segment.
        /// </summary>
        /// <param name="path">Path to perform the computation on.</param>
        /// <returns>True if the the ODataPath targets at an unknown segment. False otherwise.</returns>
        public static bool IsUndeclared(this ODataPath path)
        {
            ODataPathSegment lastNonTypeCastSegment = path.TrimEndingTypeSegment().LastSegment;
            return lastNonTypeCastSegment is DynamicPathSegment;
        }

        /// <summary>
        /// Get the string representation of <see cref="ODataPath"/>.
        /// mainly translate Context Url path.
        /// </summary>
        /// <param name="path">Path to perform the computation on.</param>
        /// <returns>The string representation of the Context Url path.</returns>
        public static string ToContextUrlPathString(this ODataPath path)
        {
            StringBuilder pathString = new StringBuilder();
            PathSegmentToContextUrlPathTranslator pathTranslator = PathSegmentToContextUrlPathTranslator.DefaultInstance;
            ODataPathSegment priorSegment = null;
            bool foundOperationWithoutPath = false;
            foreach (ODataPathSegment segment in path)
            {
                OperationSegment operationSegment = segment as OperationSegment;
                OperationImportSegment operationImportSegment = segment as OperationImportSegment;
                if (operationImportSegment != null)
                {
                    IEdmOperationImport operationImport = operationImportSegment.OperationImports.FirstOrDefault();
                    Debug.Assert(operationImport != null);

                    EdmPathExpression pathExpression = operationImport.EntitySet as EdmPathExpression;
                    if (pathExpression != null)
                    {
                        Debug.Assert(priorSegment == null); // operation import is always the first segment?
                        pathString.Append(pathExpression.Path);
                    }
                    else
                    {
                        pathString = operationImport.Operation.ReturnType != null ?
                                new StringBuilder(operationImport.Operation.ReturnType.FullName()) :
                                new StringBuilder("Edm.Untyped");

                        foundOperationWithoutPath = true;
                    }
                }
                else if (operationSegment != null)
                {
                    IEdmOperation operation = operationSegment.Operations.FirstOrDefault();
                    Debug.Assert(operation != null);

                    if (operation.IsBound &&
                        priorSegment != null &&
                        operation.Parameters.First().Type.Definition == priorSegment.EdmType)
                    {
                        if (operation.EntitySetPath != null)
                        {
                            foreach (string pathSegment in operation.EntitySetPath.PathSegments.Skip(1))
                            {
                                pathString.Append('/');
                                pathString.Append(pathSegment);
                            }
                        }
                        else if (operationSegment.EntitySet != null)
                        {
                            // Is it correct to check EntitySet?
                            pathString = new StringBuilder(operationSegment.EntitySet.Name);
                        }
                        else
                        {
                            pathString = operation.ReturnType != null ?
                                new StringBuilder(operation.ReturnType.FullName()) :
                                new StringBuilder("Edm.Untyped");

                            foundOperationWithoutPath = true;
                        }
                    }
                }
                else
                {
                    if (foundOperationWithoutPath)
                    {
                        pathString = new StringBuilder(segment.EdmType.FullTypeName());
                        foundOperationWithoutPath = false;
                    }
                    else
                    {
                        pathString.Append(segment.TranslateWith(pathTranslator));
                    }
                }

                priorSegment = segment;
            }

            return pathString.ToString().TrimStart('/');
        }

        /// <summary>
        /// Get the string representation of <see cref="ODataPath"/>.
        /// mainly translate Query Url path.
        /// </summary>
        /// <param name="path">Path to perform the computation on.</param>
        /// <param name="urlKeyDelimiter">Mark whether key is segment</param>
        /// <returns>The string representation of the Query Url path.</returns>
        public static string ToResourcePathString(this ODataPath path, ODataUrlKeyDelimiter urlKeyDelimiter)
        {
            return string.Concat(path.WalkWith(new PathSegmentToResourcePathTranslator(urlKeyDelimiter)).ToArray()).TrimStart('/');
        }

        /// <summary>
        /// Translate an ODataExpandPath into an ODataSelectPath
        /// Depending on the constructor of ODataSelectPath to validate that we aren't adding any
        /// segments that are illegal for a select.
        /// </summary>
        /// <param name="path">the ODataExpandPath to translate</param>
        /// <returns>A new ODataSelect path with the same segments as the expand path.</returns>
        public static ODataSelectPath ToSelectPath(this ODataExpandPath path)
        {
            return new ODataSelectPath(path);
        }

        /// <summary>
        /// Translate an ODataSelectPath into an ODataExpandPath
        /// Depending on the constructor of ODataExpandPath to validate that we aren't adding any
        /// segments that are illegal for an expand.
        /// </summary>
        /// <param name="path">the ODataSelectPath to translate</param>
        /// <returns>A new ODataExpand path with the same segments as the select path.</returns>
        public static ODataExpandPath ToExpandPath(this ODataSelectPath path)
        {
            return new ODataExpandPath(path);
        }

        /// <summary>
        /// Gets the target navigation source to the ODataPath.
        /// </summary>
        /// <param name="path">Path to compute the set for.</param>
        /// <returns>The target navigation source to the ODataPath.</returns>
        internal static IEdmNavigationSource TargetNavigationSource(this ODataPath path)
        {
            if (path == null)
            {
                return null;
            }

            return new ODataPathInfo(path).TargetNavigationSource;
        }
    }
}
