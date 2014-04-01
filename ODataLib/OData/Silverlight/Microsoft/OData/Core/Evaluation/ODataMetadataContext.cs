//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Evaluation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Core.JsonLight;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Interface used for substitutability of the metadata-centric responsibilities of <see cref="ODataJsonLightDeserializer"/>.
    /// </summary>
    internal interface IODataMetadataContext
    {
        /// <summary>
        /// Gets the Edm Model.
        /// </summary>
        IEdmModel Model { get; }

        /// <summary>
        /// Gets the service base Uri.
        /// </summary>
        Uri ServiceBaseUri { get; }

        /// <summary>
        /// Gets the metadata document uri.
        /// </summary>
        Uri MetadataDocumentUri { get; }

        /// <summary>
        /// Gets the OData uri.
        /// </summary>
        ODataUri ODataUri { get; }

        /// <summary>
        /// Gets an entity metadata builder for the given entry.
        /// </summary>
        /// <param name="entryState">Entry state to use as reference for information needed by the builder.</param>
        /// <returns>An entity metadata builder.</returns>
        ODataEntityMetadataBuilder GetEntityMetadataBuilderForReader(IODataJsonLightReaderEntryState entryState);

        /// <summary>
        /// Gets the list of operations that are bindable to a type.
        /// </summary>
        /// <param name="bindingType">The binding type in question.</param>
        /// <returns>The list of operations that are always bindable to a type.</returns>
        IEdmOperation[] GetBindableOperationsForType(IEdmType bindingType);

        /// <summary>
        /// Determines whether operations bound to this type must be qualified with the operation they belong to when appearing in a $select clause.
        /// </summary>
        /// <param name="entityType">The entity type the operations are bound to.</param>
        /// <returns>True if the operations must be container qualified, otherwise false.</returns>
        bool OperationsBoundToEntityTypeMustBeContainerQualified(IEdmEntityType entityType);
    }

    /// <summary>
    /// Default implementation of <see cref="IODataMetadataContext"/>.
    /// </summary>
    internal sealed class ODataMetadataContext : IODataMetadataContext
    {
        /// <summary>
        /// The Edm Model.
        /// </summary>
        private readonly IEdmModel model;

        /// <summary>
        /// EdmTypeResolver instance to resolve entity set base type.
        /// </summary>
        private readonly EdmTypeResolver edmTypeResolver;

        /// <summary>
        /// Cache of operations that are bindable to entity types.
        /// </summary>
        private readonly Dictionary<IEdmType, IEdmOperation[]> bindableOperationsCache;

        /// <summary>
        /// true if we are reading or writing a response payload, false otherwise.
        /// </summary>
        private readonly bool isResponse;

        /// <summary>
        /// Callback to determine whether operations bound to this type must be qualified with the operation they belong to when appearing in a $select clause.
        /// </summary>
        private readonly Func<IEdmEntityType, bool> operationsBoundToEntityTypeMustBeContainerQualified;

        /// <summary>
        /// The metadata document Uri.
        /// </summary>
        private readonly Uri metadataDocumentUri;

        /// <summary>
        /// The OData Uri.
        /// </summary>
        private readonly ODataUri odataUri;

        /// <summary>
        /// The service base Uri.
        /// </summary>
        private Uri serviceBaseUri;

        /// <summary>
        /// The MetadataLevel.
        /// </summary>
        private JsonLightMetadataLevel metadataLevel;

        /// <summary>
        /// Constructs an ODataMetadataContext.
        /// </summary>
        /// <param name="isResponse">true if we are writing a response payload, false otherwise.</param>
        /// <param name="model">The Edm model.</param>
        /// <param name="metadataDocumentUri">The metadata document uri.</param>
        /// <param name="odataUri">The request Uri.</param>
        /// <remarks>This overload should only be used by the writer.</remarks>
        public ODataMetadataContext(bool isResponse, IEdmModel model, Uri metadataDocumentUri, ODataUri odataUri)
            : this(isResponse, /*OperationsBoundToEntityTypeMustBeContainerQualified*/ null, EdmTypeWriterResolver.Instance, model, metadataDocumentUri, odataUri)
        {
        }

        /// <summary>
        /// Constructs an ODataMetadataContext.
        /// </summary>
        /// <param name="isResponse">true if we are reading a response payload, false otherwise.</param>
        /// <param name="operationsBoundToEntityTypeMustBeContainerQualified">Callback to determine whether operations bound to this type must be qualified with the operation they belong to when appearing in a $select clause.</param>
        /// <param name="edmTypeResolver">EdmTypeResolver instance to resolve entity set base type.</param>
        /// <param name="model">The Edm model.</param>
        /// <param name="metadataDocumentUri">The metadata document Uri.</param>
        /// <param name="odataUri">The request Uri.</param>
        /// <remarks>This overload should only be used by the reader.</remarks>
        public ODataMetadataContext(
            bool isResponse,
            Func<IEdmEntityType, bool> operationsBoundToEntityTypeMustBeContainerQualified,
            EdmTypeResolver edmTypeResolver,
            IEdmModel model,
            Uri metadataDocumentUri,
            ODataUri odataUri)
        {
            Debug.Assert(edmTypeResolver != null, "edmTypeResolver != null");
            Debug.Assert(model != null, "model != null");

            this.isResponse = isResponse;
            this.operationsBoundToEntityTypeMustBeContainerQualified = operationsBoundToEntityTypeMustBeContainerQualified ?? EdmLibraryExtensions.OperationsBoundToEntityTypeMustBeContainerQualified;
            this.edmTypeResolver = edmTypeResolver;
            this.model = model;
            this.metadataDocumentUri = metadataDocumentUri;
            this.bindableOperationsCache = new Dictionary<IEdmType, IEdmOperation[]>(ReferenceEqualityComparer<IEdmType>.Instance);
            this.odataUri = odataUri;
        }

        /// <summary>
        /// Constructs an ODataMetadataContext.
        /// </summary>
        /// <param name="isResponse">true if we are reading a response payload, false otherwise.</param>
        /// <param name="operationsBoundToEntityTypeMustBeContainerQualified">Callback to determine whether operations bound to this type must be qualified with the operation they belong to when appearing in a $select clause.</param>
        /// <param name="edmTypeResolver">EdmTypeResolver instance to resolve entity set base type.</param>
        /// <param name="model">The Edm model.</param>
        /// <param name="metadataDocumentUri">The metadata document Uri.</param>
        /// <param name="odataUri">The request Uri.</param>
        /// <param name="metadataLevel">Current Json Light MetadataLevel</param>
        /// <remarks>This overload should only be used by the reader.</remarks>
        public ODataMetadataContext(
            bool isResponse,
            Func<IEdmEntityType, bool> operationsBoundToEntityTypeMustBeContainerQualified,
            EdmTypeResolver edmTypeResolver,
            IEdmModel model,
            Uri metadataDocumentUri,
            ODataUri odataUri,
            JsonLightMetadataLevel metadataLevel)
            : this(isResponse, operationsBoundToEntityTypeMustBeContainerQualified, edmTypeResolver, model, metadataDocumentUri, odataUri)
        {
            Debug.Assert(metadataLevel != null, "MetadataLevel != null");

            this.metadataLevel = metadataLevel;
        }

        /// <summary>
        /// Gets the Edm Model.
        /// </summary>
        public IEdmModel Model
        {
            get
            {
                Debug.Assert(this.model != null, "this.model != null");
                return this.model;
            }
        }

        /// <summary>
        /// Gets the service base Uri.
        /// </summary>
        public Uri ServiceBaseUri
        {
            get { return this.serviceBaseUri ?? (this.serviceBaseUri = new Uri(this.MetadataDocumentUri, "./")); }
        }

        /// <summary>
        /// Gets the metadata document uri.
        /// </summary>
        public Uri MetadataDocumentUri
        {
            get
            {
                if (this.metadataDocumentUri == null)
                {
                    throw new ODataException(OData.Core.Strings.ODataJsonLightEntryMetadataContext_MetadataAnnotationMustBeInPayload(ODataAnnotationNames.ODataContext));
                }

                Debug.Assert(this.metadataDocumentUri.IsAbsoluteUri, "this.metadataDocumentUri.IsAbsoluteUri");
                return this.metadataDocumentUri;
            }
        }

        /// <summary>
        /// Gets the OData uri.
        /// </summary>
        public ODataUri ODataUri
        {
            get
            {
                return this.odataUri;
            }
        }

        /// <summary>
        /// Gets an entity metadata builder for the given entry.
        /// </summary>
        /// <param name="entryState">Entry state to use as reference for information needed by the builder.</param>
        /// <returns>An entity metadata builder.</returns>
        public ODataEntityMetadataBuilder GetEntityMetadataBuilderForReader(IODataJsonLightReaderEntryState entryState)
        {
            Debug.Assert(entryState != null, "entry != null");

            // Only apply the conventional template builder on response. On a request we would only report what's on the wire.
            if (entryState.MetadataBuilder == null)
            {
                ODataEntry entry = entryState.Entry;
                if (this.isResponse)
                {
                    ODataTypeAnnotation typeAnnotation = entry.GetAnnotation<ODataTypeAnnotation>();

                    Debug.Assert(typeAnnotation != null, "The JSON light reader should have already set the ODataTypeAnnotation.");
                    IEdmNavigationSource navigationSource = typeAnnotation.NavigationSource;

                    IEdmEntityType navigationSourceElementType = this.edmTypeResolver.GetElementType(navigationSource);
                    IODataFeedAndEntryTypeContext typeContext = ODataFeedAndEntryTypeContext.Create(/*serializationInfo*/ null, navigationSource, navigationSourceElementType, entryState.EntityType, this.model, /*throwIfMissingTypeInfo*/ true);
                    IODataEntryMetadataContext entryMetadataContext = ODataEntryMetadataContext.Create(entry, typeContext, /*serializationInfo*/null, (IEdmEntityType)entry.GetEdmType().Definition, this, entryState.SelectedProperties);

                    UrlConvention urlConvention = UrlConvention.ForUserSettingAndTypeContext(/*keyAsSegment*/ null, typeContext);
                    ODataConventionalUriBuilder uriBuilder = new ODataConventionalUriBuilder(this.ServiceBaseUri, urlConvention);

                    entryState.MetadataBuilder = new ODataConventionalEntityMetadataBuilder(entryMetadataContext, this, uriBuilder);
                }
                else
                {
                    entryState.MetadataBuilder = new NoOpEntityMetadataBuilder(entry);
                }
            }

            return entryState.MetadataBuilder;
        }

        /// <summary>
        /// Gets the list of operations that are always bindable to a type.
        /// </summary>
        /// <param name="bindingType">The binding type in question.</param>
        /// <returns>The list of operations that are always bindable to a type.</returns>
        public IEdmOperation[] GetBindableOperationsForType(IEdmType bindingType)
        {
            Debug.Assert(bindingType != null, "bindingType != null");
            Debug.Assert(this.bindableOperationsCache != null, "this.bindableOperationsCache != null");
            Debug.Assert(this.isResponse, "this.readingResponse");

            IEdmOperation[] bindableOperations;
            if (!this.bindableOperationsCache.TryGetValue(bindingType, out bindableOperations))
            {
                bindableOperations = MetadataUtils.CalculateBindableOperationsForType(bindingType, this.model, this.edmTypeResolver);
                this.bindableOperationsCache.Add(bindingType, bindableOperations);
            }

            return bindableOperations;
        }

        /// <summary>
        /// Determines whether operations bound to this type must be qualified with the operation they belong to when appearing in a $select clause.
        /// </summary>
        /// <param name="entityType">The entity type the operations are bound to.</param>
        /// <returns>True if the operations must be container qualified, otherwise false.</returns>
        public bool OperationsBoundToEntityTypeMustBeContainerQualified(IEdmEntityType entityType)
        {
            Debug.Assert(entityType != null, "entityType != null");
            Debug.Assert(this.isResponse, "this.isResponse");
            Debug.Assert(this.operationsBoundToEntityTypeMustBeContainerQualified != null, "this.operationsBoundToEntityTypeMustBeContainerQualified != null");

            return this.operationsBoundToEntityTypeMustBeContainerQualified(entityType);
        }
    }
}
