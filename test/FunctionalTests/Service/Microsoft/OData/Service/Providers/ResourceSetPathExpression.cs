//---------------------------------------------------------------------
// <copyright file="ResourceSetPathExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    #endregion Namespaces.

    /// <summary>This class represents the path expression to calculate the target resource set of a Function or Action.</summary>
    [DebuggerDisplay("{PathExpression}")]
    public class ResourceSetPathExpression
    {
        /// <summary>
        /// The separator for the binding properties on the path.
        /// </summary>
        internal const char PathSeparator = '/';

        /// <summary>
        /// Path expression to calculate the target resource set of a function or action.
        /// </summary>
        private readonly string pathExpression;

        /// <summary>
        /// The binding parameter to a function or action.
        /// </summary>
        private OperationParameter bindingParameter;

        /// <summary>
        /// Resource properties and type segments on the path.
        /// </summary>
        private PathSegment[] pathSegments;

        /// <summary> Creates a new instance of the <see cref="T:Microsoft.OData.Service.Providers.ResourceSetPathExpression" /> class. </summary>
        /// <param name="pathExpression">Path expression to calculate the target resource set of a function or procedure.</param>
        /// <remarks>The <paramref name="pathExpression"/> must start with the binding parameter name followed by navigation properties that are separated by "/".
        /// For example, if the binding parameter is customer, a valid path can be "customer/Orders/OrderDetails".</remarks>
        public ResourceSetPathExpression(string pathExpression)
        {
            WebUtil.CheckStringArgumentNullOrEmpty(pathExpression, "pathExpression");
            this.pathExpression = pathExpression;
        }

        /// <summary> Path expression to calculate the target resource set of a function or procedure. </summary>
        /// <returns>The path expression.</returns>
        public string PathExpression
        {
            get { return this.pathExpression; }
        }

        /// <summary>
        /// Sets the binding resource type for the current path expression.
        /// </summary>
        /// <param name="parameter">The resource type this path expression will bind to.</param>
        internal void SetBindingParameter(OperationParameter parameter)
        {
            Debug.Assert(parameter != null, "parameter != null");
            Debug.Assert(
                parameter.ParameterType.ResourceTypeKind == ResourceTypeKind.EntityType || parameter.ParameterType.ResourceTypeKind == ResourceTypeKind.EntityCollection,
                "parameter.ParameterType.ResourceTypeKind == ResourceTypeKind.EntityType || parameter.ParameterType.ResourceTypeKind == ResourceTypeKind.EntityCollection");

            if (this.PathExpression != parameter.Name && !this.PathExpression.StartsWith(parameter.Name + ResourceSetPathExpression.PathSeparator, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(Strings.ResourceSetPathExpression_PathExpressionMustStartWithBindingParameterName(this.PathExpression, parameter.Name));
            }

            this.bindingParameter = parameter;
        }

        /// <summary>
        /// Sets the binding resource type for the current path expression.
        /// </summary>
        /// <param name="provider">Provider wrapper instance.</param>
        internal void InitializePathSegments(DataServiceProviderWrapper provider)
        {
            Debug.Assert(this.bindingParameter != null, "this.bindingResourceType != null");
            if (this.pathSegments == null)
            {
                string[] segmentStrings = this.pathExpression.Split(PathSeparator);
                Debug.Assert(segmentStrings[0] == this.bindingParameter.Name, "segmentStrings[0] == this.bindingParameter.Name");

                ResourceType bindingType = this.bindingParameter.ParameterType;
                if (bindingType.ResourceTypeKind == ResourceTypeKind.EntityCollection)
                {
                    bindingType = ((EntityCollectionResourceType)this.bindingParameter.ParameterType).ItemType;
                }

                List<PathSegment> segmentList = new List<PathSegment>();
                PathSegment currentSegment = new PathSegment { SourceType = bindingType };
                bool seenTypeSegment = false;
                int segmentLength = segmentStrings.Length;
                if (segmentLength == 1)
                {
                    segmentList.Add(currentSegment);
                }
                else
                {
                    for (int idx = 1; idx < segmentLength; idx++)
                    {
                        string segmentString = segmentStrings[idx];
                        if (string.IsNullOrEmpty(segmentString))
                        {
                            throw new InvalidOperationException(Strings.ResourceSetPathExpression_EmptySegment(this.pathExpression));
                        }

                        ResourceProperty currentProperty = currentSegment.SourceType.TryResolvePropertyName(segmentString);
                        if (currentProperty == null)
                        {
                            ResourceType currentType = WebUtil.ResolveTypeIdentifier(provider, segmentString, currentSegment.SourceType, previousSegmentIsTypeSegment: seenTypeSegment);
                            if (currentType != null)
                            {
                                currentSegment.SourceType = currentType;
                                seenTypeSegment = true;
                                if (idx == segmentLength - 1)
                                {
                                    throw new InvalidOperationException(Strings.ResourceSetPathExpression_PathCannotEndWithTypeIdentifier(this.pathExpression, segmentStrings[segmentLength - 1]));
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException(Strings.ResourceSetPathExpression_PropertyNotFound(this.pathExpression, segmentString, currentSegment.SourceType.FullName));
                            }
                        }
                        else
                        {
                            seenTypeSegment = false;
                            currentSegment.Property = currentProperty;
                            segmentList.Add(currentSegment);
                            currentSegment = new PathSegment { SourceType = currentProperty.ResourceType };
                        }

                        if (currentSegment.SourceType.ResourceTypeKind != ResourceTypeKind.EntityType)
                        {
                            throw new InvalidOperationException(Strings.ResourceSetPathExpression_PropertyMustBeEntityType(this.pathExpression, segmentString, currentSegment.SourceType.FullName));
                        }
                    }
                }

                this.pathSegments = segmentList.ToArray();
            }
        }

        /// <summary>
        /// Gets the target set from the binding set and path expression.
        /// </summary>
        /// <param name="provider">Provider instance to resolve the association set.</param>
        /// <param name="bindingSet">Binding resource set.</param>
        /// <returns>Returns the target resource set for the path expression.</returns>
        internal ResourceSetWrapper GetTargetSet(DataServiceProviderWrapper provider, ResourceSetWrapper bindingSet)
        {
            Debug.Assert(provider != null, "provider != null");
            Debug.Assert(bindingSet != null, "sourceSet != null");
            Debug.Assert(this.pathSegments != null && this.pathSegments.Length > 0, "InitializePathSegments() must be called before this.");
            Debug.Assert(bindingSet.ResourceType.IsAssignableFrom(this.pathSegments[0].SourceType), "bindingSet.ResourceType.IsAssignableFrom(this.pathSegments[0].SourceType)");

            ResourceSetWrapper resultSet = bindingSet;
            for (int idx = 0; resultSet != null && idx < this.pathSegments.Length; idx++)
            {
                PathSegment segment = this.pathSegments[idx];
                if (segment.Property != null)
                {
                    resultSet = provider.GetResourceSet(resultSet, segment.SourceType, segment.Property);
                }
#if DEBUG
                else
                {
                    Debug.Assert(idx == 0, "Path cannot end with type identifier.");
                }
#endif
            }

            return resultSet;
        }

        /// <summary>
        /// This struct holds the type and property info for a path segment.
        /// </summary>
        private struct PathSegment
        {
            /// <summary>The resource type where the property is defined.</summary>
            public ResourceType SourceType { get; set; }

            /// <summary>The resource property of the segment.</summary>
            public ResourceProperty Property { get; set; }
        }
    }
}
