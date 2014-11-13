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

namespace Microsoft.Data.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// OData JsonLight serializer for entries and feeds.
    /// </summary>
    internal sealed class ODataJsonLightEntryAndFeedSerializer : ODataJsonLightPropertySerializer
    {
        /// <summary>A map from annotation group name to annotation group for all annotation groups
        /// encountered so far in this payload.</summary>
        private readonly Dictionary<string, ODataJsonLightAnnotationGroup> annotationGroups;

        /// <summary>The metadata uri builder to use.</summary>
        private readonly ODataJsonLightMetadataUriBuilder metadataUriBuilder;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        internal ODataJsonLightEntryAndFeedSerializer(ODataJsonLightOutputContext jsonLightOutputContext)
            : base(jsonLightOutputContext)
        {
            DebugUtils.CheckNoExternalCallers();
            this.annotationGroups = new Dictionary<string, ODataJsonLightAnnotationGroup>(StringComparer.Ordinal);
            
            // DEVNOTE: grab this early so that any validation errors are thrown at creation time rather than when Write___ is called.
            this.metadataUriBuilder = jsonLightOutputContext.CreateMetadataUriBuilder();
        }

        /// <summary>
        /// Gets the base Uri of the metadata document uri, if it has been set.
        /// </summary>
        private Uri MetadataDocumentBaseUri
        {
            get
            {
                // Note: If we are in no-metadata mode or serializing a request, we don't require the MetadataDocumentUri to be set.
                return this.metadataUriBuilder.BaseUri;
            }
        }

        /// <summary>
        /// Writes an annotation group declaration or annotation group reference if specified for the entry.
        /// </summary>
        /// <param name="entry">The entry to write the annotation group declaration or reference for.</param>
        internal void WriteAnnotationGroup(ODataEntry entry)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entry != null, "entry != null");

            ODataJsonLightAnnotationGroup annotationGroup = entry.GetAnnotation<ODataJsonLightAnnotationGroup>();
            if (annotationGroup == null)
            {
                return;
            }

            if (!this.JsonLightOutputContext.WritingResponse)
            {
                throw new ODataException(OData.Strings.ODataJsonLightEntryAndFeedSerializer_AnnotationGroupInRequest);
            }

            string annotationGroupName = annotationGroup.Name;
            if (string.IsNullOrEmpty(annotationGroupName))
            {
                throw new ODataException(OData.Strings.ODataJsonLightEntryAndFeedSerializer_AnnotationGroupWithoutName);
            }

            // Check whether this is the first occurrence of the annotation group.
            ODataJsonLightAnnotationGroup existingAnnotationGroup;
            if (this.annotationGroups.TryGetValue(annotationGroupName, out existingAnnotationGroup))
            {
                // Make sure the annotation groups are reference equal if they have the same name.
                if (!object.ReferenceEquals(existingAnnotationGroup, annotationGroup))
                {
                    throw new ODataException(OData.Strings.ODataJsonLightEntryAndFeedSerializer_DuplicateAnnotationGroup(annotationGroupName));
                }

                // Write an annotation group reference
                this.JsonWriter.WriteName(ODataAnnotationNames.ODataAnnotationGroupReference);
                this.JsonWriter.WritePrimitiveValue(annotationGroupName, this.JsonLightOutputContext.Version);
            }
            else
            {
                // Write an annotation group declaration
                this.JsonWriter.WriteName(ODataAnnotationNames.ODataAnnotationGroup);
                this.JsonWriter.StartObjectScope();
                this.JsonWriter.WriteName(JsonLightConstants.ODataAnnotationGroupNamePropertyName);
                this.JsonWriter.WritePrimitiveValue(annotationGroupName, this.JsonLightOutputContext.Version);

                if (annotationGroup.Annotations != null)
                {
                    foreach (KeyValuePair<string, object> kvp in annotationGroup.Annotations)
                    {
                        string annotationKey = kvp.Key;
                        Debug.Assert(annotationKey != null, "annotationKey != null");
                        if (annotationKey.Length == 0)
                        {
                            throw new ODataException(OData.Strings.ODataJsonLightEntryAndFeedSerializer_AnnotationGroupMemberWithoutName(annotationGroup.Name));
                        }

                        if (!ODataJsonLightReaderUtils.IsAnnotationProperty(annotationKey))
                        {
                            throw new ODataException(OData.Strings.ODataJsonLightEntryAndFeedSerializer_AnnotationGroupMemberMustBeAnnotation(annotationKey, annotationGroup.Name));
                        }

                        this.JsonWriter.WriteName(annotationKey);

                        object annotationValue = kvp.Value;
                        string annotationValueString = annotationValue as string;
                        if (annotationValueString == null && annotationValue != null)
                        {
                            throw new ODataException(OData.Strings.ODataJsonLightEntryAndFeedSerializer_AnnotationGroupMemberWithInvalidValue(annotationKey, annotationGroup.Name, annotationValue.GetType().FullName));
                        }

                        this.JsonWriter.WritePrimitiveValue(annotationValueString, this.JsonLightOutputContext.Version);
                    }
                }

                this.JsonWriter.EndObjectScope();

                // Remember that we wrote the declaration of the annotation group.
                this.annotationGroups.Add(annotationGroupName, annotationGroup);
            }
        }

        /// <summary>
        /// Writes the metadata properties for an entry which can only occur at the start.
        /// </summary>
        /// <param name="entryState">The entry state for which to write the metadata properties.</param>
        internal void WriteEntryStartMetadataProperties(IODataJsonLightWriterEntryState entryState)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entryState != null, "entryState != null");

            ODataEntry entry = entryState.Entry;

            // Write the "odata.type": "typename"
            string typeName = this.JsonLightOutputContext.TypeNameOracle.GetEntryTypeNameForWriting(entryState.GetOrCreateTypeContext(this.Model, this.WritingResponse).ExpectedEntityTypeName, entry);
            if (typeName != null)
            {
                this.JsonWriter.WriteName(ODataAnnotationNames.ODataType);
                this.JsonWriter.WriteValue(typeName);
            }

            // Write the "odata.id": "Entity Id"
            string id = entry.Id;
            if (id != null)
            {
                this.JsonWriter.WriteName(ODataAnnotationNames.ODataId);
                this.JsonWriter.WriteValue(id);
            }

            // Write the "etag": "ETag value"
            string etag = entry.ETag;
            if (etag != null)
            {
                this.JsonWriter.WriteName(ODataAnnotationNames.ODataETag);
                this.JsonWriter.WriteValue(etag);
            }
        }

        /// <summary>
        /// Writes the metadata properties for an entry which can occur both at the start or at the end.
        /// </summary>
        /// <param name="entryState">The entry state for which to write the metadata properties.</param>
        /// <remarks>
        /// This method will only write properties which were not written yet.
        /// </remarks>
        internal void WriteEntryMetadataProperties(IODataJsonLightWriterEntryState entryState)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entryState != null, "entryState != null");

            ODataEntry entry = entryState.Entry;

            // Write the "odata.editLink": "edit-link-uri"
            Uri editLinkUriValue = entry.EditLink;
            if (editLinkUriValue != null && !entryState.EditLinkWritten)
            {
                this.JsonWriter.WriteName(ODataAnnotationNames.ODataEditLink);
                this.JsonWriter.WriteValue(this.UriToString(editLinkUriValue));
                entryState.EditLinkWritten = true;
            }

            // Write the "odata.readLink": "read-link-uri"
            Uri readLinkUriValue = entry.ReadLink;
            if (readLinkUriValue != null && !entryState.ReadLinkWritten)
            {
                this.JsonWriter.WriteName(ODataAnnotationNames.ODataReadLink);
                this.JsonWriter.WriteValue(this.UriToString(readLinkUriValue));
                entryState.ReadLinkWritten = true;
            }

            // Write MLE metadata
            ODataStreamReferenceValue mediaResource = entry.MediaResource;
            if (mediaResource != null)
            {
                // Write the "odata.mediaEditLink": "edit-link-uri"
                Uri mediaEditLinkUriValue = mediaResource.EditLink;
                if (mediaEditLinkUriValue != null && !entryState.MediaEditLinkWritten)
                {
                    this.JsonWriter.WriteName(ODataAnnotationNames.ODataMediaEditLink);
                    this.JsonWriter.WriteValue(this.UriToString(mediaEditLinkUriValue));
                    entryState.MediaEditLinkWritten = true;
                }

                // Write the "odata.mediaReadLink": "read-link-uri"
                Uri mediaReadLinkUriValue = mediaResource.ReadLink;
                if (mediaReadLinkUriValue != null && !entryState.MediaReadLinkWritten)
                {
                    this.JsonWriter.WriteName(ODataAnnotationNames.ODataMediaReadLink);
                    this.JsonWriter.WriteValue(this.UriToString(mediaReadLinkUriValue));
                    entryState.MediaReadLinkWritten = true;
                }

                // Write the "odata.mediaContentType": "content/type"
                string mediaContentType = mediaResource.ContentType;
                if (mediaContentType != null && !entryState.MediaContentTypeWritten)
                {
                    this.JsonWriter.WriteName(ODataAnnotationNames.ODataMediaContentType);
                    this.JsonWriter.WriteValue(mediaContentType);
                    entryState.MediaContentTypeWritten = true;
                }

                // Write the "odata.mediaETag": "ETAG"
                string mediaETag = mediaResource.ETag;
                if (mediaETag != null && !entryState.MediaETagWritten)
                {
                    this.JsonWriter.WriteName(ODataAnnotationNames.ODataMediaETag);
                    this.JsonWriter.WriteValue(mediaETag);
                    entryState.MediaETagWritten = true;
                }
            }

            // TODO: actions
            // TODO: functions
            // TODO: association links
        }

        /// <summary>
        /// Writes the metadata properties for an entry which can only occur at the end.
        /// </summary>
        /// <param name="entryState">The entry state for which to write the metadata properties.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate names checker for properties of this entry.</param>
        internal void WriteEntryEndMetadataProperties(IODataJsonLightWriterEntryState entryState, DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entryState != null, "entryState != null");

            ODataEntry entry = entryState.Entry;

            var navigationLinkInfo = entry.MetadataBuilder.GetNextUnprocessedNavigationLink();
            while (navigationLinkInfo != null)
            {
                Debug.Assert(entry.MetadataBuilder != null, "entry.MetadataBuilder != null");
                navigationLinkInfo.NavigationLink.SetMetadataBuilder(entry.MetadataBuilder);

                this.WriteNavigationLinkMetadata(navigationLinkInfo.NavigationLink, duplicatePropertyNamesChecker);
                navigationLinkInfo = entry.MetadataBuilder.GetNextUnprocessedNavigationLink();
            }

            // write "odata.actions" metadata
            IEnumerable<ODataAction> actions = entry.Actions;
            if (actions != null)
            {
                this.WriteOperations(actions.Cast<ODataOperation>(), /*isAction*/ true);
            }

            // write "odata.functions" metadata
            IEnumerable<ODataFunction> functions = entry.Functions;
            if (functions != null)
            {
                this.WriteOperations(functions.Cast<ODataOperation>(), /*isAction*/ false);
            }
        }

        /// <summary>
        /// Writes the navigation link metadata.
        /// </summary>
        /// <param name="navigationLink">The navigation link to write the metadata for.</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        internal void WriteNavigationLinkMetadata(ODataNavigationLink navigationLink, DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(navigationLink != null, "navigationLink != null");
            Debug.Assert(!string.IsNullOrEmpty(navigationLink.Name), "The navigation link Name should have been validated by now.");
            Debug.Assert(duplicatePropertyNamesChecker != null, "duplicatePropertyNamesChecker != null");

            Uri navigationLinkUrl = navigationLink.Url;
            string navigationLinkName = navigationLink.Name;
            if (navigationLinkUrl != null)
            {
                // The navigation link URL is a property annotation "NavigationLinkName@odata.navigationLinkUrl: 'url'"
                this.JsonWriter.WritePropertyAnnotationName(navigationLinkName, ODataAnnotationNames.ODataNavigationLinkUrl);
                this.JsonWriter.WriteValue(this.UriToString(navigationLinkUrl));
            }

            Uri associationLinkUrl = navigationLink.AssociationLinkUrl;
            if (associationLinkUrl != null)
            {
                duplicatePropertyNamesChecker.CheckForDuplicateAssociationLinkNames(new ODataAssociationLink { Name = navigationLinkName });
                this.WriteAssociationLink(navigationLink.Name, associationLinkUrl);
            }
        }

        /// <summary>
        /// Writes "actions" or "functions" metadata.
        /// </summary>
        /// <param name="operations">The operations to write.</param>
        /// <param name="isAction">true when writing the entry's actions; false when writing the entry's functions.</param>
        internal void WriteOperations(IEnumerable<ODataOperation> operations, bool isAction)
        {
            DebugUtils.CheckNoExternalCallers();

            // We cannot compare two URI's directly because the 'Equals' method on the 'Uri' class compares two 'Uri' instances without regard to the 
            // fragment part of the URI. (E.G: For 'http://someuri/index.htm#EC.action1' and http://someuri/index.htm#EC.action2', the 'Equals' method 
            // will return true.
            IEnumerable<IGrouping<string, ODataOperation>> metadataGroups = operations.GroupBy(o =>
            {
                // We need to validate here to ensure that the metadata is not null, otherwise call to the method 'UriToString' will throw.
                ValidationUtils.ValidateOperationNotNull(o, isAction);
                WriterValidationUtils.ValidateCanWriteOperation(o, this.JsonLightOutputContext.WritingResponse);
                ODataJsonLightValidationUtils.ValidateOperation(this.MetadataDocumentBaseUri, o);
                return this.GetOperationMetadataString(o);
            });

            foreach (IGrouping<string, ODataOperation> metadataGroup in metadataGroups)
            {
                this.WriteOperationMetadataGroup(metadataGroup);
            }
        }

        /// <summary>
        /// Tries to writes the metadata URI property for an entry into the payload if one is available.
        /// </summary>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the entry.</param>
        internal void TryWriteEntryMetadataUri(ODataFeedAndEntryTypeContext typeContext)
        {
            DebugUtils.CheckNoExternalCallers();
            Uri metadataUri;
            if (this.metadataUriBuilder.TryBuildEntryMetadataUri(typeContext, out metadataUri))
            {
                this.WriteMetadataUriProperty(metadataUri);                
            }
        }

        /// <summary>
        /// Tries to writes the metadata URI property for a feed into the payload if one is available.
        /// </summary>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the feed.</param>
        internal void TryWriteFeedMetadataUri(ODataFeedAndEntryTypeContext typeContext)
        {
            DebugUtils.CheckNoExternalCallers();
            Uri metadataUri;
            if (this.metadataUriBuilder.TryBuildFeedMetadataUri(typeContext, out metadataUri))
            {
                this.WriteMetadataUriProperty(metadataUri);
            }
        }

        /// <summary>
        /// Writes an association link property annotation.
        /// </summary>
        /// <param name="propertyName">The name of the navigation property for which to write the association link.</param>
        /// <param name="associationLinkUrl">The association link URL to write.</param>
        private void WriteAssociationLink(string propertyName, Uri associationLinkUrl)
        {
            Debug.Assert(!String.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(associationLinkUrl != null, "associationLinkUrl != null");

            // The association link URL is a property annotation "NavigationLinkName@odata.associationLinkUrl: 'url'"
            this.JsonWriter.WritePropertyAnnotationName(propertyName, ODataAnnotationNames.ODataAssociationLinkUrl);
            this.JsonWriter.WriteValue(this.UriToString(associationLinkUrl));
        }

        /// <summary>
        /// Gets the metadata reference fragment from the operation metadata uri.
        /// i.e. if the operation metadata uri is {absolute metadata document uri}#{container-qualified-operation-name},
        /// this method will return #{container-qualified-operation-name}.
        /// </summary>
        /// <param name="operation">Operation in question.</param>
        /// <returns>The metadata reference fragment from the operation metadata uri.</returns>
        private string GetOperationMetadataString(ODataOperation operation)
        {
            Debug.Assert(operation != null && operation.Metadata != null, "operation != null && operation.Metadata != null");

            string operationMetadataString = UriUtilsCommon.UriToString(operation.Metadata);
            Debug.Assert(ODataJsonLightUtils.IsMetadataReferenceProperty(operationMetadataString), "ODataJsonLightUtils.IsMetadataReferenceProperty(operationMetadataString)");

            // If we don't have a metadata document URI (which is the case with nometadata mode), just write the string form of the Uri we were given.
            if (this.MetadataDocumentBaseUri == null)
            {
                return operation.Metadata.Fragment;
            }
            
            Debug.Assert(
                !ODataJsonLightValidationUtils.IsOpenMetadataReferencePropertyName(this.MetadataDocumentBaseUri, operationMetadataString),
                "Open metadata reference property is not supported, we should have thrown before this point.");

            return JsonLightConstants.MetadataUriFragmentIndicator + ODataJsonLightUtils.GetUriFragmentFromMetadataReferencePropertyName(this.MetadataDocumentBaseUri, operationMetadataString);
        }

        /// <summary>
        /// Returns the target uri string from the given operation.
        /// </summary>
        /// <param name="operation">Operation in question.</param>
        /// <returns>Returns the target uri string from the given operation.</returns>
        private string GetOperationTargetUriString(ODataOperation operation)
        {
            return operation.Target == null ? null : this.UriToString(operation.Target);
        }

        /// <summary>
        /// Validates a group of operations with the same Metadata Uri.
        /// </summary>
        /// <param name="operations">Operations to validate.</param>
        private void ValidateOperationMetadataGroup(IGrouping<string, ODataOperation> operations)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(operations != null, "operations must not be null.");
            Debug.Assert(operations.Any(), "operations.Any()");
            Debug.Assert(operations.All(o => this.GetOperationMetadataString(o) == operations.Key), "The operations should be grouped by their metadata.");

            if (operations.Count() > 1 && operations.Any(o => o.Target == null))
            {
                throw new ODataException(OData.Strings.ODataJsonLightEntryAndFeedSerializer_ActionsAndFunctionsGroupMustSpecifyTarget(operations.Key));
            }

            foreach (IGrouping<string, ODataOperation> operationsByTarget in operations.GroupBy(this.GetOperationTargetUriString))
            {
                if (operationsByTarget.Count() > 1)
                {
                    throw new ODataException(OData.Strings.ODataJsonLightEntryAndFeedSerializer_ActionsAndFunctionsGroupMustNotHaveDuplicateTarget(operations.Key, operationsByTarget.Key));
                }
            }
        }

        /// <summary>
        /// Writes a group of operation (all actions or all functions) that have the same "metadata".
        /// </summary>
        /// <remarks>
        /// Expects the actions or functions scope to already be open.
        /// </remarks>
        /// <param name="operations">A grouping of operations that are all actions or all functions and share the same "metadata".</param>
        private void WriteOperationMetadataGroup(IGrouping<string, ODataOperation> operations)
        {
            this.ValidateOperationMetadataGroup(operations);
            this.JsonLightOutputContext.JsonWriter.WriteName(operations.Key);
            bool useArray = operations.Count() > 1;
            if (useArray)
            {
                this.JsonLightOutputContext.JsonWriter.StartArrayScope();
            }

            foreach (ODataOperation operation in operations)
            {
                this.WriteOperation(operation);
            }

            if (useArray)
            {
                this.JsonLightOutputContext.JsonWriter.EndArrayScope();
            }
        }

        /// <summary>
        /// Writes an operation (an action or a function).
        /// </summary>
        /// <remarks>
        /// Expects the write to already have written the "rel value" and opened an array.
        /// </remarks>
        /// <param name="operation">The operation to write.</param>
        private void WriteOperation(ODataOperation operation)
        {
            Debug.Assert(operation != null, "operation must not be null.");
            Debug.Assert(operation.Metadata != null, "operation.Metadata != null");

            this.JsonLightOutputContext.JsonWriter.StartObjectScope();

            if (operation.Title != null)
            {
                this.JsonLightOutputContext.JsonWriter.WriteName(JsonConstants.ODataOperationTitleName);
                this.JsonLightOutputContext.JsonWriter.WriteValue(operation.Title);
            }

            if (operation.Target != null)
            {
                string targetUrlString = this.GetOperationTargetUriString(operation);
                this.JsonLightOutputContext.JsonWriter.WriteName(JsonConstants.ODataOperationTargetName);
                this.JsonLightOutputContext.JsonWriter.WriteValue(targetUrlString);
            }

            this.JsonLightOutputContext.JsonWriter.EndObjectScope();
        }
    }
}
