//---------------------------------------------------------------------
// <copyright file="ODataAnnotationUriBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Evaluation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Uri builder for OData annotations such as odata.id and so on.
    /// </summary>
    internal sealed class ODataAnnotationUriBuilder
    {
        /// <summary>
        /// The mapping from Edm primitive type to Clr primitive type.
        /// These primitive types can be used as key of entity type.
        /// </summary>
        private static readonly IDictionary<EdmPrimitiveTypeKind, Type> EdmToClrPrimitiveTypeMapping = new Dictionary<EdmPrimitiveTypeKind, Type>
        {
            { EdmPrimitiveTypeKind.None, null },
            { EdmPrimitiveTypeKind.Boolean, typeof(bool) },
            { EdmPrimitiveTypeKind.Byte, typeof(byte) },
            { EdmPrimitiveTypeKind.DateTimeOffset, typeof(DateTimeOffset) },
            { EdmPrimitiveTypeKind.Decimal, typeof(Decimal) },
            { EdmPrimitiveTypeKind.Duration, typeof(TimeSpan) },
            { EdmPrimitiveTypeKind.Guid, typeof(Guid) },
            { EdmPrimitiveTypeKind.Int16, typeof(Int16) },
            { EdmPrimitiveTypeKind.Int32, typeof(Int32) },
            { EdmPrimitiveTypeKind.Int64, typeof(Int64) },
            { EdmPrimitiveTypeKind.SByte, typeof(SByte) },
            { EdmPrimitiveTypeKind.String, typeof(String) },
        };

        /// <summary>
        /// The entity set name.
        /// </summary>
        private string entitySetName;

        /// <summary>
        /// The full entity type name.
        /// </summary>
        private string entityTypeName;

        /// <summary>
        /// The key properties.
        /// </summary>
        private IDictionary<string, object> keyProperties;

        /// <summary>
        /// Path segments in the OData path.
        /// </summary>
        private IList<ODataPathSegment> segments;

        /// <summary>
        /// Always the last segment of the OData path.
        /// </summary>
        private ODataPathSegment lastSegment;

        /// <summary>
        /// The computed uri.
        /// </summary>
        private Uri computedUri;

        /// <summary>
        /// Constructor of the uri builder.
        /// Each service would need to create only one instance of this class.
        /// </summary>
        /// <param name="serviceRoot">The base uri of the OData service.</param>
        internal ODataAnnotationUriBuilder(Uri serviceRoot)
        {
            this.ServiceRoot = serviceRoot;
        }

        /// <summary>
        /// The base uri of the OData service.
        /// </summary>
        internal Uri ServiceRoot { get; private set; }

        /// <summary>
        /// Compute id from the given OData path.
        /// </summary>
        /// <param name="path">The request uri.</param>
        /// <param name="keyProperties">The key properties.</param>
        /// <returns>The computed id</returns>
        internal Uri ComputeId(ODataPath path, IDictionary<string, object> keyProperties)
        {
            this.segments = path.ToList();
            this.lastSegment = segments.Last();

            var navigationSource = this.ComputeNavigationSource();
            this.entitySetName = navigationSource.Name;
            this.entityTypeName = navigationSource.EntityType().FullTypeName();

            this.computedUri = this.ServiceRoot;

            switch (navigationSource.NavigationSourceKind())
            {
                case EdmNavigationSourceKind.EntitySet:
                    this.keyProperties = ValidateKeyPropertiesAgainstEntityType(keyProperties, navigationSource.EntityType());
                    return this.keyProperties != null ? this.ComputeIdForEntitySet() : null;

                case EdmNavigationSourceKind.ContainedEntitySet:
                    this.keyProperties = ValidateKeyPropertiesAgainstEntityType(keyProperties, navigationSource.EntityType());
                    return this.keyProperties != null ? this.ComputeIdForContainedEntitySet() : null;

                case EdmNavigationSourceKind.Singleton:
                    return this.ComputeIdForSingleton();

                default:
                    return null;
            }
        }

        /// <summary>
        /// Validate if the key properties provided are consistent with the entity type given.
        /// </summary>
        /// <param name="keyProperties">The key properties to validate.</param>
        /// <param name="entityType">The entity type to compare against.</param>
        /// <returns>The original key properties if validation succeded or null if failed.</returns>
        internal static IDictionary<string, object> ValidateKeyPropertiesAgainstEntityType(IDictionary<string, object> keyProperties, IEdmEntityType entityType)
        {
            int declaredKeyCount = 0;
            foreach (var edmKeyProperty in entityType.DeclaredKey)
            {
                Type expectedClrType;
                if (!EdmToClrPrimitiveTypeMapping.TryGetValue(edmKeyProperty.Type.PrimitiveKind(), out expectedClrType))
                {
                    return null;
                }

                object keyValue;
                if (!keyProperties.TryGetValue(edmKeyProperty.Name, out keyValue))
                {
                    return null;
                }

                if (keyValue == null || keyValue.GetType() != expectedClrType)
                {
                    return null;
                }

                ++declaredKeyCount;
            }

            if (declaredKeyCount != keyProperties.Count)
            {
                return null;
            }

            return keyProperties;
        }

        /// <summary>
        /// Compute the navigation source from the segments.
        /// </summary>
        /// <returns>The computed navigation source.</returns>
        private IEdmNavigationSource ComputeNavigationSource()
        {
            // Find the last segment which can be used to determine the navigation source kind.
            while (!(lastSegment is NavigationPropertySegment || lastSegment is EntitySetSegment || lastSegment is SingletonSegment))
            {
                segments.Remove(lastSegment);
                lastSegment = segments.Last();
            }

            if (lastSegment is NavigationPropertySegment)
            {
                return ((NavigationPropertySegment)lastSegment).NavigationSource;
            }

            if (lastSegment is EntitySetSegment)
            {
                return ((EntitySetSegment)lastSegment).EntitySet;
            }

            if (lastSegment is SingletonSegment)
            {
                return ((SingletonSegment)lastSegment).Singleton;
            }

            return null;
        }

        /// <summary>
        /// Compute id for entity set.
        /// </summary>
        /// <returns>The computed id.</returns>
        private Uri ComputeIdForEntitySet()
        {
            this.computedUri = AppendSegment(this.computedUri, entitySetName, true);
            this.computedUri = AppendKeyExpression(this.computedUri, this.keyProperties);

            return this.computedUri;
        }

        /// <summary>
        /// Compute id for contained entity set.
        /// </summary>
        /// <returns>The computed id.</returns>
        private Uri ComputeIdForContainedEntitySet()
        {
            this.RemoveRedundantEntitySetPath();

            // Find the last navigation property segment.
            while (!(this.lastSegment is NavigationPropertySegment))
            {
                RemoveLastSegment();
            }

            this.RemoveRedundantTypeCasts();

            // Also remove the last navigation property segment.
            RemoveLastSegment();

            // Append each segment to base uri
            foreach (var segment in segments)
            {
                if (segment is KeySegment)
                {
                    this.computedUri = AppendKeyExpression(this.computedUri, ((KeySegment)segment).Keys);
                }
                else if (segment is EntitySetSegment)
                {
                    this.computedUri = AppendSegment(this.computedUri, ((EntitySetSegment)segment).EntitySet.Name, true);
                }
                else if (segment is TypeSegment)
                {
                    this.computedUri = AppendSegment(this.computedUri, segment.EdmType.FullTypeName(), true);
                }
                else
                {
                    this.computedUri = AppendSegment(this.computedUri, ((NavigationPropertySegment)segment).NavigationProperty.Name, true);
                }
            }

            // Add path segment for the containment navigation property
            this.computedUri = AppendSegment(this.computedUri, entitySetName, true);
            this.computedUri = AppendKeyExpression(this.computedUri, this.keyProperties);

            return this.computedUri;
        }

        /// <summary>
        /// Compute id for singleton.
        /// </summary>
        /// <returns>The computed id.</returns>
        private Uri ComputeIdForSingleton()
        {
            this.computedUri = AppendSegment(this.computedUri, entitySetName, true);

            return this.computedUri;
        }

        /// <summary>
        /// Appends a segment to the specified base URI.
        /// </summary>
        /// <param name="baseUri">The base Uri to append the segment to.</param>
        /// <param name="segment">The segment to append.</param>
        /// <param name="escapeSegment">True if the new segment should be escaped, otherwise False.</param>
        /// <returns>New URI with the appended segment and no trailing slash added.</returns>
        private static Uri AppendSegment(Uri baseUri, string segment, bool escapeSegment)
        {
            string baseUriString = baseUri.AbsoluteUri;

            if (escapeSegment)
            {
                segment = Uri.EscapeDataString(segment);
            }

            if (baseUriString[baseUriString.Length - 1] != '/')
            {
                return new Uri(string.Format("{0}/{1}", baseUriString, segment), UriKind.Absolute);
            }

            return new Uri(baseUri, segment);
        }

        /// <summary>
        /// Appends a key expression to the specified base URI.
        /// </summary>
        /// <param name="baseUri">The base Uri to append the key expression to.</param>
        /// <param name="keyProperties">The key properties to append.</param>
        /// <returns>New URI with the appended key expression.</returns>
        private static Uri AppendKeyExpression(Uri baseUri, IEnumerable<KeyValuePair<string, object>> keyProperties)
        {
            StringBuilder builder = new StringBuilder(baseUri.AbsoluteUri);

            builder.Append('(');

            var first = true;
            foreach (var property in keyProperties)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder.Append(',');
                }

                if (keyProperties.Count() != 1)
                {
                    builder.Append(property.Key);
                    builder.Append('=');
                }

                builder.Append(property.Value);
            }

            builder.Append(')');

            return new Uri(builder.ToString(), UriKind.Absolute);
        }

        /// <summary>
        /// Remove redundant type casts in segments.
        /// </summary>
        private void RemoveRedundantTypeCasts()
        {
            List<ODataPathSegment> segmentsToRemove = new List<ODataPathSegment>();

            foreach (var segment in this.segments)
            {
                NavigationPropertySegment navigationPropertySegment = segment as NavigationPropertySegment;
                if (navigationPropertySegment != null)
                {
                    var segmentToRemove = this.GetRedundantTypeCast(navigationPropertySegment);
                    if (segmentToRemove != null)
                    {
                        segmentsToRemove.Add(segmentToRemove);
                    }
                }
            }

            foreach (var segment in segmentsToRemove)
            {
                this.segments.Remove(segment);
            }
        }

        /// <summary>
        /// Get the redundant type cast segment for the given navigation property segment.
        /// </summary>
        /// <param name="navigationPropertySegment">The navigation property segment.</param>
        /// <returns>The redundant type cast segment or null if the type cast is necessary.</returns>
        private ODataPathSegment GetRedundantTypeCast(NavigationPropertySegment navigationPropertySegment)
        {
            var previousSegment = this.GetPreviousSegment(navigationPropertySegment);
            if (previousSegment is TypeSegment)
            {
                var typeSegment = previousSegment;
                var entitySetSegment = this.GetPreviousSegment(typeSegment);

                IEdmEntityType owningType = entitySetSegment.EdmType as IEdmEntityType;
                if (owningType != null && owningType.FindProperty(navigationPropertySegment.NavigationProperty.Name) != null)
                {
                    // Return redundant type cast.
                    return typeSegment;
                }
            }

            return null;
        }

        /// <summary>
        /// Remove redundant entity set path to keep the last non-contained entity set.
        /// </summary>
        private void RemoveRedundantEntitySetPath()
        {
            List<ODataPathSegment> newSegments = new List<ODataPathSegment>();

            // Find the last non-contained navigation property segment.
            var navigationPropertySegment = this.lastSegment as NavigationPropertySegment;
            while (navigationPropertySegment == null ||
                navigationPropertySegment.NavigationProperty.ContainsTarget ||
                navigationPropertySegment.NavigationProperty.TargetMultiplicity() == EdmMultiplicity.One)
            {
                newSegments.Insert(0, this.lastSegment);
                this.RemoveLastSegment();
                navigationPropertySegment = this.lastSegment as NavigationPropertySegment;

                // Have traversed all segments.
                if (this.lastSegment == null)
                {
                    break;
                }
            }

            // Translate the last non-contained property segment to entity set segment.
            if (navigationPropertySegment != null)
            {
                var entitySetSegment = new EntitySetSegment(navigationPropertySegment.NavigationSource as IEdmEntitySet);
                newSegments.Insert(0, entitySetSegment);
            }

            // Set segments to newSegments.
            this.segments.Clear();
            this.segments = newSegments;
            this.lastSegment = this.segments.Last();
        }

        /// <summary>
        /// Remove the last segment from segments.
        /// </summary>
        private void RemoveLastSegment()
        {
            this.segments.Remove(this.lastSegment);
            if (this.segments.Any())
            {
                this.lastSegment = this.segments.Last();
            }
            else
            {
                this.lastSegment = null;
            }
        }

        /// <summary>
        /// Get the segment before the current one.
        /// </summary>
        /// <param name="segment">The current segment.</param>
        /// <returns>The segment before the current one or null if the current one is already the first one.</returns>
        private ODataPathSegment GetPreviousSegment(ODataPathSegment segment)
        {
            int index = this.segments.IndexOf(segment);
            if (index > 1)
            {
                return this.segments[index - 1];
            }

            return null;
        }
    }
}
