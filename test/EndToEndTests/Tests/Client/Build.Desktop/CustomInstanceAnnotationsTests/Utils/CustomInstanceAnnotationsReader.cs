//---------------------------------------------------------------------
// <copyright file="CustomInstanceAnnotationsReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.CustomInstanceAnnotationsTests.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.OData.Tests.Client.Common;
    using System.Text;

    /// <summary>
    /// Utility methods for reading custom instance annotation tests
    /// </summary>
    public static class CustomInstanceAnnotationsReader
    {
        public static IEdmModel GetServiceModel(Uri uri)
        {
            HttpWebRequestMessage message = new HttpWebRequestMessage(uri);
            message.SetHeader("Accept", MimeTypes.ApplicationXml);

            using (var messageReader = new ODataMessageReader(message.GetResponse()))
            {
                return messageReader.ReadMetadataDocument();
            }
        }

        public static List<CustomInstanceAnnotationsDescriptor> ReadEntry(Uri uri, string contentType, ODataMessageReaderSettings readerSettings, IEdmModel model)
        {
            return ReadFeedOrEntry(false /*isFeed*/, uri, contentType, readerSettings, model);
        }

        public static List<CustomInstanceAnnotationsDescriptor> ReadFeed(Uri uri, string contentType, ODataMessageReaderSettings readerSettings, IEdmModel model)
        {
            return ReadFeedOrEntry(true /*isFeed*/, uri, contentType, readerSettings, model);
        }

        public static string GetResponseString(Uri uri, string contentType)
        {
            var responseMessage = GetResponseMessge(uri, contentType);
            var streamReader = new StreamReader(responseMessage.GetStream());
            return streamReader.ReadToEnd();
        }

        public static List<CustomInstanceAnnotationsDescriptor> ReadFeedOrEntry(bool isFeed, Uri uri, string contentType, ODataMessageReaderSettings readerSettings, IEdmModel model)
        {
            var annotationsStack = new Stack<CustomInstanceAnnotationsDescriptor>();
            var allAnnotations = new List<CustomInstanceAnnotationsDescriptor>();

            var responseMessage = GetResponseMessge(uri, contentType);
            using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, model))
            {
                var reader = isFeed ? messageReader.CreateODataResourceSetReader() : messageReader.CreateODataResourceReader();
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceSetStart:
                        case ODataReaderState.ResourceStart:
                            {
                                var instanceAnnotations = GetItemInstanceAnnotations(reader);
                                var parent = annotationsStack.Count == 0 ? null : annotationsStack.Peek();
                                var current = new CustomInstanceAnnotationsDescriptor { TypeOfAnnotatedItem = reader.Item.GetType(), Parent = parent, AnnotationsOnStart = instanceAnnotations };
                                annotationsStack.Push(current);
                                allAnnotations.Add(current);
                                break;
                            }

                        case ODataReaderState.ResourceSetEnd:
                        case ODataReaderState.ResourceEnd:
                            {
                                var instanceAnnotations = GetItemInstanceAnnotations(reader);
                                var current = annotationsStack.Pop();
                                current.AnnotationsOnEnd = instanceAnnotations;
                                break;
                            }
                    }
                }
            }

            return allAnnotations;
        }

        private static ICollection<ODataInstanceAnnotation> GetItemInstanceAnnotations(ODataReader reader)
        {
            var feed = reader.Item as ODataResourceSet;
            var entry = reader.Item as ODataResource;
            var annotations = (feed != null) ? feed.InstanceAnnotations : entry.InstanceAnnotations;

            return annotations.Clone();
        }

        private static ODataInstanceAnnotation Clone(this ODataInstanceAnnotation annotation)
        {
            if (annotation == null)
            {
                return null;
            }

            return new ODataInstanceAnnotation(annotation.Name, annotation.Value);
        }

        private static ICollection<ODataInstanceAnnotation> Clone(this ICollection<ODataInstanceAnnotation> annotations)
        {
            if (annotations == null)
            {
                return null;
            }

            var collectionClone = new List<ODataInstanceAnnotation>();
            foreach (var annotation in annotations)
            {
                collectionClone.Add(annotation.Clone());
            }

            return collectionClone;
        }

        private static IODataResponseMessage GetResponseMessge(Uri uri, string format)
        {
            var requestMessage = new HttpWebRequestMessage(uri);
            requestMessage.SetHeader("Accept", format);

            // We set the prefer odata.include-annotations header to always get instance annotations, 
            // since WCF DS Server does not write instance annotations on responses with DSV< V3
            requestMessage.SetHeader("Prefer", "odata.include-annotations=*");

            return requestMessage.GetResponse();
        }
    }
}