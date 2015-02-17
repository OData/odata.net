//---------------------------------------------------------------------
// <copyright file="PayloadElementToJsonConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Json;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// The converter from a reach payload elemnt representation to the atom/Json representation.
    /// </summary>
    [ImplementationName(typeof(IPayloadElementToJsonConverter), "Default", HelpText = "The default converter from a reach payload elemnt representation to the atom/Json representation.")]
    public class PayloadElementToJsonConverter : IPayloadElementToJsonConverter, IDisposable
    {
        private JsonWriter writer;

        /// <summary>
        /// Gets or sets the spatial converter to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ISpatialPrimitiveToODataJsonValueConverter SpatialConverter { get; set; }

        /// <summary>
        /// Gets or sets the json primitive converter.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IJsonPrimitiveConverter PrimitiveConverter { get; set; }

        /// <summary>
        /// Converts the given payload element into an Json representation.
        /// </summary>
        /// <param name="rootElement">The root payload element to convert.</param>        
        /// <returns>The Json representation of the payload.</returns>
        public string ConvertToJson(ODataPayloadElement rootElement)
        {
            StringBuilder builder = new StringBuilder();
            using (StringWriter strWriter = new StringWriter(builder, CultureInfo.CurrentCulture))
            {
                this.writer = new JsonWriter(strWriter);
                this.Serialize(rootElement);
                this.Dispose();
            }

            return builder.ToString();
        }

        /// <summary>
        /// disposes the writer
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool dispose)
        {
            if (dispose)
            {
                if (this.writer != null)
                {
                    this.writer.Dispose();
                    this.writer = null;
                }
            }
        }

        private void Serialize(ODataPayloadElement rootElement)
        {
            ExceptionUtilities.CheckAllRequiredDependencies(this);
            new SerializeODataPayloadElementVisitor(this.writer, this).Serialize(rootElement);
        }

        /// <summary>
        /// Visitor for serializing an ODataPayloadElement to verbose JSON.
        /// </summary>
        private sealed class SerializeODataPayloadElementVisitor : IODataPayloadElementVisitor
        {
            private readonly JsonWriter writer;
            private readonly PayloadElementToJsonConverter parent;
            private bool isRootElement = true;

            /// <summary>
            /// Initializes a new instance of the SerializeODataPayloadElementVisitor class
            /// </summary>
            /// <param name="writer">Json writer</param>
            /// <param name="parent">The parent converter.</param>
            internal SerializeODataPayloadElementVisitor(JsonWriter writer, PayloadElementToJsonConverter parent)
            {
                this.writer = writer;
                this.parent = parent;
            }

            /// <summary>
            /// Serialize the current payload element
            /// </summary>
            /// <param name="rootElement">Element to be converted</param>            
            public void Serialize(ODataPayloadElement rootElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(rootElement, "rootElement");
                this.isRootElement = true;
                this.Recurse(rootElement);
            }

            /// <summary>
            /// Visits a payload element whose root is a BatchRequestPayload.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(BatchRequestPayload payloadElement)
            {
                throw new TaupoNotSupportedException("Batch payloads are not supported");
            }

            /// <summary>
            /// Visits a payload element whose root is a BatchResponsePayload.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(BatchResponsePayload payloadElement)
            {
                throw new TaupoNotSupportedException("Batch payloads are not supported");
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ComplexInstance payloadElement)
            {
                this.isRootElement = false;
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                if (payloadElement.IsNull)
                {
                    this.writer.WriteNull();
                }
                else
                {
                    this.writer.StartObjectScope();

                    if (payloadElement.FullTypeName != null)
                    {                        
                        this.writer.WriteName("__metadata");
                        this.writer.StartObjectScope();
                        this.writer.WriteName("type");
                        this.writer.WriteString(payloadElement.FullTypeName);
                        this.writer.EndScope();
                    }

                    foreach (var property in payloadElement.Properties)
                    {
                        this.Recurse(property);
                    }

                    this.writer.EndScope();
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexInstanceCollection.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ComplexInstanceCollection payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
                ExceptionUtilities.Assert(this.isRootElement, "Complex collection is only supported as the root element");
                this.isRootElement = false;

                this.writer.StartArrayScope();
                foreach (var item in payloadElement)
                {
                    this.Recurse(item);
                }

                this.writer.EndScope();
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexMultiValue.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ComplexMultiValue payloadElement)
            {
                this.writer.StartArrayScope();
                foreach (var item in payloadElement)
                {
                    this.Recurse(item);
                }

                this.writer.EndScope();
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexCollectionProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ComplexMultiValueProperty payloadElement)
            {
                bool needsWrapping = this.isRootElement;
                if (needsWrapping)
                {
                    this.isRootElement = false;
                    this.writer.StartObjectScope();
                }

                this.writer.WriteName(payloadElement.Name);

                // if an annotation is present that specifies not to use the wrapper, don't
                bool useResultsWrapper = !payloadElement.Value.Annotations.OfType<JsonCollectionResultWrapperAnnotation>().Any(a => !a.Value);
                if (useResultsWrapper)
                {
                    this.writer.StartObjectScope();

                    if (payloadElement.Value.FullTypeName != null)
                    {
                        this.writer.WriteName("__metadata");
                        this.writer.StartObjectScope();
                        this.writer.WriteName("type");
                        this.writer.WriteString(payloadElement.Value.FullTypeName);
                        this.writer.EndScope();
                    }

                    this.writer.WriteName("results");
                }

                if (payloadElement.Value.IsNull)
                {
                    this.writer.WriteNull();
                }
                else
                {
                    this.Recurse(payloadElement.Value);
                }

                if (useResultsWrapper)
                {
                    this.writer.EndScope();
                }

                if (needsWrapping)
                {
                    this.writer.EndScope();
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ComplexProperty payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
                bool needsWrapping = this.isRootElement;
                this.isRootElement = false;
                if (needsWrapping)
                {
                    this.writer.StartObjectScope();                    
                }

                if (!string.IsNullOrEmpty(payloadElement.Name))
                {
                    this.writer.WriteName(payloadElement.Name);
                }

                this.Recurse(payloadElement.Value);

                if (needsWrapping)
                {
                    this.writer.EndScope();
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a DeferredLink.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(DeferredLink payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
                bool needsWrapping = this.isRootElement;
                this.isRootElement = false;
                if (needsWrapping)
                {                    
                    this.writer.StartObjectScope();
                    this.writer.WriteName("uri");
                    this.writer.WriteString(payloadElement.UriString);
                }
                else
                {
                    this.writer.StartObjectScope();

                    // __deferred is used for payloads generated by the server and sent to the client
                    // __metadata is used if the client wants to update the navigation property
                    // We are always using __metadata because the test framework is only used to generate                    
                    // a payload on the client and sent them to the server.
                    this.writer.WriteName("__metadata");
                    this.writer.StartObjectScope();
                    this.writer.WriteName("uri");
                    this.writer.WriteString(payloadElement.UriString);
                    this.writer.EndScope();
                    this.writer.EndScope();
                }

                if (needsWrapping)
                {
                    this.writer.EndScope();
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a EmptyCollectionProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(EmptyCollectionProperty payloadElement)
            {
                bool needsWrapping = this.isRootElement;
                if (needsWrapping)
                {
                    this.isRootElement = false;
                    this.writer.StartObjectScope();
                }

                this.writer.WriteName(payloadElement.Name);
                this.Recurse(payloadElement.Value);

                if (needsWrapping)
                {
                    this.writer.EndScope();
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a EmptyPayload.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(EmptyPayload payloadElement)
            {
                throw new TaupoNotSupportedException("Json serialization does not allow element of type: " + payloadElement.ElementType.ToString());
            }

            /// <summary>
            /// Visits a payload element whose root is a EmptyUntypedCollection.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(EmptyUntypedCollection payloadElement)
            {
                this.isRootElement = false;
                this.writer.StartArrayScope();
                this.writer.EndScope();
            }

            /// <summary>
            /// Visits a payload element whose root is a EntityInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(EntityInstance payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
                this.isRootElement = false;

                if (payloadElement.IsNull)
                {
                    this.writer.WriteNull();
                }
                else
                {
                    this.writer.StartObjectScope();

                    this.WriteMetadataProperty(payloadElement);

                    foreach (var property in payloadElement.Properties)
                    {
                        this.Recurse(property);
                    }

                    this.writer.EndScope();
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a EntitySetInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(EntitySetInstance payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
                this.isRootElement = false;
                this.writer.StartArrayScope();
                for (int i = 0; i < payloadElement.Count(); i++)
                {
                    payloadElement[i].Accept(this);
                }

                this.writer.EndScope();
            }

            /// <summary>
            /// Visits a payload element whose root is a ExpandedLink.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ExpandedLink payloadElement)
            {
                this.isRootElement = false;
                if (payloadElement.ExpandedElement == null)
                {
                    this.writer.WriteNull();
                }
                else
                {
                    payloadElement.ExpandedElement.Accept(this);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a LinkCollection.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(LinkCollection payloadElement)
            {
                this.isRootElement = false;
                this.writer.StartArrayScope();
                for (int i = 0; i < payloadElement.Count(); i++)
                {
                    payloadElement[i].Accept(this);
                }

                this.writer.EndScope();
            }

            /// <summary>
            /// Visits a payload element whose root is a NamedStreamProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(NamedStreamInstance payloadElement)
            {
                throw new TaupoNotSupportedException("Json serialization does not allow element of type: " + payloadElement.ElementType.ToString());
            }

            /// <summary>
            /// Visits a payload element whose root is a NavigationProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(NavigationPropertyInstance payloadElement)
            {
                this.isRootElement = false;
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
                if (payloadElement.Name != null)
                {                    
                    this.writer.WriteName(payloadElement.Name);
                }

                if (payloadElement.Value == null)
                {
                    this.writer.WriteNull();
                }
                else
                {
                    this.Recurse(payloadElement.Value);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a NullPropertyInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(NullPropertyInstance payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
                bool needsWrapping = this.isRootElement;
                this.isRootElement = false;
                if (needsWrapping)
                {
                    this.writer.StartObjectScope();
                }

                if (!string.IsNullOrEmpty(payloadElement.Name))
                {
                    this.writer.WriteName(payloadElement.Name);
                }
                
                this.writer.WriteNull();

                if (needsWrapping)
                {
                    this.writer.EndScope();
                }
            }

            /// <summary>
            /// Visits a payload element whose root is an ODataErrorPayload.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ODataErrorPayload payloadElement)
            {
                throw new TaupoNotSupportedException("Not supported yet");
            }

            /// <summary>
            /// Visits a payload element whose root is an ODataInternalExceptionPayload.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ODataInternalExceptionPayload payloadElement)
            {
                throw new TaupoNotSupportedException("Not supported yet");
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveCollection.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(PrimitiveCollection payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
                ExceptionUtilities.Assert(this.isRootElement, "Primitive collection is only supported as the root element");
                this.isRootElement = false;

                this.writer.StartArrayScope();
                foreach (var item in payloadElement)
                {
                    this.Recurse(item);
                }

                this.writer.EndScope();
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveMultiValue.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(PrimitiveMultiValue payloadElement)
            {
                this.writer.StartArrayScope();
                foreach (var item in payloadElement)
                {
                    this.Recurse(item);
                }

                this.writer.EndScope();
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveCollectionProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(PrimitiveMultiValueProperty payloadElement)
            {
                bool needsWrapping = this.isRootElement;
                if (needsWrapping)
                {
                    this.isRootElement = false;
                    this.writer.StartObjectScope();
                }

                this.writer.WriteName(payloadElement.Name);

                // if an annotation is present that specifies not to use the wrapper, don't
                bool useResultsWrapper = !payloadElement.Value.Annotations.OfType<JsonCollectionResultWrapperAnnotation>().Any(a => !a.Value);
                if (useResultsWrapper)
                {
                    this.writer.StartObjectScope();

                    if (payloadElement.Value.FullTypeName != null)
                    {
                        this.writer.WriteName("__metadata");
                        this.writer.StartObjectScope();
                        this.writer.WriteName("type");
                        this.writer.WriteString(payloadElement.Value.FullTypeName);
                        this.writer.EndScope();
                    }

                    this.writer.WriteName("results");
                }

                if (payloadElement.Value.IsNull)
                {
                    this.writer.WriteNull();
                }
                else
                {
                    this.Recurse(payloadElement.Value);
                }

                if (useResultsWrapper)
                {
                    this.writer.EndScope();
                }

                if (needsWrapping)
                {
                    this.writer.EndScope();
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(PrimitiveProperty payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
                bool needsWrapping = this.isRootElement;
                this.isRootElement = false;
                if (needsWrapping)
                {
                    this.writer.StartObjectScope();
                }

                this.writer.WriteName(payloadElement.Name);

                this.Recurse(payloadElement.Value);

                if (needsWrapping)
                {
                    this.writer.EndScope();
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveValue.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(PrimitiveValue payloadElement)
            {               
                if (payloadElement.IsNull)
                {
                    this.writer.WriteNull();
                }
                else
                {
                    JsonObject jsonObject;
                    if (this.parent.SpatialConverter.TryConvertIfSpatial(payloadElement, out jsonObject))
                    {
                        this.writer.WriteJsonValue(jsonObject, o => this.parent.PrimitiveConverter.SerializePrimitive(o));
                    }
                    else
                    {
                        var stringValue = this.parent.PrimitiveConverter.SerializePrimitive(payloadElement.ClrValue);
                        payloadElement.Annotations.Add(new RawTextPayloadElementRepresentationAnnotation() { Text = stringValue });
                        this.writer.WriteRaw(stringValue);
                    }
                } 
                
                this.isRootElement = false;
            }

            /// <summary>
            /// Visits a payload element whose root is a ResourceCollectionInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ResourceCollectionInstance payloadElement)
            {
                ExceptionUtilities.Assert(!this.isRootElement, "Json serialization does not allow a root element of type: {0}", payloadElement.ElementType.ToString());

                // NOTE: Following the ODataLib approach here where the Href is used as the entity set name in JSON.
                //       The Title is only available in ATOM through ATOM metadata.
                this.writer.WriteString(payloadElement.Href);
            }

            /// <summary>
            /// Visits a payload element whose root is a ServiceDocumentInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ServiceDocumentInstance payloadElement)
            {
                ExceptionUtilities.Assert(this.isRootElement, "Json serialization allows service document elements only at the root level");
                this.isRootElement = false;

                // NOTE nothing to do here for now since both Astoria and ODataLib only support a single workspace per service document
                ExceptionUtilities.Assert(payloadElement.Workspaces.Count == 1, "Exactly one workspace expected for the service document.");

                foreach (WorkspaceInstance workspace in payloadElement.Workspaces)
                {
                    this.Recurse(workspace);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is an ServiceOperationDescriptor.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ServiceOperationDescriptor payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                var serviceOperationPayloadDescriptor = payloadElement as ServiceOperationDescriptor;
                ExceptionUtilities.CheckArgumentNotNull(serviceOperationPayloadDescriptor, "serviceOperationPayloadDescriptor");
                ExceptionUtilities.CheckObjectNotNull(serviceOperationPayloadDescriptor.Metadata, "serviceOperationPayloadDescriptor.Metadata cannot be null");

                this.writer.StartObjectScope();

                if (serviceOperationPayloadDescriptor.Title != null)
                {
                    this.writer.WriteName("title");
                    this.writer.WriteString(serviceOperationPayloadDescriptor.Title);
                }

                if (serviceOperationPayloadDescriptor.Target != null)
                {
                    this.writer.WriteName("target");
                    this.writer.WriteString(serviceOperationPayloadDescriptor.Target);
                }

                this.writer.EndScope();
            }

            /// <summary>
            /// Visits a payload element whose root is a WorkspaceInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(WorkspaceInstance payloadElement)
            {
                ExceptionUtilities.Assert(!this.isRootElement, "Json serialization does not allow a root element of type: {0}", payloadElement.ElementType.ToString());

                this.writer.StartObjectScope();

                this.writer.WriteName("EntitySets");

                this.writer.StartArrayScope();

                if (payloadElement.ResourceCollections != null)
                {
                    foreach (ResourceCollectionInstance collection in payloadElement.ResourceCollections)
                    {
                        this.Recurse(collection);
                    }
                }

                this.writer.EndScope();

                this.writer.EndScope();
            }

            private void WriteMetadataProperty(EntityInstance payloadElement)
            {
                if (payloadElement.ETag == null && payloadElement.FullTypeName == null && payloadElement.Id == null)
                {
                    return;
                }

                this.writer.WriteName("__metadata");
                this.writer.StartObjectScope();

                if (payloadElement.Id != null)
                {
                    this.writer.WriteName("uri");
                    this.writer.WriteString(payloadElement.Id);
                }

                if (payloadElement.FullTypeName != null)
                {
                    this.writer.WriteName("type");
                    this.writer.WriteString(payloadElement.FullTypeName);
                }

                if (payloadElement.ETag != null)
                {
                    this.writer.WriteName("etag");
                    this.writer.WriteString(payloadElement.ETag);
                }

                this.WriteServiceOperationDescriptorAnnotations("actions", payloadElement.ServiceOperationDescriptors.Where(s => s.IsAction));

                this.WriteServiceOperationDescriptorAnnotations("functions", payloadElement.ServiceOperationDescriptors.Where(s => s.IsFunction));

                this.writer.EndScope();
            }

            private void WriteServiceOperationDescriptorAnnotations(string collectionName, IEnumerable<ServiceOperationDescriptor> serviceOperationPayloadRelAnnotations)
            {
                if (serviceOperationPayloadRelAnnotations.Any())
                {
                    var groups = serviceOperationPayloadRelAnnotations.GroupBy(o => o.Metadata);

                    this.writer.WriteName(collectionName);
                    this.writer.StartObjectScope();

                    foreach (var relGroup in groups)
                    {
                        this.writer.WriteName(relGroup.First().Metadata);

                        this.writer.StartArrayScope();

                        foreach (var serviceOperation in relGroup)
                        {
                            this.Recurse(serviceOperation);
                        }

                        this.writer.EndScope();
                    }

                    this.writer.EndScope();
                }
            }

            private void Recurse(ODataPayloadElement element)
            {
                ExceptionUtilities.CheckArgumentNotNull(element, "element");
                var preformatted = element.Annotations.OfType<RawTextPayloadElementRepresentationAnnotation>().SingleOrDefault();
                if (preformatted != null)
                {
                    this.writer.WriteRaw(preformatted.Text);
                }
                else
                {
                    element.Accept(this);
                }
            }
        }
    }
}