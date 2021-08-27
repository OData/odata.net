//---------------------------------------------------------------------
// <copyright file="UpdateResponseVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Astoria.Json;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Response verifier for insert and update requests
    /// </summary>
    public class UpdateResponseVerifier : ResponseVerifierBase, ISelectiveResponseVerifier
    {
        private readonly DataServiceProtocolVersion maxProtocolVersion;

        /// <summary>
        /// Initializes a new instance of the UpdateResponseVerifier class
        /// </summary>
        /// <param name="maxProtocolVersion">The max protocol version of the service</param>
        internal UpdateResponseVerifier(DataServiceProtocolVersion maxProtocolVersion)
        {
            this.maxProtocolVersion = maxProtocolVersion;
        }

        /// <summary>
        /// Gets or sets the structural value comparer
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryStructuralValueToNamedValueComparer Comparer { get; set; }

        /// <summary>
        /// Gets or sets the format selector
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IProtocolFormatStrategySelector FormatSelector { get; set; }

        /// <summary>
        /// Gets or sets the response verification context
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IResponseVerificationContext Context { get; set; }

        /// <summary>
        /// Gets or sets the payload to named values converter
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IPayloadElementToNamedValuesConverter PayloadConverter { get; set; }

        /// <summary>
        /// Gets or sets the metadata resolver
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataPayloadElementMetadataResolver MetadataResolver { get; set; }
        
        /// <summary>
        /// Gets or sets the response verification services
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IResponseVerificationServices VerificationServices { get; set; }

        /// <summary>
        /// Gets or sets the json primitive converter
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IJsonPrimitiveConverter JsonPrimitiveConverter { get; set; }

        /// <summary>
        /// Returns true if this is an update request
        /// </summary>
        /// <param name="request">The request being verified</param>
        /// <returns>Whether or not this verifier applies to the request</returns>
        public bool Applies(ODataRequest request)
        {
            if (request.Uri.LastSegment is FunctionSegment)
            {
                return false;
            }

            if (request.Uri.IsServiceOperation())
            {
                return false;
            }

            if (request.Uri.IsBatch())
            {
                return false;
            }

            var verb = request.GetEffectiveVerb();
            return verb != HttpVerb.Delete && verb != HttpVerb.Get;
        }

        /// <summary>
        /// Returns true if for all non-error responses
        /// </summary>
        /// <param name="response">The response</param>
        /// <returns>Whether or not this verifier applies to the response</returns>
        public bool Applies(ODataResponse response)
        {
            return !response.StatusCode.IsError();
        }

        /// <summary>
        /// Verifies the update succeeded
        /// </summary>
        /// <param name="request">The request to verify</param>
        /// <param name="response">The response to verify</param>
        public override void Verify(ODataRequest request, ODataResponse response)
        {
            base.Verify(request, response);

            if (request.Uri.IsEntityReferenceLink())
            {
                // TODO: handle add link and set link
                return;
            }

            QueryStructuralValue beforeUpdate;
            QueryStructuralValue afterUpdate;
            this.GetUpdatedEntityBeforeAndAfter(request, response, out beforeUpdate, out afterUpdate);

            this.VerifyStoreData(request, response, afterUpdate);

            this.VerifyResponsePayload(request, response, afterUpdate);
        }

        /// <summary>
        /// Re-interprets the request payload in the way the server would, if it contains values which have lossy serialization.
        /// For now, this is limited to date-time values in JSON.
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>The request payload to use for verification</returns>
        internal static ODataPayloadElement ReinterpretRequestPayloadIfNeeded(ODataRequest request)
        {
            return request.Body.RootElement;
        }

        private void GetUpdatedEntityBeforeAndAfter(ODataRequest request, ODataResponse response, out QueryStructuralValue beforeUpdate, out QueryStructuralValue afterUpdate)
        {
            using (this.Context.Begin(request))
            {
                if (request.GetEffectiveVerb() == HttpVerb.Post)
                {
                    afterUpdate = this.Context.GetInsertedEntity(request, response);
                    ExceptionUtilities.CheckObjectNotNull(afterUpdate, "Structural value returned by GetInsertedEntity unexpectedly null");
                    beforeUpdate = afterUpdate.Type.NullValue;
                }
                else
                {
                    this.Context.GetUpdatedEntity(request, out beforeUpdate, out afterUpdate);
                    ExceptionUtilities.CheckObjectNotNull(beforeUpdate, "Before-update structural value returned by GetUpdatedEntity unexpectedly null");
                    ExceptionUtilities.CheckObjectNotNull(afterUpdate, "After-update structural value returned by GetUpdatedEntity unexpectedly null");
                }
            }
        }

        private void VerifyStoreData(ODataRequest request, ODataResponse response, QueryStructuralValue storeValue)
        {
            string contentType;
            ExceptionUtilities.Assert(request.Headers.TryGetValue(HttpHeaders.ContentType, out contentType), "Could not get Content-Type header from request");

            if (request.Uri.IsNamedStream())
            {
                string streamName = request.Uri.Segments.OfType<NamedStreamSegment>().Last().Name;
                var streamValue = storeValue.GetStreamValue(streamName);
                this.VerifyStream(streamValue, contentType, request, response);
                return;
            }

            bool isInsert = request.GetEffectiveVerb() == HttpVerb.Post;
            bool isMediaResource = request.Uri.IsMediaResource();
            if (isInsert)
            {
                EntitySet expectedEntitySet;
                if (request.Uri.TryGetExpectedEntitySet(out expectedEntitySet))
                {
                    isMediaResource = expectedEntitySet.EntityType.HasStream();
                }
            }

            if (isMediaResource)
            {
                var streamValue = storeValue.GetDefaultStreamValue();
                this.VerifyStream(streamValue, contentType, request, response);
            }
            else
            {
                if (isInsert)
                {
                    this.VerifyTypeNameForInsert(request, response, storeValue);
                }

                var formatStrategy = this.FormatSelector.GetStrategy(contentType, request.Uri);
                var primitiveComparer = formatStrategy.GetPrimitiveComparer();

                // TODO: verify relationships
                // TODO: verify PUT vs PATCH semantics
                this.VerifyPropertyValues(request, primitiveComparer, storeValue);
            }
        }

        private void VerifyResponsePayload(ODataRequest request, ODataResponse response, QueryStructuralValue expected)
        {
            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                QueryValue expectedValue = expected;
                if (!request.Uri.IsEntity())
                {
                    var entityUri = request.Uri.ScopeToEntity();
                    foreach (var property in request.Uri.Segments.Skip(entityUri.Segments.Count).OfType<PropertySegment>())
                    {
                        expectedValue = ((QueryStructuralValue)expectedValue).GetValue(property.Property.Name);
                    }
                }

                this.VerificationServices.ValidateResponsePayload(request.Uri, response, expectedValue, this.maxProtocolVersion);
            }
        }

        private void VerifyPropertyValues(ODataRequest request, IQueryScalarValueToClrValueComparer primitiveComparer, QueryStructuralValue afterUpdate)
        {
            var namedValues = this.ExtractNamedValues(request);
            this.Comparer.Compare(afterUpdate, namedValues, primitiveComparer);
        }

        private void VerifyStream(AstoriaQueryStreamValue streamValue, string contentType, ODataRequest request, ODataResponse response)
        {
            this.AssertAreEqual(contentType, streamValue.ContentType, "Stream content type did not match", request, response);

            var expectedPayload = (byte[])((PrimitiveValue)request.Body.RootElement).ClrValue;
            this.AssertAreEqual(expectedPayload, streamValue.Value, "Stream content did not match", request, response);
        }

        private void VerifyTypeNameForInsert(ODataRequest request, ODataResponse response, QueryStructuralValue inserted)
        {
            var entityInstance = request.Body.RootElement as EntityInstance;
            ExceptionUtilities.CheckObjectNotNull(entityInstance, "Cannot get expected type name because request payload was not an entity instance. It was: '{0}'", request.Body.RootElement.ElementType);

            string expectedTypeName = request.Uri.Segments.OfType<EntityTypeSegment>().Select(t => t.EntityType.FullName).LastOrDefault();
            if (entityInstance.FullTypeName != null)
            {
                expectedTypeName = entityInstance.FullTypeName;
            }
            
            var entityType = inserted.Type as QueryEntityType;
            ExceptionUtilities.CheckObjectNotNull(entityType, "Cannot verify type because inserted value is not an entity type. Type was: '{0}'.", inserted.Type);

            this.AssertAreEqual(expectedTypeName, entityType.EntityType.FullName, "Inserted instance type did not match", request, response);
        }

        private IEnumerable<NamedValue> ExtractNamedValues(ODataRequest request)
        {
            bool isProperty = request.Uri.IsProperty();
            bool isValue = request.Uri.IsPropertyValue();

            string prefix = null;
            if (isProperty || isValue)
            {
                // get the uri for the last entity
                var entityUri = request.Uri.ScopeToEntity();

                // get the property names following the entity
                var propertyNamesToPrepend = request.Uri.Segments
                    .Skip(entityUri.Segments.Count)
                    .OfType<PropertySegment>()
                    .Select(p => p.Property.Name)
                    .ToList();

                // for property uri's the last property name is also in the payload, so we shouldnt add it
                if (isProperty)
                {
                    propertyNamesToPrepend.RemoveAt(propertyNamesToPrepend.Count - 1);
                }

                if (propertyNamesToPrepend.Count > 0)
                {
                    prefix = string.Join(".", propertyNamesToPrepend.ToArray());
                }
            }

            if (request.Body.RootElement.ElementType == ODataPayloadElementType.PrimitiveValue)
            {
                return new[] { new NamedValue(prefix, ((PrimitiveValue)request.Body.RootElement).ClrValue) };
            }
            else
            {
                this.MetadataResolver.ResolveMetadata(request.Body.RootElement, request.Uri);
                var payload = ReinterpretRequestPayloadIfNeeded(request);
                var namedValues = this.PayloadConverter.ConvertToNamedValues(payload).ToList();

                if (prefix != null)
                {
                    for (int i = 0; i < namedValues.Count; i++)
                    {
                        var original = namedValues[i];
                        namedValues[i] = new NamedValue(prefix + '.' + original.Name, original.Value);
                    }
                }

                return namedValues;
            }
        }
        
        private void AssertAreEqual(object expected, object actual, string message, ODataRequest request, ODataResponse response)
        {
            string template = @"{0}
Expected: {1}
Actual:   {2}";
            message = string.Format(CultureInfo.InvariantCulture, template, message, expected, actual);
            this.Assert(ValueComparer.Instance.Equals(expected, actual), message, request, response);
        }

        internal class JsonRequestPayloadReinterpretingVisitor : ODataPayloadElementReplacingVisitor
        {
            private readonly IJsonPrimitiveConverter converter;

            internal JsonRequestPayloadReinterpretingVisitor(IJsonPrimitiveConverter converter)
            {
                ExceptionUtilities.CheckArgumentNotNull(converter, "converter");
                this.converter = converter;
            }

            public ODataPayloadElement Normalize(ODataPayloadElement payloadElement)
            {
                return payloadElement.Accept(this);
            }

            public override ODataPayloadElement Visit(PrimitiveValue payloadElement)
            {
                var rawText = payloadElement.Annotations.OfType<RawTextPayloadElementRepresentationAnnotation>().Select(r => r.Text).SingleOrDefault();
                var dataType = payloadElement.Annotations.OfType<DataTypeAnnotation>().Select(d => d.DataType).OfType<PrimitiveDataType>().SingleOrDefault();
                if (rawText != null && dataType != null)
                {
                    using (var reader = new StringReader(rawText))
                    {
                        var tokenizer = new JsonTokenizer(reader);
                        var parsed = tokenizer.Value;

                        if (tokenizer.TokenType == JsonTokenType.String)
                        {
                            var clrType = dataType.GetFacetValue<PrimitiveClrTypeFacet, Type>(null);
                            if (this.converter.TryConvertFromString((string)parsed, clrType, out parsed))
                            {
                                return payloadElement.ReplaceWith(new PrimitiveValue(payloadElement.FullTypeName, parsed));
                            }
                        }
                    }
                }

                return base.Visit(payloadElement);
            }
        }
    }
}
