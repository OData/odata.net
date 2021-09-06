//---------------------------------------------------------------------
// <copyright file="PreexistingModelGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.EntityModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;

    /// <summary>
    /// Model generator for any data service given its ServiceDocument and ServiceMetadataDocument Uri
    /// </summary>
    [ImplementationName(typeof(IModelGenerator), "Preexisting")]
    public class PreexistingModelGenerator : IModelGenerator
    {
        /// <summary>
        /// Gets or sets the CSDL parser to parse the $metadata document of an OData Service
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ICsdlParser CsdlParser { get; set; }

        /// <summary>
        /// Gets or sets the $metadata endpoint uri of an OData Service
        /// </summary>
        [InjectTestParameter("ServiceMetadataDocumentUri")]
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "This is externally injected")]
        public string ServiceMetadataDocumentUri { get; set; }

        /// <summary>
        /// Gets or sets the PayloadElementConverter to parse CSDL from the service's $metadata endpoint
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IXmlToPayloadElementConverter PayloadElementConverter { get; set; }

        /// <summary>
        /// Gets or sets the Http stack used to make requests to the $metadata uri
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IHttpImplementation HttpStack { get; set; }

        /// <summary>
        /// Generates the entity model for a DataService refered to via the ServiceMetadataDocumentUri parameter
        /// </summary>
        /// <returns>An entity model representing the Data Service</returns>
        public EntityModelSchema GenerateModel()
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(this.ServiceMetadataDocumentUri, "Using this ModelGenrator requires setting the ServiceMetadataDocumentUri test parameter");

            HttpRequestData metadataDocumentRequest = new HttpRequestData() { Uri = new Uri(this.ServiceMetadataDocumentUri) };
            HttpResponseData response = this.HttpStack.GetResponse(metadataDocumentRequest);

            var encoding = response.GetEncodingFromHeadersOrDefault();
            string metadataDocumentString = encoding.GetString(response.Body);
            XDocument metadataDocument = XDocument.Parse(metadataDocumentString);
            XElement edmxElement = metadataDocument.Root;

            var metadataPayloadElement = this.PayloadElementConverter.ConvertToPayloadElement(edmxElement) as MetadataPayloadElement;
            ExceptionUtilities.CheckObjectNotNull(metadataPayloadElement, "MetadataPayloadElement should not be null");

            var model = metadataPayloadElement.EntityModelSchema;
            new SetDefaultCollectionTypesFixup().Fixup(model);
            return model;
        }
    }
}
