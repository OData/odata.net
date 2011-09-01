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

using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.Data.Edm;
using System.IO;
using Microsoft.Data.OData.Samples.Messages;

namespace Microsoft.Data.OData.Samples
{
    class HTTPResponseSamples
    {
        private IndentedTextWriter writer;

        public IEdmModel GetMetadata(string uri)
        {
            HTTPClientRequestMessage message = new HTTPClientRequestMessage(uri);
            message.SetHeader("Accept", "application/xml");
            message.SetHeader("DataServiceVersion", ODataVersion.V1.ToHeaderValue());
            message.SetHeader("MaxDataServiceVersion", ODataVersion.V3.ToHeaderValue());

            using (ODataMessageReader messageReader = new ODataMessageReader(message.GetResponse()))
            {
                return messageReader.ReadMetadataDocument();
            }
        }

        public void GetResponse(string uri, string format, ODataVersion version, ODataVersion maxVersion, IEdmModel model, string fileName)
        {
            HTTPClientRequestMessage message = new HTTPClientRequestMessage(uri);
            message.SetHeader("Accept", format);
            message.SetHeader("DataServiceVersion", version.ToHeaderValue());
            message.SetHeader("MaxDataServiceVersion", maxVersion.ToHeaderValue());

            string filePath = @".\out\" + fileName + ".txt";
            using (StreamWriter outputWriter = new StreamWriter(filePath))
            {
                this.writer = new IndentedTextWriter(outputWriter, "  ");

                using (ODataMessageReader messageReader = new ODataMessageReader(message.GetResponse(), new ODataMessageReaderSettings(), model))
                {
                    ODataReader reader = messageReader.CreateODataFeedReader();
                    this.ReadAndOutputEntryOrFeed(reader);
                }
            }
        }

        private void ReadAndOutputEntryOrFeed(ODataReader reader)
        {
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.FeedStart:
                        {
                            ODataFeed feed = (ODataFeed)reader.Item;
                            this.writer.WriteLine("ODataFeed:");
                            this.writer.Indent++;
                        }

                        break;

                    case ODataReaderState.FeedEnd:
                        {
                            ODataFeed feed = (ODataFeed)reader.Item;
                            if (feed.Count != null)
                            {
                                this.writer.WriteLine("Count: " + feed.Count.ToString());
                            }
                            if (feed.NextPageLink != null)
                            {
                                this.writer.WriteLine("NextPageLink: " + feed.NextPageLink.AbsoluteUri);
                            }

                            this.writer.Indent--;
                        }

                        break;

                    case ODataReaderState.EntryStart:
                        {
                            ODataEntry entry = (ODataEntry)reader.Item;
                            this.writer.WriteLine("ODataEntry:");
                            this.writer.Indent++;
                        }

                        break;

                    case ODataReaderState.EntryEnd:
                        {
                            ODataEntry entry = (ODataEntry)reader.Item;
                            this.writer.WriteLine("TypeName: " + (entry.TypeName ?? "<null>"));
                            this.writer.WriteLine("Id: " + (entry.Id ?? "<null>"));
                            if (entry.ReadLink != null)
                            {
                                this.writer.WriteLine("ReadLink: " + entry.ReadLink.AbsoluteUri);
                            }

                            if (entry.EditLink != null)
                            {
                                this.writer.WriteLine("EditLink: " + entry.EditLink.AbsoluteUri);
                            }

                            if (entry.MediaResource != null)
                            {
                                this.writer.Write("MediaResource: ");
                                this.WriteValue(entry.MediaResource);
                            }

                            this.WriteProperties(entry.Properties);

                            this.writer.Indent--;
                        }

                        break;

                    case ODataReaderState.NavigationLinkStart:
                        {
                            ODataNavigationLink navigationLink = (ODataNavigationLink)reader.Item;
                            this.writer.WriteLine(navigationLink.Name + ": ODataNavigationLink: ");
                            this.writer.Indent++;
                        }

                        break;

                    case ODataReaderState.NavigationLinkEnd:
                        {
                            ODataNavigationLink navigationLink = (ODataNavigationLink)reader.Item;
                            this.writer.WriteLine("Url: " + (navigationLink.Url == null ? "<null>" : navigationLink.Url.AbsoluteUri));
                            this.writer.Indent--;
                        }

                        break;
                }
            }
        }

        private void WriteProperties(IEnumerable<ODataProperty> properties)
        {
            this.writer.WriteLine("Properties:");
            this.writer.Indent++;
            foreach (ODataProperty property in properties)
            {
                this.writer.Write(property.Name + ": ");
                this.WriteValue(property.Value);
            }

            this.writer.Indent--;
        }

        private void WriteValue(object value)
        {
            ODataComplexValue complexValue = value as ODataComplexValue;
            if (complexValue != null)
            {
                this.writer.WriteLine("ODataComplexValue");
                this.writer.Indent++;
                this.writer.WriteLine("TypeName: " + (complexValue.TypeName ?? "<null>"));
                this.WriteProperties(complexValue.Properties);
                this.writer.Indent--;

                return;
            }

            ODataMultiValue multiValue = value as ODataMultiValue;
            if (multiValue != null)
            {
                this.writer.WriteLine("ODataMultiValue");
                this.writer.Indent++;
                this.writer.WriteLine("TypeName: " + (multiValue.TypeName ?? "<null>"));
                this.writer.WriteLine("Items:");
                this.writer.Indent++;
                foreach (object item in multiValue.Items)
                {
                    this.WriteValue(item);
                }

                this.writer.Indent--;
                this.writer.Indent--;

                return;
            }

            ODataStreamReferenceValue streamReferenceValue = value as ODataStreamReferenceValue;
            if (streamReferenceValue != null)
            {
                this.writer.WriteLine("ODataStreamReferenceValue");
                this.writer.Indent++;
                if (streamReferenceValue.ReadLink != null)
                {
                    this.writer.WriteLine("ReadLink: " + streamReferenceValue.ReadLink.AbsoluteUri);
                }

                if (streamReferenceValue.EditLink != null)
                {
                    this.writer.WriteLine("EditLink: " + streamReferenceValue.EditLink.AbsoluteUri);
                }

                this.writer.Indent--;

                return;
            }

            if (value == null)
            {
                this.writer.WriteLine("null");
            }
            else
            {
                this.writer.WriteLine(value.ToString());
            }
        }
    }
}
