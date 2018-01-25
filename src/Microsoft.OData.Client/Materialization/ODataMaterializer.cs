//---------------------------------------------------------------------
// <copyright file="ODataMaterializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Client;
    using Microsoft.OData.Edm;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Use this class to materialize objects provided from an <see cref="ODataMessageReader"/>.
    /// </summary>
    internal abstract class ODataMaterializer : IDisposable
    {
        /// <summary>Empty navigation links collection</summary>
        internal static readonly ODataNestedResourceInfo[] EmptyLinks = new ODataNestedResourceInfo[0];

        /// <summary>Empty property collection</summary>
        protected static readonly ODataProperty[] EmptyProperties = new ODataProperty[0];

        /// <summary>Collection->Next Link Table for nested links</summary>
        protected Dictionary<IEnumerable, DataServiceQueryContinuation> nextLinkTable;

        /// <summary>The collection value materialization policy. </summary>
        private readonly CollectionValueMaterializationPolicy collectionValueMaterializationPolicy;

        /// <summary>The enum value materialization policy. </summary>
        private readonly EnumValueMaterializationPolicy enumValueMaterializationPolicy;

        /// <summary>The instance annotation materialization policy. </summary>
        private readonly InstanceAnnotationMaterializationPolicy instanceAnnotationMaterializationPolicy;

        /// <summary> The materialization policy used to materialize primitive values. </summary>
        private readonly PrimitiveValueMaterializationPolicy primitiveValueMaterializationPolicy;

        /// <summary>The converter to use when assigning values of primitive properties. </summary>
        private DSClient.SimpleLazy<PrimitivePropertyConverter> lazyPrimitivePropertyConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataMaterializer" /> class.
        /// </summary>
        /// <param name="materializerContext">The materializer context.</param>
        /// <param name="expectedType">The expected type.</param>
        protected ODataMaterializer(IODataMaterializerContext materializerContext, Type expectedType)
        {
            this.ExpectedType = expectedType;
            this.MaterializerContext = materializerContext;
            this.nextLinkTable = new Dictionary<IEnumerable, DataServiceQueryContinuation>(DSClient.ReferenceEqualityComparer<IEnumerable>.Instance);

            this.enumValueMaterializationPolicy = new EnumValueMaterializationPolicy(this.MaterializerContext);
            this.lazyPrimitivePropertyConverter = new DSClient.SimpleLazy<PrimitivePropertyConverter>(() => new PrimitivePropertyConverter());
            this.primitiveValueMaterializationPolicy = new PrimitiveValueMaterializationPolicy(this.MaterializerContext, this.lazyPrimitivePropertyConverter);
            this.collectionValueMaterializationPolicy = new CollectionValueMaterializationPolicy(this.MaterializerContext, this.primitiveValueMaterializationPolicy);
            this.instanceAnnotationMaterializationPolicy = new InstanceAnnotationMaterializationPolicy(this.MaterializerContext);
            this.collectionValueMaterializationPolicy.InstanceAnnotationMaterializationPolicy = this.instanceAnnotationMaterializationPolicy;
            this.instanceAnnotationMaterializationPolicy.CollectionValueMaterializationPolicy = this.collectionValueMaterializationPolicy;
            this.instanceAnnotationMaterializationPolicy.EnumValueMaterializationPolicy = this.enumValueMaterializationPolicy;
        }

        /// <summary>Current value being materialized; possibly null.</summary>
        /// <remarks>
        /// This will typically be an entity if <see cref="CurrentEntry"/>
        /// is assigned, but may contain a string for example if a top-level
        /// primitive of type string is found.
        /// </remarks>
        internal abstract object CurrentValue { get; }

        /// <summary>Feed being materialized; possibly null.</summary>
        internal abstract ODataResourceSet CurrentFeed { get; }

        /// <summary>Entry being materialized; possibly null.</summary>
        internal abstract ODataResource CurrentEntry { get; }

        /// <summary>Table storing the next links assoicated with the current payload</summary>
        internal Dictionary<IEnumerable, DataServiceQueryContinuation> NextLinkTable
        {
            get { return this.nextLinkTable; }
        }

        /// <summary>Whether we have finished processing the current data stream.</summary>
        internal abstract bool IsEndOfStream { get; }

        /// <summary>
        /// Returns true if the underlying object used for counting is available
        /// </summary>
        internal virtual bool IsCountable
        {
            get { return false; }
        }

        /// <summary>
        ///  The action to set instance annotation associated with current feed
        /// </summary>
        internal Action<IDictionary<string, object>> SetInstanceAnnotations { get; set; }

        /// <summary>
        /// The count tag's value, if requested
        /// </summary>
        /// <returns>The count value returned from the server</returns>
        internal abstract long CountValue { get; }

        /// <summary>Function to materialize an entry and produce a value.</summary>
        internal abstract ProjectionPlan MaterializeEntryPlan { get; }

        /// <summary>
        /// Gets the materializer context
        /// </summary>
        protected internal IODataMaterializerContext MaterializerContext { get; private set; }

        /// <summary>
        /// Returns true if the materializer has been disposed
        /// </summary>
        protected abstract bool IsDisposed { get; }

        /// <summary>
        /// Gets the expected type.
        /// </summary>
        /// <value>
        /// The expected type.
        /// </value>
        protected Type ExpectedType { get; private set; }

        /// <summary>
        /// Gets the collection value materialization policy.
        /// </summary>
        protected CollectionValueMaterializationPolicy CollectionValueMaterializationPolicy
        {
            get { return this.collectionValueMaterializationPolicy; }
        }

        /// <summary>
        /// Gets the collection value materialization policy.
        /// </summary>
        protected EnumValueMaterializationPolicy EnumValueMaterializationPolicy
        {
            get { return this.enumValueMaterializationPolicy; }
        }

        /// <summary>
        /// Gets the instance annotation materialization policy.
        /// </summary>
        protected InstanceAnnotationMaterializationPolicy InstanceAnnotationMaterializationPolicy
        {
            get { return this.instanceAnnotationMaterializationPolicy; }
        }

        /// <summary>
        /// The converter to use when assigning values of primitive properties.
        /// </summary>
        protected PrimitivePropertyConverter PrimitivePropertyConverter
        {
            get { return this.lazyPrimitivePropertyConverter.Value; }
        }

        /// <summary>
        /// The policy used to materialize primitive values.
        /// </summary>
        protected PrimitiveValueMaterializationPolicy PrimitiveValueMaterializier
        {
            get { return this.primitiveValueMaterializationPolicy; }
        }

        /// <summary>
        /// The format of the response being materialized.
        /// </summary>
        protected abstract ODataFormat Format { get; }

        /// <summary>
        /// Creates an <see cref="ODataMaterializer"/> for a response.
        /// </summary>
        /// <param name="responseMessage">The response message.</param>
        /// <param name="responseInfo">The response context.</param>
        /// <param name="materializerType">The type to materialize.</param>
        /// <param name="queryComponents">The query components for the request.</param>
        /// <param name="plan">The projection plan.</param>
        /// <param name="payloadKind">expected payload kind.</param>
        /// <returns>A materializer specialized for the given response.</returns>
        public static ODataMaterializer CreateMaterializerForMessage(
            IODataResponseMessage responseMessage,
            ResponseInfo responseInfo,
            Type materializerType,
            QueryComponents queryComponents,
            ProjectionPlan plan,
            ODataPayloadKind payloadKind)
        {
            ODataMessageReader messageReader = CreateODataMessageReader(responseMessage, responseInfo, ref payloadKind);

            ODataMaterializer result;
            IEdmType edmType = null;

            try
            {
                ODataMaterializerContext materializerContext = new ODataMaterializerContext(responseInfo);

                // Since in V1/V2, astoria client allowed Execute<object> and depended on the typeresolver or the wire type name
                // to get the clr type to materialize. Hence if we see the materializer type as object, we should set the edmtype
                // to null, since there is no expected type.
                if (materializerType != typeof(System.Object))
                {
                    edmType = responseInfo.TypeResolver.ResolveExpectedTypeForReading(materializerType);
                }

                if (payloadKind == ODataPayloadKind.Property && edmType != null)
                {
                    if (edmType.TypeKind.IsStructured())
                    {
                        payloadKind = ODataPayloadKind.Resource;
                    }
                    else if (edmType.TypeKind == EdmTypeKind.Collection && (edmType as IEdmCollectionType).ElementType.IsStructured())
                    {
                        payloadKind = ODataPayloadKind.ResourceSet;
                    }
                }

                if (payloadKind == ODataPayloadKind.Resource || payloadKind == ODataPayloadKind.ResourceSet)
                {
                    // In V1/V2, we allowed System.Object type to be allowed to pass to ExecuteQuery.
                    // Hence we need to explicitly check for System.Object to allow this
                    if (edmType != null && !edmType.TypeKind.IsStructured())
                    {
                        throw DSClient.Error.InvalidOperation(DSClient.Strings.AtomMaterializer_InvalidNonEntityType(materializerType.FullName));
                    }

                    ODataReaderWrapper reader = ODataReaderWrapper.Create(messageReader, payloadKind, edmType, responseInfo.ResponsePipeline);
                    EntityTrackingAdapter entityTrackingAdapter = new EntityTrackingAdapter(responseInfo.EntityTracker, responseInfo.MergeOption, responseInfo.Model, responseInfo.Context);
                    LoadPropertyResponseInfo loadPropertyResponseInfo = responseInfo as LoadPropertyResponseInfo;

                    if (loadPropertyResponseInfo != null)
                    {
                        result = new ODataLoadNavigationPropertyMaterializer(
                            messageReader,
                            reader,
                            materializerContext,
                            entityTrackingAdapter,
                            queryComponents,
                            materializerType,
                            plan,
                            loadPropertyResponseInfo);
                    }
                    else
                    {
                        result = new ODataReaderEntityMaterializer(
                            messageReader,
                            reader,
                            materializerContext,
                            entityTrackingAdapter,
                            queryComponents,
                            materializerType,
                            plan);
                    }
                }
                else
                {
                    switch (payloadKind)
                    {
                        case ODataPayloadKind.Value:
                            result = new ODataValueMaterializer(messageReader, materializerContext, materializerType, queryComponents.SingleResult);
                            break;
                        case ODataPayloadKind.Collection:
                            result = new ODataCollectionMaterializer(messageReader, materializerContext, materializerType, queryComponents.SingleResult);
                            break;
                        case ODataPayloadKind.Property:
                        case ODataPayloadKind.IndividualProperty:
                            // Top level properties cannot be of entity type.
                            if (edmType != null && (edmType.TypeKind == EdmTypeKind.Entity || edmType.TypeKind == EdmTypeKind.Complex))
                            {
                                throw DSClient.Error.InvalidOperation(DSClient.Strings.AtomMaterializer_InvalidEntityType(materializerType.FullName));
                            }

                            result = new ODataPropertyMaterializer(messageReader, materializerContext, materializerType, queryComponents.SingleResult);
                            break;
                        case ODataPayloadKind.EntityReferenceLinks:
                        case ODataPayloadKind.EntityReferenceLink:
                            result = new ODataLinksMaterializer(messageReader, materializerContext, materializerType, queryComponents.SingleResult);
                            break;

                        case ODataPayloadKind.Error:
                            var odataError = messageReader.ReadError();
                            throw new ODataErrorException(odataError.Message, odataError);
                        default:
                            throw DSClient.Error.InvalidOperation(DSClient.Strings.AtomMaterializer_InvalidResponsePayload(XmlConstants.DataWebNamespace));
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                if (CommonUtil.IsCatchableExceptionType(ex))
                {
                    // Dispose the message reader in all error scenarios.
                    messageReader.Dispose();
                }

                throw;
            }
        }

        /// <summary>Reads the next value from the input content.</summary>
        /// <returns>true if another value is available after reading; false otherwise.</returns>
        /// <remarks>
        /// After invocation, the currentValue field (and CurrentValue property) will
        /// reflect the value materialized from the parser; possibly null if the
        /// result is true (for null values); always null if the result is false.
        /// </remarks>
        public bool Read()
        {
            this.VerifyNotDisposed();

            return this.ReadImplementation();
        }

        /// <summary>
        /// Disposes the materializer
        /// </summary>
        public void Dispose()
        {
            this.OnDispose();

            Debug.Assert(this.IsDisposed, "this.IsDisposed");
        }

        /// <summary>Clears the materialization log of activity.</summary>
        internal abstract void ClearLog();

        /// <summary>Applies the materialization log to the context.</summary>
        internal abstract void ApplyLogToContext();

        /// <summary>
        /// Creates an <see cref="ODataMessageReader"/> for a given message and context using
        /// WCF DS client settings.
        /// </summary>
        /// <param name="responseMessage">The response message</param>
        /// <param name="responseInfo">The response context</param>
        /// <param name="payloadKind">Type of the message.</param>
        /// <returns>The message reader.</returns>
        protected static ODataMessageReader CreateODataMessageReader(IODataResponseMessage responseMessage, ResponseInfo responseInfo, ref ODataPayloadKind payloadKind)
        {
            ODataMessageReaderSettings settings = responseInfo.ReadHelper.CreateSettings();

            ODataMessageReader odataMessageReader = responseInfo.ReadHelper.CreateReader(responseMessage, settings);

            if (payloadKind == ODataPayloadKind.Unsupported)
            {
                var payloadKinds = odataMessageReader.DetectPayloadKind().ToList();

                if (payloadKinds.Count == 0)
                {
                    throw DSClient.Error.InvalidOperation(DSClient.Strings.AtomMaterializer_InvalidResponsePayload(XmlConstants.DataWebNamespace));
                }

                // Pick the first payload kind detected by ODataLib and use that to parse the exception.
                // The only exception being payload with entity reference link(s). If one of the payload kinds
                // is reference links, then we need to give preference to reference link payloads.
                ODataPayloadKindDetectionResult detectionResult = payloadKinds.FirstOrDefault(k => k.PayloadKind == ODataPayloadKind.EntityReferenceLink || k.PayloadKind == ODataPayloadKind.EntityReferenceLinks);

                if (detectionResult == null)
                {
                    detectionResult = payloadKinds.First();
                }

                if (detectionResult.Format != ODataFormat.Json && detectionResult.Format != ODataFormat.RawValue)
                {
                    throw DSClient.Error.InvalidOperation(DSClient.Strings.AtomMaterializer_InvalidContentTypeEncountered(responseMessage.GetHeader(XmlConstants.HttpContentType)));
                }

                payloadKind = detectionResult.PayloadKind;
            }

            return odataMessageReader;
        }

        /// <summary>
        /// Verifies that the object is not disposed.
        /// </summary>
        protected void VerifyNotDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(typeof(ODataEntityMaterializer).FullName);
            }
        }

        /// <summary>
        /// Implementation of <see cref="Read"/>.
        /// </summary>
        /// <returns>Return value of <see cref="Read"/></returns>
        protected abstract bool ReadImplementation();

        /// <summary>
        /// Called when IDisposable.Dispose is called.
        /// </summary>
        protected abstract void OnDispose();
    }
}
