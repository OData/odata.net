//---------------------------------------------------------------------
// <copyright file="JsonPayloadNormalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Spatial.Contracts;

    /// <summary>
    /// Payload element normalizer for the json format
    /// </summary>
    public class JsonPayloadNormalizer : ODataPayloadElementNormalizerBase
    {
        /// <summary>
        /// Gets or sets the primitive converter to use
        /// </summary>
        [InjectDependency]
        public IJsonPrimitiveConverter PrimitiveConverter { get; set; }

        /// <summary>
        /// Gets or sets the geometry to geography converter.
        /// </summary>
        [InjectDependency]
        public IGeometryToGeographyConverter GeometryToGeographyConverter { get; set; }

        /// <summary>
        /// Replaces the empty collection property with a more specific type
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public override ODataPayloadElement Visit(EmptyUntypedCollection payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var expectedTypeAnnotatation = payloadElement.Annotations.OfType<ExpectedPayloadElementTypeAnnotation>().SingleOrDefault();
            if (expectedTypeAnnotatation != null && expectedTypeAnnotatation.ExpectedType == ODataPayloadElementType.LinkCollection)
            {
                var linkCollection = new LinkCollection();
                linkCollection.InlineCount = payloadElement.InlineCount;
                linkCollection.NextLink = payloadElement.NextLink;
                return payloadElement.ReplaceWith(linkCollection);
            }

            var entitySetAnnotation = payloadElement.Annotations.OfType<EntitySetAnnotation>().SingleOrDefault();
            if (entitySetAnnotation != null)
            {
                var entitySet = new EntitySetInstance();
                entitySet.InlineCount = payloadElement.InlineCount;
                entitySet.NextLink = payloadElement.NextLink;
                return payloadElement.ReplaceWith(entitySet);
            }

            var functionAnnotation = payloadElement.Annotations.OfType<FunctionAnnotation>().SingleOrDefault();
            if (functionAnnotation != null && expectedTypeAnnotatation != null)
            {
                if (expectedTypeAnnotatation.ExpectedType == ODataPayloadElementType.ComplexInstanceCollection)
                {
                    return payloadElement.ReplaceWith(new ComplexInstanceCollection());
                }

                if (expectedTypeAnnotatation.ExpectedType == ODataPayloadElementType.PrimitiveCollection)
                {
                    return payloadElement.ReplaceWith(new PrimitiveCollection());
                }
            }

            return base.Visit(payloadElement);
        }

        /// <summary>
        /// Determines whether the value is a geometry instanct that must be converted to geography.
        /// </summary>
        /// <param name="value">The value which might need to be converted.</param>
        /// <param name="expectedType">The expected type.</param>
        /// <returns>True if the value must be converted, otherwise false</returns>
        internal static bool MustConvertGeometryToGeography(object value, Type expectedType)
        {
            if (value == null)
            {
                return false;
            }

            SpatialTypeKind? actualKind;
            if (!SpatialUtilities.TryInferSpatialTypeKind(value.GetType(), out actualKind))
            {
                return false;
            }

            SpatialTypeKind? expectedKind;
            if (!SpatialUtilities.TryInferSpatialTypeKind(expectedType, out expectedKind))
            {
                return false;
            }

            return expectedKind == SpatialTypeKind.Geography && actualKind == SpatialTypeKind.Geometry;
        }

        /// <summary>
        /// Converts the given primitive value to the expected type
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="expectedType">The expected type based on the metadata</param>
        /// <returns>The converted object</returns>
        protected override object ConvertPrimitiveValue(object value, Type expectedType)
        {
            var stringValue = value as string;
            object converted;
            if (stringValue != null && this.PrimitiveConverter.TryConvertFromString(stringValue, expectedType, out converted))
            {
                return converted;
            }

            if (MustConvertGeometryToGeography(value, expectedType))
            {
                ExceptionUtilities.CheckObjectNotNull(this.GeometryToGeographyConverter, "Cannot convert Geometry to Geography without converter");

                // in GeoJSON the first value (or x after parsing) is longitude.
                return this.GeometryToGeographyConverter.Convert(value, false);
            }

            return base.ConvertPrimitiveValue(value, expectedType);
        }
    }
}
