//---------------------------------------------------------------------
// <copyright file="VerifyPayloadHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.WriteJsonPayloadTests
{
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.OData.Framework;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.AstoriaDefaultServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Xunit;

    /// <summary>
    /// Some helper methods to verify various ODataResourceSet/Entry/value payloads.
    /// </summary>
    public static partial class WritePayloadHelper
    {
        /// <summary>
        /// Read the response message and perform given verifications
        /// </summary>
        /// <param name="isFeed">Whether the response has a feed</param>
        /// <param name="responseMessage">The response message</param>
        /// <param name="expectedSet">Expected IEdmEntitySet</param>
        /// <param name="expectedType">Expected IEdmEntityType</param>
        /// <param name="verifyFeed">Action to verify the feed</param>
        /// <param name="verifyEntry">Action to verify the entry</param>
        /// <param name="verifyNavigation">Action to verify the navigation</param>
        public static void ReadAndVerifyFeedEntryMessage(bool isFeed, StreamResponseMessage responseMessage,
                                   IEdmEntitySet expectedSet, IEdmEntityType expectedType,
                                   Action<ODataResourceSet> verifyFeed, Action<ODataResource> verifyEntry,
                                   Action<ODataNestedResourceInfo> verifyNavigation)
        {
            var settings = new ODataMessageReaderSettings() { BaseUri = ServiceUri };
            settings.ShouldIncludeAnnotation = s => true;
            ODataMessageReader messageReader = new ODataMessageReader(responseMessage, settings, Model);
            ODataReader reader = isFeed
                                     ? messageReader.CreateODataResourceSetReader(expectedSet, expectedType)
                                     : messageReader.CreateODataResourceReader(expectedSet, expectedType);
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.ResourceSetEnd:
                        {
                            if (verifyFeed != null)
                            {
                                verifyFeed((ODataResourceSet)reader.Item);
                            }

                            break;
                        }
                    case ODataReaderState.ResourceEnd:
                        {
                            if (verifyEntry != null && reader.Item != null)
                            {
                                verifyEntry((ODataResource)reader.Item);
                            }

                            break;
                        }
                    case ODataReaderState.NestedResourceInfoEnd:
                        {
                            if (verifyNavigation != null)
                            {
                                verifyNavigation((ODataNestedResourceInfo)reader.Item);
                            }

                            break;
                        }
                }
            }

            Assert.Equal(ODataReaderState.Completed, reader.State);
        }

        /// <summary>
        /// Read and return the stream content
        /// </summary>
        /// <param name="stream">The stream content</param>
        /// <returns></returns>
        public static string ReadStreamContent(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }

        /// <summary>
        /// Verify that two atom/json writer results are equivalent
        /// </summary>
        /// <param name="writerOuput1">The first string</param>
        /// <param name="writerOutput2">The second string</param>
        /// <param name="mimeType">The mime type</param>
        public static void VerifyPayloadString(string writerOuput1, string writerOutput2, string mimeType)
        {
            if (string.Equals(mimeType, MimeTypes.ApplicationAtomXml, StringComparison.Ordinal))
            {
                // resulting atom payloads with/without model should be the same except for the updated time stamps
                const string pattern = @"<updated>([A-Za-z0-9\-\:]{20})\</updated>";
                const string replacement = "<updated>0000-00-00T00:00:00Z</updated>";
                writerOuput1 = Regex.Replace(writerOuput1, pattern, (match) => replacement);
                writerOutput2 = Regex.Replace(writerOutput2, pattern, (match) => replacement);
                Assert.Equal(writerOuput1, writerOutput2);
            }
            else
            {
                Assert.Equal(writerOuput1, writerOutput2);
            }
        }

        /// <summary>
        /// Construct the default expected Json object with all metadata
        /// </summary>
        /// <param name="entityType">The IEdmEntityType</param>
        /// <param name="relativeEditLink">The relative edit link</param>
        /// <param name="entry">The ODataEntry </param>
        /// <param name="hasModel">Whether IEdmModel is provided to writer</param>
        /// <param name="isDerivedType">Whether the entry is of derived type</param>
        /// <returns>The expected Json object</returns>
        internal static Dictionary<string, object> ComputeExpectedFullMetadataEntryObject(IEdmEntityType entityType, string relativeEditLink, ODataResource entry, bool hasModel, bool isDerivedType = false)
        {
            var derivedTypeNameSegment = string.Empty;
            if (isDerivedType)
            {
                derivedTypeNameSegment = "/" + NameSpace + entityType.Name;
            }

            Dictionary<string, object> expectedEntryObject = new Dictionary<string, object>();
            expectedEntryObject.Add(JsonLightConstants.ODataTypeAnnotationName, "#" + NameSpace + entityType.Name);
            expectedEntryObject.Add(JsonLightConstants.ODataIdAnnotationName, entry.Id == null ? relativeEditLink : entry.Id.OriginalString);
            expectedEntryObject.Add(JsonLightConstants.ODataEditLinkAnnotationName, entry.EditLink == null ? relativeEditLink + derivedTypeNameSegment : entry.EditLink.AbsoluteUri);

            // when the writer has IEdmModel, expect other metadata in addition to id/edit/readlink 
            if (hasModel)
            {
                // add expected actions
                var boundedActions = Model.FindDeclaredBoundOperations(entityType);
                foreach (var action in boundedActions)
                {
                    var actionFullName = action.FullName();
                    var bindingTypeName = isDerivedType ? "/" + action.Parameters.First().Type.FullName() : "";
                    Dictionary<string, object> actionObject = new Dictionary<string, object>
                        {
                            {"title", actionFullName}, 
                            {"target", ServiceUri + relativeEditLink + bindingTypeName + "/" + actionFullName},
                        };
                    expectedEntryObject.Add("#" + actionFullName, actionObject);
                }

                var baseTypeToAddItem = entityType;
                while (baseTypeToAddItem != null)
                {
                    // add expected navigation properties
                    foreach (var navigation in baseTypeToAddItem.DeclaredNavigationProperties())
                    {
                        var navigationLinkKey = navigation.Name + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNavigationLinkUrlAnnotationName;
                        if (!expectedEntryObject.ContainsKey(navigationLinkKey))
                        {
                            expectedEntryObject.Add(navigationLinkKey, string.Concat(ServiceUri, relativeEditLink, derivedTypeNameSegment, "/", navigation.Name));
                        }
                    }
                    baseTypeToAddItem = baseTypeToAddItem.BaseEntityType();
                }
            }

            foreach (var property in entry.Properties)
            {
                ODataCollectionValue collectionValue = property.Value as ODataCollectionValue;
                if (collectionValue != null)
                {
                    expectedEntryObject.Add(property.Name + JsonLightConstants.ODataTypeAnnotationName, "#" + collectionValue.TypeName);
                }

                expectedEntryObject.Add(property.Name, property.Value);
            }

            return expectedEntryObject;
        }

        /// <summary>
        /// Construct expected MLE/named stream metadata
        /// </summary>
        /// <param name="entityType">The IEdmEntityType</param>
        /// <param name="relativeEditLink">The relative edit link</param>
        /// <param name="entry">The ODataEntry</param>
        /// <param name="expectedEntryObject">The expected Json object</param>
        /// <param name="hasStream">Whether the entity type has MLE stream</param>
        /// <param name="hasModel">Whether IEdmModel is provided to writer</param>
        internal static void ComputeDefaultExpectedFullMetadataEntryMedia(IEdmEntityType entityType, string relativeEditLink, ODataResource entry, Dictionary<string, object> expectedEntryObject, bool hasStream, bool hasModel)
        {
            if (hasStream)
            {
                expectedEntryObject.Add(JsonLightConstants.ODataMediaEditLinkAnnotationName, relativeEditLink + "/$value");
            }

            if (hasModel)
            {
                foreach (var property in entityType.DeclaredProperties.Where(dp => string.Equals(dp.Type.FullName(), "Edm.Stream", StringComparison.Ordinal)))
                {
                    expectedEntryObject.Add(property.Name + JsonLightConstants.ODataMediaEditLinkAnnotationName, ServiceUri + relativeEditLink + "/" + property.Name);
                    expectedEntryObject.Add(property.Name + JsonLightConstants.ODataMediaReadLinkAnnotationName, ServiceUri + relativeEditLink + "/" + property.Name);
                }
            }
        }
    }
}
