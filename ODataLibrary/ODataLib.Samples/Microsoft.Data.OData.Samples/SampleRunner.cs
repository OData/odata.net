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

using System;
using Microsoft.Data.Edm;
using System.IO;

namespace Microsoft.Data.OData.Samples
{
    class SampleRunner
    {
        static void Main(string[] args)
        {
            if (!Directory.Exists(@".\out"))
            {
                Directory.CreateDirectory(@".\out");
            }

            SampleRunner.RunResponseSamples();

            SampleRunner.RunHTTPRequestSamples();

            SampleRunner.RunHTTPResponseSamples();
        }

        private static void RunHTTPRequestSamples()
        {
            HTTPRequestSamples samples = new HTTPRequestSamples();
            string atomFormat = "application/atom+xml";
            string jsonFormat = "application/json";
            string xmlFormat = "application/xml";
            string baseUri = "http://localhost:54530/NorthwindEdmx.svc/";
            ODataVersion baseVersion = ODataVersion.V3;
            ODataVersion maxVersion = ODataVersion.V3;

            try
            {
                samples.BasicGetRequest(baseUri, xmlFormat, baseVersion, maxVersion, "XmlGetServiceDocument");
                samples.BasicGetRequest(baseUri, jsonFormat, baseVersion, maxVersion, "JsonGetServiceDocument");
                samples.BasicGetRequest(baseUri + "Customers?$inlinecount=allpages", atomFormat, baseVersion, maxVersion, "AtomGetCustomers");
                samples.BasicGetRequest(baseUri + "Customers?$inlinecount=allpages", jsonFormat, baseVersion, maxVersion, "JsonGetCustomers");

                samples.InsertEntityRequest(baseUri, baseUri + "Customers", ODataFormat.Atom, baseVersion, maxVersion, "AtomInsertEntity");
                samples.DeleteEntityRequest(baseUri + "Customers('01%20%20%20')", baseVersion, maxVersion, "DeleteEntity");
                samples.InsertEntityRequest(baseUri, baseUri + "Customers", ODataFormat.Json, baseVersion, maxVersion, "JsonInsertEntity");
                samples.DeleteEntityRequest(baseUri + "Customers('01%20%20%20')", baseVersion, maxVersion, "DeleteEntity");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void RunHTTPResponseSamples()
        {
            HTTPResponseSamples samples = new HTTPResponseSamples();
            FileResponseSamples fileResponseSamples = new FileResponseSamples();
            string atomFormat = "application/atom+xml";
            string jsonFormat = "application/json";
            string baseUri = "http://services.odata.org/Northwind/Northwind.svc/";
            string netflixBaseUri = "http://odata.netflix.com/v2/Catalog/";
            string stackoverflowBaseUri = "http://data.stackexchange.com/stackoverflow/atom/";
            ODataVersion baseVersion = ODataVersion.V2;
            ODataVersion maxVersion = ODataVersion.V3;

            try
            {
                samples.GetResponse(baseUri + "Customers?$inlinecount=allpages", atomFormat, baseVersion, maxVersion, null, "AtomResponseNoMetadataCustomers");
                samples.GetResponse(baseUri + "Orders?$expand=Customer&$top=10", atomFormat, baseVersion, maxVersion, null, "AtomResponseNoMetadataOrdersWithCustomer");

                IEdmModel model = samples.GetMetadata(baseUri + "$metadata");
                samples.GetResponse(baseUri + "Customers?$inlinecount=allpages", atomFormat, baseVersion, maxVersion, model, "AtomResponseWithMetadataCustomers");
                samples.GetResponse(baseUri + "Orders?$expand=Customer&$top=10", atomFormat, baseVersion, maxVersion, model, "AtomResponseWithMetadataOrdersWithCustomer");
                samples.GetResponse(baseUri + "Customers?$inlinecount=allpages", jsonFormat, baseVersion, maxVersion, model, "JsonResponseWithMetadataCustomers");
                samples.GetResponse(baseUri + "Orders?$expand=Customer&$top=10", jsonFormat, baseVersion, maxVersion, model, "JsonResponseWithMetadataOrdersWithCustomer");
                fileResponseSamples.WriteMetadata(model, baseVersion, "NorthwindMetadata");

                model = samples.GetMetadata(netflixBaseUri + "$metadata");
                samples.GetResponse(netflixBaseUri + "Titles?$top=10", atomFormat, baseVersion, maxVersion, model, "AtomResponseWithMetadataNetflixTitles");
                samples.GetResponse(netflixBaseUri + "Titles?$top=10", jsonFormat, baseVersion, maxVersion, model, "JsonResponseWithMetadataNetflixTitles");

                fileResponseSamples.WriteMetadata(model, baseVersion, "NetflixMetadata");

                model = samples.GetMetadata(stackoverflowBaseUri + "$metadata");
                samples.GetResponse(stackoverflowBaseUri + "Posts?$top=10", atomFormat, baseVersion, maxVersion, model, "AtomStackExchange");

                fileResponseSamples.WriteMetadata(model, baseVersion, "StackOverflowMetadata");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void RunResponseSamples()
        {
            FileResponseSamples samples = new FileResponseSamples();
            ODataVersion baseVersion = ODataVersion.V2;

            samples.WriteEntry(ODataFormat.Atom, baseVersion, "AtomWriteEntry");
            samples.WriteEntry(ODataFormat.Json, baseVersion, "JsonWriteEntry");
            samples.WriteEntryWithAnnotations(ODataFormat.Atom, baseVersion, "AtomWriteEntryWithAnnotations");
            samples.WriteEntryWithAnnotations(ODataFormat.Json, baseVersion, "JsonWriteEntryWithAnnotations");
            samples.WriteFeedWithExpandedCollections(ODataFormat.Atom, baseVersion, "AtomWriteFeedWithExpandedCollection");
            samples.WriteFeedWithExpandedCollections(ODataFormat.Json, baseVersion, "JsonWriteFeedWithExpandedCollection");
            samples.WriteFeedWithExpandedReference(ODataFormat.Atom, baseVersion, "AtomWriteFeedWithExpandedReference");
            samples.WriteFeedWithExpandedReference(ODataFormat.Json, baseVersion, "JsonWriteFeedWithExpandedReference");
            samples.WriteProperty(ODataFormat.Atom, baseVersion, "XmlWriteProperty");
            samples.WriteProperty(ODataFormat.Json, baseVersion, "JsonWriteProperty");
            samples.WriteCollection(ODataFormat.Atom, baseVersion, "XmlWritePrimitiveCollection");
            samples.WriteCollection(ODataFormat.Json, baseVersion, "JsonWritePrimitiveCollection");
            samples.WriteComplexCollection(ODataFormat.Atom, baseVersion, "XmlWriteComplexCollection");
            samples.WriteComplexCollection(ODataFormat.Json, baseVersion, "JsonWriteComplexCollection");
            samples.WriteError(ODataFormat.Atom, baseVersion, "XmlWriteError");
            samples.WriteError(ODataFormat.Json, baseVersion, "JsonWriteError");
            samples.WriteBatch(ODataFormat.Atom, baseVersion, "AtomWriteBatch");
            samples.WriteBatch(ODataFormat.Json, baseVersion, "JsonWriteBatch");
            samples.WriteServiceDocument(ODataFormat.Atom, baseVersion, "AtomWriteServiceDocument");
            samples.WriteServiceDocument(ODataFormat.Json, baseVersion, "JsonWriteServiceDocument");
        }
    }
}
