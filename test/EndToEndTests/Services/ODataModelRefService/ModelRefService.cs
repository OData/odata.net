//---------------------------------------------------------------------
// <copyright file="ModelRefService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Text;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    /// <summary>
    /// The class implements an OData service using WCF as the host
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single)]
    public class ModelRefService : ODataService<ModelRefSvcDataSource>
    {
        public override Stream ExecuteGetQuery()
        {
            DataSourceManager.EnsureCurrentDataSource<ModelRefSvcDataSource>();
            DataSourceManager.GetCurrentDataSource<ModelRefSvcDataSource>().ResetEdmModel();

            // TODO: temp work around to get IEdmModel from server, since ODL read API ReadMetadataDocument does not work.
            var requestUri = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri;
            if (requestUri.AbsoluteUri.EndsWith(".csdl"))
            {
                var fileName = requestUri.AbsoluteUri.Substring(requestUri.AbsoluteUri.LastIndexOf('/') + 1);

                var testAssembly = Assembly.GetAssembly(typeof(ModelRefService));
                string fullResourceName = testAssembly.GetManifestResourceNames().Single(n => n.EndsWith(fileName));
                Stream resourceStream = testAssembly.GetManifestResourceStream(fullResourceName);

                var reader = new StreamReader(resourceStream);
                string str = reader.ReadToEnd();
                str = str.Replace("[ServiceBaseUrl]", ServiceConstants.ServiceBaseUri.AbsoluteUri);
                byte[] byteArray = Encoding.UTF8.GetBytes(str);

                WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml";

                MemoryStream stream = new MemoryStream(byteArray);
                return stream;
            }

            return base.ExecuteGetQuery();
        }
    }
}
