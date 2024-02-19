//---------------------------------------------------------------------
// <copyright file="ODataJsonLightServiceDocumentSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    #endregion Namespaces

    /// <summary>
    /// OData JsonLight serializer for service documents.
    /// </summary>
    internal sealed class ODataJsonLightServiceDocumentSerializer : ODataJsonLightSerializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        internal ODataJsonLightServiceDocumentSerializer(ODataJsonLightOutputContext jsonLightOutputContext)
            : base(jsonLightOutputContext, /*initContextUriBuilder*/ true)
        {
        }

        /// <summary>
        /// Writes a service document in JsonLight format.
        /// </summary>
        /// <param name="serviceDocument">The service document to write.</param>
        internal void WriteServiceDocument(ODataServiceDocument serviceDocument)
        {
            Debug.Assert(serviceDocument != null, "serviceDocument != null");

            this.WriteTopLevelPayload(
                () =>
                {
                    // "{"
                    this.JsonWriter.StartObjectScope();

                    // "@odata.context":...
                    this.WriteContextUriProperty(ODataPayloadKind.ServiceDocument);

                    // "value":
                    this.JsonWriter.WriteValuePropertyName();

                    // "["
                    this.JsonWriter.StartArrayScope();

                    if (serviceDocument.EntitySets != null)
                    {
                        foreach (ODataEntitySetInfo collectionInfo in serviceDocument.EntitySets)
                        {
                            this.WriteServiceDocumentElement(collectionInfo, JsonLightConstants.ServiceDocumentEntitySetKindName);
                        }
                    }

                    if (serviceDocument.Singletons != null)
                    {
                        foreach (ODataSingletonInfo singletonInfo in serviceDocument.Singletons)
                        {
                            this.WriteServiceDocumentElement(singletonInfo, JsonLightConstants.ServiceDocumentSingletonKindName);
                        }
                    }

                    HashSet<string> functionImportsWritten = new HashSet<string>(StringComparer.Ordinal);

                    if (serviceDocument.FunctionImports != null)
                    {
                        foreach (ODataFunctionImportInfo functionImportInfo in serviceDocument.FunctionImports)
                        {
                            if (functionImportInfo == null)
                            {
                                throw new ODataException(Strings.ValidationUtils_WorkspaceResourceMustNotContainNullItem);
                            }

                            if (!functionImportsWritten.Contains(functionImportInfo.Name))
                            {
                                functionImportsWritten.Add(functionImportInfo.Name);
                                this.WriteServiceDocumentElement(functionImportInfo, JsonLightConstants.ServiceDocumentFunctionImportKindName);
                            }
                        }
                    }

                    // "]"
                    this.JsonWriter.EndArrayScope();

                    // "}"
                    this.JsonWriter.EndObjectScope();
                });
        }

        /// <summary>
        /// Writes a element (EntitySet, Singleton or FunctionImport) in service document.
        /// </summary>
        /// <param name="serviceDocumentElement">The element in service document to write.</param>
        /// <param name="kind">Kind of the service document element, optional for entitysets must for FunctionImport and Singleton.</param>
        private void WriteServiceDocumentElement(ODataServiceDocumentElement serviceDocumentElement, string kind)
        {
            // validate that the resource has a non-null url.
            ValidationUtils.ValidateServiceDocumentElement(serviceDocumentElement, ODataFormat.Json);

            // "{"
            this.JsonWriter.StartObjectScope();

            // "name": ...
            this.JsonWriter.WriteName(JsonLightConstants.ODataServiceDocumentElementName);
            this.JsonWriter.WriteValue(serviceDocumentElement.Name);

            // Do not write title if it is null or empty, or if title is the same as name.
            if (!string.IsNullOrEmpty(serviceDocumentElement.Title) && !serviceDocumentElement.Title.Equals(serviceDocumentElement.Name, StringComparison.Ordinal))
            {
                // "title": ...
                this.JsonWriter.WriteName(JsonLightConstants.ODataServiceDocumentElementTitle);
                this.JsonWriter.WriteValue(serviceDocumentElement.Title);
            }

            // Not always writing because it can be null if an ODataEntitySetInfo, not necessary to write this. Required for the others though.
            if (kind != null)
            {
                // "kind": ...
                this.JsonWriter.WriteName(JsonLightConstants.ODataServiceDocumentElementKind);
                this.JsonWriter.WriteValue(kind);
            }

            // "url": ...
            this.JsonWriter.WriteName(JsonLightConstants.ODataServiceDocumentElementUrlName);
            this.JsonWriter.WriteValue(this.UriToString(serviceDocumentElement.Url));

            // "}"
            this.JsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Asynchronously writes a service document in JsonLight format.
        /// </summary>
        /// <param name="serviceDocument">The service document to write.</param>
        internal Task WriteServiceDocumentAsync(ODataServiceDocument serviceDocument)
        {
            Debug.Assert(serviceDocument != null, "serviceDocument != null");

            return this.WriteTopLevelPayloadAsync(
                async (thisParam, serviceDocumentParam) =>
                {
                    // "{"
                    await thisParam.JsonWriter.StartObjectScopeAsync()
                        .ConfigureAwait(false);

                    // "@odata.context":...
                    await thisParam.WriteContextUriPropertyAsync(ODataPayloadKind.ServiceDocument)
                        .ConfigureAwait(false);

                    // "value":
                    await thisParam.JsonWriter.WriteValuePropertyNameAsync()
                        .ConfigureAwait(false);

                    // "["
                    await thisParam.JsonWriter.StartArrayScopeAsync()
                        .ConfigureAwait(false);

                    if (serviceDocumentParam.EntitySets != null)
                    {
                        foreach (ODataEntitySetInfo collectionInfo in serviceDocumentParam.EntitySets)
                        {
                            await thisParam.WriteServiceDocumentElementAsync(collectionInfo, JsonLightConstants.ServiceDocumentEntitySetKindName)
                                .ConfigureAwait(false);
                        }
                    }

                    if (serviceDocumentParam.Singletons != null)
                    {
                        foreach (ODataSingletonInfo singletonInfo in serviceDocumentParam.Singletons)
                        {
                            await thisParam.WriteServiceDocumentElementAsync(singletonInfo, JsonLightConstants.ServiceDocumentSingletonKindName)
                                .ConfigureAwait(false);
                        }
                    }

                    HashSet<string> functionImportsWritten = new HashSet<string>(StringComparer.Ordinal);

                    if (serviceDocumentParam.FunctionImports != null)
                    {
                        foreach (ODataFunctionImportInfo functionImportInfo in serviceDocumentParam.FunctionImports)
                        {
                            if (functionImportInfo == null)
                            {
                                throw new ODataException(Strings.ValidationUtils_WorkspaceResourceMustNotContainNullItem);
                            }

                            if (!functionImportsWritten.Contains(functionImportInfo.Name))
                            {
                                functionImportsWritten.Add(functionImportInfo.Name);
                                await thisParam.WriteServiceDocumentElementAsync(functionImportInfo, JsonLightConstants.ServiceDocumentFunctionImportKindName)
                                    .ConfigureAwait(false);
                            }
                        }
                    }

                    // "]"
                    await thisParam.JsonWriter.EndArrayScopeAsync()
                        .ConfigureAwait(false);

                    // "}"
                    await thisParam.JsonWriter.EndObjectScopeAsync()
                        .ConfigureAwait(false);
                },
                this,
                serviceDocument);
        }

        /// <summary>
        /// Asynchronously writes a element (EntitySet, Singleton or FunctionImport) in service document.
        /// </summary>
        /// <param name="serviceDocumentElement">The element in service document to write.</param>
        /// <param name="kind">Kind of the service document element, optional for entitysets must for FunctionImport and Singleton.</param>
        private async Task WriteServiceDocumentElementAsync(ODataServiceDocumentElement serviceDocumentElement, string kind)
        {
            // validate that the resource has a non-null url.
            ValidationUtils.ValidateServiceDocumentElement(serviceDocumentElement, ODataFormat.Json);

            // "{"
            await this.JsonWriter.StartObjectScopeAsync()
                .ConfigureAwait(false);

            // "name": ...
            await this.JsonWriter.WriteNameAsync(JsonLightConstants.ODataServiceDocumentElementName)
                .ConfigureAwait(false);
            await this.JsonWriter.WriteValueAsync(serviceDocumentElement.Name)
                .ConfigureAwait(false);

            // Do not write title if it is null or empty, or if title is the same as name.
            if (!string.IsNullOrEmpty(serviceDocumentElement.Title) && !serviceDocumentElement.Title.Equals(serviceDocumentElement.Name, StringComparison.Ordinal))
            {
                // "title": ...
                await this.JsonWriter.WriteNameAsync(JsonLightConstants.ODataServiceDocumentElementTitle)
                    .ConfigureAwait(false);
                await this.JsonWriter.WriteValueAsync(serviceDocumentElement.Title)
                    .ConfigureAwait(false);
            }

            // Not always writing because it can be null if an ODataEntitySetInfo, not necessary to write this. Required for the others though.
            if (kind != null)
            {
                // "kind": ...
                await this.JsonWriter.WriteNameAsync(JsonLightConstants.ODataServiceDocumentElementKind)
                    .ConfigureAwait(false);
                await this.JsonWriter.WriteValueAsync(kind)
                    .ConfigureAwait(false);
            }

            // "url": ...
            await this.JsonWriter.WriteNameAsync(JsonLightConstants.ODataServiceDocumentElementUrlName)
                .ConfigureAwait(false);
            await this.JsonWriter.WriteValueAsync(this.UriToString(serviceDocumentElement.Url))
                .ConfigureAwait(false);

            // "}"
            await this.JsonWriter.EndObjectScopeAsync()
                .ConfigureAwait(false);
        }
    }
}
