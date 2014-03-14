//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Json
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    #endregion Namespaces

    /// <summary>
    /// OData JSON serializer for entries and feeds.
    /// </summary>
    internal sealed class ODataJsonEntryAndFeedSerializer : ODataJsonPropertyAndValueSerializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonOutputContext">The output context to write to.</param>
        internal ODataJsonEntryAndFeedSerializer(ODataJsonOutputContext jsonOutputContext)
            : base(jsonOutputContext)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Writes the __metadata property and its content for an entry
        /// </summary>
        /// <param name="entry">The entry for which to write the metadata.</param>
        /// <param name="projectedProperties">Set of projected properties, or null if all properties should be written.</param>
        /// <param name="entryEntityType">The entity type of the entry to write.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use.</param>
        internal void WriteEntryMetadata(
            ODataEntry entry,
            ProjectedPropertiesAnnotation projectedProperties,
            IEdmEntityType entryEntityType,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entry != null, "entry != null");

            // Write the "__metadata" for the entry
            this.JsonWriter.WriteName(JsonConstants.ODataMetadataName);
            this.JsonWriter.StartObjectScope();

            // Write the "id": "Entity Id"
            string id = entry.Id;
            if (id != null)
            {
                this.JsonWriter.WriteName(JsonConstants.ODataEntryIdName);
                this.JsonWriter.WriteValue(id);
            }

            // Write the "uri": "edit/read-link-uri"
            Uri uriValue = entry.EditLink ?? entry.ReadLink;

            if (uriValue != null)
            {
                this.JsonWriter.WriteName(JsonConstants.ODataMetadataUriName);
                this.JsonWriter.WriteValue(this.UriToAbsoluteUriString(uriValue));
            }

            // Write the "etag": "ETag value"
            // TODO: if this is a top-level entry also put the ETag into the headers.
            string etag = entry.ETag;
            if (etag != null)
            {
                this.WriteETag(JsonConstants.ODataMetadataETagName, etag);
            }

            // Write the "type": "typename"
            string typeName = entry.TypeName;
            SerializationTypeNameAnnotation serializationTypeNameAnnotation = entry.GetAnnotation<SerializationTypeNameAnnotation>();
            if (serializationTypeNameAnnotation != null)
            {
                typeName = serializationTypeNameAnnotation.TypeName;
            }

            if (typeName != null)
            {
                this.JsonWriter.WriteName(JsonConstants.ODataMetadataTypeName);
                this.JsonWriter.WriteValue(typeName);
            }

            // Write MLE metadata
            ODataStreamReferenceValue mediaResource = entry.MediaResource;
            if (mediaResource != null)
            {
                WriterValidationUtils.ValidateStreamReferenceValue(mediaResource, true);
                this.WriteStreamReferenceValueContent(mediaResource);
            }

            // write "actions" metadata
            IEnumerable<ODataAction> actions = entry.Actions;
            if (actions != null)
            {
                this.WriteOperations(actions.Cast<ODataOperation>(), /*isAction*/ true);
            }

            // write "functions" metadata
            IEnumerable<ODataFunction> functions = entry.Functions;
            if (functions != null)
            {
                this.WriteOperations(functions.Cast<ODataOperation>(), /*isAction*/ false);
            }

            // Write properties metadata
            // For now only association links are supported here
            IEnumerable<ODataAssociationLink> associationLinks = entry.AssociationLinks;
            if (associationLinks != null)
            {
                bool firstAssociationLink = true;

                foreach (ODataAssociationLink associationLink in associationLinks)
                {
                    ValidationUtils.ValidateAssociationLinkNotNull(associationLink);
                    if (projectedProperties.ShouldSkipProperty(associationLink.Name))
                    {
                        continue;
                    }

                    if (firstAssociationLink)
                    {
                        // Write the "properties": {
                        this.JsonWriter.WriteName(JsonConstants.ODataMetadataPropertiesName);
                        this.JsonWriter.StartObjectScope();

                        firstAssociationLink = false;
                    }

                    this.ValidateAssociationLink(associationLink, entryEntityType);
                    this.WriteAssociationLink(associationLink, duplicatePropertyNamesChecker);
                }

                if (!firstAssociationLink)
                {
                    // Close the "properties" object
                    this.JsonWriter.EndObjectScope();
                }
            }

            // Close the __metadata object scope
            this.JsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Writes the metadata content for an association link
        /// </summary>
        /// <param name="associationLink">The association link to write.</param>
        /// <param name="duplicatePropertyNamesChecker">The checker instance for duplicate property names.</param>
        private void WriteAssociationLink(
            ODataAssociationLink associationLink,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            DebugUtils.CheckNoExternalCallers();
            duplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(associationLink);

            // Write the "LinkName": {
            this.JsonWriter.WriteName(associationLink.Name);
            this.JsonWriter.StartObjectScope();

            // Write the "associationuri": "url"
            this.JsonWriter.WriteName(JsonConstants.ODataMetadataPropertiesAssociationUriName);
            this.JsonWriter.WriteValue(this.UriToAbsoluteUriString(associationLink.Url));

            // Close the "LinkName" object
            this.JsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Writes "actions" or "functions" metadata.
        /// </summary>
        /// <param name="operations">The operations to write.</param>
        /// <param name="isAction">true when writing the entry's actions; false when writing the entry's functions.</param>
        private void WriteOperations(IEnumerable<ODataOperation> operations, bool isAction)
        {
            DebugUtils.CheckNoExternalCallers();
            bool firstOperation = true;
            string operationName = isAction ? JsonConstants.ODataActionsMetadataName : JsonConstants.ODataFunctionsMetadataName;

            // we cannot compare two URI's directly because the 'Equals' method on the 'Uri' class compares two 'Uri' instances without regard to the 
            // fragment part of the URI. (E.G: For 'http://someuri/index.htm#EC.action1' and http://someuri/index.htm#EC.action2', the 'Equals' method 
            // will return true.
            var metadataGroups = operations.GroupBy(o =>
            {
                // We need to validate here to ensure that the metadata is not null, otherwise call to the method 'UriToString' will throw.
                ValidationUtils.ValidateOperationNotNull(o, isAction);
                WriterValidationUtils.ValidateOperation(o, this.WritingResponse);

                return this.UriToUriString(o.Metadata, /*makeAbsolute*/ false);
            });

            foreach (IGrouping<string, ODataOperation> metadataGroup in metadataGroups)
            {
                if (firstOperation)
                {
                    // write the the object only if there is any operations.
                    this.JsonWriter.WriteName(operationName);
                    this.JsonWriter.StartObjectScope();
                    firstOperation = false;
                }

                this.WriteOperationMetadataGroup(metadataGroup);
            }

            // close the object if there is any operations.
            if (!firstOperation)
            {
                this.JsonWriter.EndObjectScope();
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
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(operations != null, "operations must not be null.");

            bool first = true;

            foreach (ODataOperation operation in operations)
            {
                if (first)
                {
                    this.JsonWriter.WriteName(operations.Key);
                    this.JsonWriter.StartArrayScope();
                    first = false;
                }

                this.WriteOperation(operation);
            }

            Debug.Assert(first == false, "There was no operations in the rel group. There must be at least operation in every rel group.");
            this.JsonWriter.EndArrayScope();
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
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(operation != null, "operation must not be null.");
            Debug.Assert(operation.Metadata != null, "operation.Metadata != null");
            Debug.Assert(operation.Target != null, "operation.Target != null");

            this.JsonWriter.StartObjectScope();

            if (operation.Title != null)
            {
                this.JsonWriter.WriteName(JsonConstants.ODataOperationTitleName);
                this.JsonWriter.WriteValue(operation.Title);
            }

            string targetUrlString = this.UriToAbsoluteUriString(operation.Target);

            this.JsonWriter.WriteName(JsonConstants.ODataOperationTargetName);
            this.JsonWriter.WriteValue(targetUrlString);

            this.JsonWriter.EndObjectScope();
        }
    }
}
