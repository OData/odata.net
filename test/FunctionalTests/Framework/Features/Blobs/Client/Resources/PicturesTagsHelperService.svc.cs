//---------------------------------------------------------------------
// <copyright file="PicturesTagsHelperService.svc.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Xml;
using System.Xml.Linq;

namespace PicturesTags {
    [ServiceContract(Namespace = "http://xpista/2009/06/Blob4ClinetHelperService")]
    public interface IHelperService {
        [OperationContract]
        [WebGet(UriTemplate = "StatusCodeResponder/{value}")]
        void StatusCodeResponder(string value);

        [OperationContract]
        [WebGet(UriTemplate = "StreamData")]
        Stream StreamData();

        [OperationContract]
        [WebGet(UriTemplate = "a.xml")]
        Stream AXml();
    }

    [AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Allowed)]
    [Sniffer]
    public class HelperService : IHelperService {
        public void StatusCodeResponder(string value) {
            WebOperationContext.Current.OutgoingResponse.StatusCode = (HttpStatusCode)Int32.Parse(value);
        }

        public Stream StreamData() {
            return new MemoryStream(Encoding.UTF8.GetBytes("Trying web stream, No idea how this will fly ... "));
        }

        public Stream AXml() {
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml";
            string DataDirName = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), string.Concat("BlobClientTestFiles", @"\", System.Web.Hosting.HostingEnvironment.ApplicationID.Split('/', '\\').Last()));
            if (!File.Exists(Path.Combine(DataDirName, "a.xml"))) {
                XElement e = new XElement("root", new XElement("A", "This is a test file for BlobClient test."));
                e.Save(Path.Combine(DataDirName, "a.xml"));
            }
            return File.OpenRead(Path.Combine(DataDirName, "a.xml"));
        }
    }
}
