//---------------------------------------------------------------------
// <copyright file="PropertyAnnotationCollector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using Microsoft.OData.Json;

    /// <summary>
    /// An independent annotation collector to collect the raw json annotations.
    /// </summary>
    internal sealed class PropertyAnnotationCollector
    {
        /// <summary>
        /// The raw annotations.
        /// </summary>
        private Dictionary<string, List<ODataInstanceAnnotation>> propertyAnnotations =
            new Dictionary<string, List<ODataInstanceAnnotation>>(StringComparer.Ordinal);

        /// <summary>
        /// If should collect annotation.
        /// </summary>
        private bool shouldCollectAnnotation;

        /// <summary>
        /// Gets or sets if should collect annotation;
        /// </summary>
        public bool ShouldCollectAnnotation
        {
            get
            {
                return this.shouldCollectAnnotation;
            }

            set
            {
                this.shouldCollectAnnotation = value;
            }
        }

        /// <summary>
        /// Tries to peek and collect a raw annotation value from BufferingJsonReader.
        /// </summary>
        /// <param name="annotationCollector">The property annotation collector, may be null.</param>
        /// <param name="jsonReader">The BufferingJsonReader.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="annotationName">The annotation name.</param>
        public static void TryPeekAndCollectAnnotationRawValue(
            PropertyAnnotationCollector annotationCollector,
            BufferingJsonReader jsonReader,
            string propertyName,
            string annotationName)
        {
            if (annotationCollector != null && annotationCollector.shouldCollectAnnotation)
            {
                annotationCollector.PeekAndCollectAnnotationRawValue(jsonReader, propertyName, annotationName);
            }
        }

        /// <summary>
        /// Tries to add property annotation raw json.
        /// </summary>
        /// <param name="annotationCollector">The property annotation collector, may be null.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="annotationName">The annotation name.</param>
        /// <param name="rawValue">The raw json string.</param>
        public static void TryAddPropertyAnnotationRawValue(PropertyAnnotationCollector annotationCollector, string propertyName, string annotationName, string rawValue)
        {
            if (annotationCollector != null && annotationCollector.shouldCollectAnnotation)
            {
                annotationCollector.AddPropertyAnnotationRawValue(propertyName, annotationName, rawValue);
            }
        }

        /// <summary>
        /// Gets an ODataInstanceAnnotation collection that can be attached to ODataValue or ODataUntypedValue.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns>An ODataInstanceAnnotation collection.</returns>
        public ICollection<ODataInstanceAnnotation> GetPropertyRawAnnotations(string propertyName)
        {
            List<ODataInstanceAnnotation> annotations;
            if (!this.propertyAnnotations.TryGetValue(propertyName, out annotations))
            {
                annotations = new List<ODataInstanceAnnotation>();
                this.propertyAnnotations[propertyName] = annotations;
            }

            return annotations;
        }

        /// <summary>
        /// Peeks and collects a raw annotation value from BufferingJsonReader.
        /// </summary>
        /// <param name="jsonReader">The BufferingJsonReader.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="annotationName">The annotation name.</param>
        private void PeekAndCollectAnnotationRawValue(
            BufferingJsonReader jsonReader,
            string propertyName,
            string annotationName)
        {
            if (jsonReader.IsBuffering)
            {
                return; // only need to collect annotation for not-buffering pass reading.
            }

            try
            {
                jsonReader.StartBuffering();
                if (jsonReader.NodeType == JsonNodeType.Property)
                {
                    jsonReader.Read(); // read over annotation name
                }

                StringBuilder builder = new StringBuilder();
                jsonReader.SkipValue(builder);
                string annotationRawValue = builder.ToString();
                this.AddPropertyAnnotationRawValue(propertyName, annotationName, annotationRawValue);
            }
            finally
            {
                jsonReader.StopBuffering();
            }
        }

        /// <summary>
        /// Add property annotation raw json.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="annotationName">The annotation name.</param>
        /// <param name="rawValue">The raw json string.</param>
        private void AddPropertyAnnotationRawValue(string propertyName, string annotationName, string rawValue)
        {
            Debug.Assert(annotationName != null, "annotationName != null");
            List<ODataInstanceAnnotation> annotations = null;
            if (!this.propertyAnnotations.TryGetValue(propertyName, out annotations))
            {
                annotations = new List<ODataInstanceAnnotation>();
                this.propertyAnnotations[propertyName] = annotations;
            }

            annotations.Add(new ODataInstanceAnnotation(
                annotationName,
                new ODataUntypedValue() { RawValue = rawValue },
                true));
        }
    }
}
