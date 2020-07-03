//---------------------------------------------------------------------
// <copyright file="CustomInstanceAnnotationsVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.CustomInstanceAnnotationsTests.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.OData.Framework.Common;
    using Xunit;

    public static class CustomInstanceAnnotationsVerifier
    {
        public static void VerifyAnnotatedItems(this List<CustomInstanceAnnotationsDescriptor> actualAnnotatedItems, string contentType, bool hasExpandedNavigationProperties, bool verifyAnnotationsOnStart, Func<string, bool> shouldIncludeAnnotation)
        {
            var baselineAnnotatedItems = CustomInstanceAnnotationsWriter.AnnotatedItemsBaseline;
            baselineAnnotatedItems.ApplyFilter(shouldIncludeAnnotation);
            VerifyAnnotatedItems(baselineAnnotatedItems, actualAnnotatedItems, contentType, hasExpandedNavigationProperties, verifyAnnotationsOnStart);
        }

        private static void VerifyAnnotatedItems(List<CustomInstanceAnnotationsDescriptor> baselineAnnotatedItems, List<CustomInstanceAnnotationsDescriptor> actualAnnotatedItems, string contentType, bool hasExpandedNavigationProperties, bool verifyAnnotationsOnStart)
        {
            //Number of annotations do not match
            Assert.Equal(baselineAnnotatedItems.Count, actualAnnotatedItems.Count);
            for (int i = 0; i < actualAnnotatedItems.Count; i++)
            {
                var actualItem = actualAnnotatedItems[i];
                var baselineItem = baselineAnnotatedItems[i];
                //Type of annotated item does not match at element {0}, i
                Assert.Equal(baselineItem.TypeOfAnnotatedItem, actualItem.TypeOfAnnotatedItem);

                if (verifyAnnotationsOnStart)
                {
                    var actualAnnotationsOnStart = actualItem.AnnotationsOnStart.ToList();
                    var baselineAnnotationsOnStart = baselineItem.GetExpectedAnnotationsOnStart(contentType, hasExpandedNavigationProperties && actualItem.hasNestedResourceInfo).ToList();
                    VerifyAnnotations(baselineAnnotationsOnStart, actualAnnotationsOnStart, "start", i, actualItem.TypeOfAnnotatedItem.Name);
                }

                var actualAnnotationsOnEnd = actualItem.AnnotationsOnEnd.ToList();
                var baselineAnnotationsOnEnd = baselineItem.AnnotationsOnStart.Concat(baselineItem.AnnotationsOnEnd).ToList();
                VerifyAnnotations(baselineAnnotationsOnEnd, actualAnnotationsOnEnd, "end", i, actualItem.TypeOfAnnotatedItem.Name);
            }
        }

        private static void VerifyAnnotations(List<ODataInstanceAnnotation> baselineAnnotations, List<ODataInstanceAnnotation> actualAnnotations, string startOrEnd, int indexOfAnnotatedItem, string typeOfAnnotatedItem)
        {
            //Number of annotations read at {0} of element {1} ({2}) does not match", startOrEnd, indexOfAnnotatedItem, typeOfAnnotatedItem
            Assert.Equal(baselineAnnotations.Count, actualAnnotations.Count);
            for (int i = 0; i < actualAnnotations.Count; i++)
            {
                var actualAnnotation = actualAnnotations[i];
                var baselineAnnotation = baselineAnnotations[i];
                //"Name of annotation does not match (annotation [{0},{1}])", indexOfAnnotatedItem, i
                Assert.Equal(baselineAnnotation.Name, actualAnnotation.Name);

                var actualAnnotationValue = actualAnnotation.Value;
                var baselineAnnotationValue = baselineAnnotation.Value;
                //Type of annotation does not match (annotation [{0},{1}])", indexOfAnnotatedItem, i
                Assert.Equal(actualAnnotationValue.GetType().FullName, actualAnnotationValue.GetType().FullName);

                var baselinePrimitiveValue = baselineAnnotationValue as ODataPrimitiveValue;
                if (baselinePrimitiveValue != null)
                {
                    VerifyPrimitiveValue(baselinePrimitiveValue.Value, ((ODataPrimitiveValue)actualAnnotationValue).Value, indexOfAnnotatedItem, i);
                }

                // TODO: verify non-primitive values
            }
        }

        private static void VerifyPrimitiveValue(object baselinePrimitiveValue, object actualPrimitiveValue, int indexOfAnnotatedItem, int indexOfValue)
        {
            //"Type of value does not match (annotation [{0},{1}])", indexOfAnnotatedItem, indexOfValue
            Assert.Equal(baselinePrimitiveValue.GetType().FullName, actualPrimitiveValue.GetType().FullName);

            var baselineBinaryValue = baselinePrimitiveValue as byte[];
            if (baselineBinaryValue == null)
            {
                //Value of annotation does not match (annotation [{0},{1}])", indexOfAnnotatedItem, indexOfValue
                Assert.Equal(baselinePrimitiveValue, actualPrimitiveValue);
            }
            else
            {
                var actualBinaryValue = (byte[])actualPrimitiveValue;
                Assert.Equal(baselineBinaryValue.Length, actualBinaryValue.Length);
                for (int i = 0; i < baselineBinaryValue.Length; i++)
                {
                    //Size of binary value does not match (annotation [{0},{1}])", indexOfAnnotatedItem, indexOfValue
                    Assert.Equal(baselineBinaryValue[i], actualBinaryValue[i]);
                }
            }
        }

        private static IEnumerable<ODataInstanceAnnotation> GetExpectedAnnotationsOnStart(this CustomInstanceAnnotationsDescriptor annotatedItem, string contentType, bool hasExpandedNavigationProperties)
        {
            if (annotatedItem.IsFeedWithStreaming(contentType))
            {
                return annotatedItem.AnnotationsOnStart;
            }

            if (annotatedItem.TypeOfAnnotatedItem == typeof(ODataResource) && hasExpandedNavigationProperties)
            {
                return annotatedItem.AnnotationsOnStart;
            }

            return annotatedItem.AnnotationsOnStart.Concat(annotatedItem.AnnotationsOnEnd);
        }

        private static bool IsTopLevelEntryOrEntryOfTopLevelFeed(this CustomInstanceAnnotationsDescriptor annotatedItem)
        {
            // Not an entry
            if (annotatedItem.TypeOfAnnotatedItem != typeof(ODataResource))
            {
                return false;
            }

            // A top level entry
            if (annotatedItem.Parent == null)
            {
                return true;
            }

            // An entry of a top level feed
            if (annotatedItem.Parent.Parent == null)
            {
                Assert.True(annotatedItem.Parent.TypeOfAnnotatedItem == typeof(ODataResourceSet), "Found an entry whose parent is not a feed! Revisit assumptions of top level feed.");
                return true;
            }

            // All other entries
            return false;
        }

        private static bool IsFeedWithStreaming(this CustomInstanceAnnotationsDescriptor item, string contentType)
        {
            return item.TypeOfAnnotatedItem == typeof(ODataResourceSet) && contentType.Contains(MimeTypes.StreamingParameterTrue);
        }

        private static void ApplyFilter(this IEnumerable<CustomInstanceAnnotationsDescriptor> descriptors, Func<string, bool> shouldIncludeAnnotation)
        {
            if (shouldIncludeAnnotation == null)
            {
                // The preference-applied header should be set to *, so we expect the reader to materialize all annotations even if no filter is set
                shouldIncludeAnnotation = s => true;
            }

            foreach(var descriptor in descriptors)
            {
                var annotationsOnStart = descriptor.AnnotationsOnStart.Where(a => shouldIncludeAnnotation(a.Name));
                descriptor.AnnotationsOnStart = annotationsOnStart.ToList();

                var annotationsOnEnd = descriptor.AnnotationsOnEnd.Where(a => shouldIncludeAnnotation(a.Name));
                descriptor.AnnotationsOnEnd = annotationsOnEnd.ToList();
            }
        }
    }
}
