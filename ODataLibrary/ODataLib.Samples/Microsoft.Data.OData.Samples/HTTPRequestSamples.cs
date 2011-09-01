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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.OData.Samples.Messages;
using Microsoft.Data.OData.Samples.Services.Data;

namespace Microsoft.Data.OData.Samples
{
    class HTTPRequestSamples
    {
        private NorthwindData dataSource = new NorthwindData();

        public void BasicGetRequest(string uri, string format, ODataVersion version, ODataVersion maxVersion, string filename)
        {
            HTTPClientRequestMessage message = new HTTPClientRequestMessage(uri);
            message.SetHeader("Accept", format);
            message.SetHeader("DataServiceVersion", version.ToHeaderValue());
            message.SetHeader("MaxDataServiceVersion", maxVersion.ToHeaderValue());

            HTTPRequestSamples.WriteResponseToFile(message.GetResponse() as HTTPClientResponseMessage, filename);
        }

        public void DeleteEntityRequest(string uri, ODataVersion version, ODataVersion maxVersion, string filename)
        {
            HTTPClientRequestMessage message = new HTTPClientRequestMessage(uri);
            message.Method = HttpMethod.Delete;
            message.SetHeader("DataServiceVersion", version.ToHeaderValue());
            message.SetHeader("MaxDataServiceVersion", maxVersion.ToHeaderValue());

            HTTPRequestSamples.WriteResponseToFile(message.GetResponse() as HTTPClientResponseMessage, filename);
        }

        public void UpdatePropertyRequest(string baseUri, string uri, ODataFormat formatKind, ODataVersion version, ODataVersion maxVersion, string filename)
        {
            HTTPClientRequestMessage message = new HTTPClientRequestMessage(uri);
            message.SetHeader("Accept", formatKind == ODataFormat.Json ? "application/json" : "application/atom+xml");
            message.Method = HttpMethod.Merge;
            message.SetHeader("MaxDataServiceVersion", maxVersion.ToHeaderValue());

            // create the writer, indent for readability of the examples.
            ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings() { Indent = true, CheckCharacters = false, BaseUri = new Uri(baseUri), Version = version };
            writerSettings.SetContentType(formatKind);
            ODataMessageWriter messageWriter = new ODataMessageWriter(message, writerSettings);

            var writerTask = messageWriter.CreateODataEntryWriterAsync();
            writerTask.Wait();
        }

        public void InsertEntityRequest(string baseUri, string uri, ODataFormat formatKind, ODataVersion version, ODataVersion maxVersion, string filename)
        {
            HTTPClientRequestMessage message = new HTTPClientRequestMessage(uri);
            message.SetHeader("Accept", formatKind == ODataFormat.Json ? "application/json" : "application/atom+xml");
            message.Method = HttpMethod.Post;
            message.SetHeader("MaxDataServiceVersion", maxVersion.ToHeaderValue());

            // create the writer, indent for readability of the examples.
            ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings() { Indent = true, CheckCharacters = false, BaseUri = new Uri(baseUri), Version = version };
            writerSettings.SetContentType(formatKind);
            using (ODataMessageWriter messageWriter = new ODataMessageWriter(message, writerSettings))
            {
                ODataWriter writer = messageWriter.CreateODataEntryWriter();

                // start the entry
                writer.WriteStart(new ODataEntry()
                {
                    // the edit link is relative to the baseUri set on the writer in the case
                    EditLink = new Uri("/Customers('" + dataSource.Customers.First().CustomerID + "')", UriKind.Relative),
                    Id = "Customers('" + dataSource.Customers.First().CustomerID + "')",
                    TypeName = "NORTHWNDModel.Customer",
                    Properties = new List<ODataProperty>(){
                        new ODataProperty(){ Name = "CustomerID", Value = dataSource.Customers.First().CustomerID },
                        new ODataProperty(){ Name = "CompanyName", Value = dataSource.Customers.First().CompanyName },
                        new ODataProperty(){ Name = "ContactName", Value = dataSource.Customers.First().ContactName },
                        new ODataProperty(){ Name = "ContactTitle", Value = dataSource.Customers.First().ContactTitle }
                    }
                });

                writer.WriteEnd();

                writer.Flush();
            }

            HTTPRequestSamples.WriteResponseToFile(message.GetResponse() as HTTPClientResponseMessage, filename);
        }

        public static void WriteResponseToFile(HTTPClientResponseMessage response, string fileName)
        {
            string filePath = @".\out\" + fileName + ".txt";

            var streamTask = response.GetStreamAsync();
            streamTask.Wait();
            using (FileStream output = new FileStream(filePath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(output))
                {
                    writer.WriteLine(response.StatusCode + " " + response.StatusDescription);
                    foreach (var q in response.Headers)
                    {
                        writer.WriteLine(q.Key + ": " + q.Value);
                    }

                    writer.WriteLine();

                    using (StreamReader reader = new StreamReader(streamTask.Result))
                    {
                        writer.Write(reader.ReadToEnd());
                    }
                }
            }
        }
    }
}
