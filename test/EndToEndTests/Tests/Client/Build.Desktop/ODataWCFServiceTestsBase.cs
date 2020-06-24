//---------------------------------------------------------------------
// <copyright file="ODataWCFServiceTestsBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.OData.Framework.Server;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Xunit;

    /// <summary>
    /// Base class for tests that use the service implemented using ODataLib and EDMLib.
    /// </summary>
    public class ODataWCFServiceTestsBase<TClientContext> where TClientContext : Microsoft.OData.Client.DataServiceContext
    {
        private readonly ServiceDescriptor serviceDescriptor;
        protected IServiceWrapper TestServiceWrapper;
        protected TClientContext TestClientContext;

        public IEdmModel Model
        {
            get;
            protected set;
        }

        public ODataWCFServiceTestsBase(ServiceDescriptor serviceDescriptor)
        {
            TestServiceUtil.ServiceUriGenerator = ServiceGeneratorFactory.CreateServiceUriGenerator();
            this.serviceDescriptor = serviceDescriptor;

            TestServiceWrapper = new WCFServiceWrapper(this.serviceDescriptor);
            TestServiceWrapper.StartService();
            RetrieveServiceEdmModel();
            TestClientContext = Activator.CreateInstance(typeof(TClientContext), ServiceBaseUri) as TClientContext;
            ResetDataSource();
        }

        /// <summary>
        /// Gets the base URI for the test OData Service.
        /// </summary>
        protected Uri ServiceBaseUri
        {
            get { return new Uri(TestServiceWrapper.ServiceUri.AbsoluteUri.TrimEnd('/') + "/"); }
        }

        protected readonly string[] mimeTypes = new string[]
        {
            //MimeTypes.ApplicationAtomXml,
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
        };

        private void ResetDataSource()
        {
            var ar = TestClientContext.BeginExecute(new Uri("ResetDataSource/", UriKind.Relative), null, null, "POST");
            ar.AsyncWaitHandle.WaitOne();
        }

        public void Dispose()
        {
            TestServiceWrapper.StopService();
        }
        private Stream GetStreamFromUrl(string absoluteUri)
        {
            HttpWebRequestMessage message = new HttpWebRequestMessage(new Uri(absoluteUri, UriKind.Absolute));
            return message.GetResponse().GetStream();
        }

        /// <summary>
        /// Stores the metadata document from the test service as an IEdmModel
        /// </summary>
        private void RetrieveServiceEdmModel()
        {
            HttpWebRequestMessage message = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "$metadata", UriKind.Absolute));
            message.SetHeader("Accept", MimeTypes.ApplicationXml);

            using (var messageReader = new ODataMessageReader(message.GetResponse()))
            {
                Func<Uri, XmlReader> getReferencedSchemaFunc = uri =>
                {
                    return XmlReader.Create(GetStreamFromUrl(uri.AbsoluteUri));
                };
                Model = messageReader.ReadMetadataDocument(getReferencedSchemaFunc);
            }
            //$metadata request failed
            Assert.NotNull(Model.EntityContainer);
        }
    }
}
