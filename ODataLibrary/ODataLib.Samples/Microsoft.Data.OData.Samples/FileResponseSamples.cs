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
using Microsoft.Data.Edm;
using System.IO;
using System.Linq;
using Microsoft.Data.OData.Samples.Messages;
using Microsoft.Data.OData.Samples.Services.Data;

namespace Microsoft.Data.OData.Samples
{
    class FileResponseSamples
    {
        private NorthwindData dataSource = new NorthwindData();

        /// <summary>
        /// Writes out a single atom:entry element  into a file
        /// </summary>
        /// <param name="formatKind">The format kind : atom / json</param>
        /// <param name="fileName">The file name to write the output to </param>
        public void WriteEntry(ODataFormat formatKind, ODataVersion version, string fileName)
        {
            string filePath = @".\out\" + fileName + ".txt";
            FileResponseMessage message = new FileResponseMessage(new FileStream(filePath, FileMode.Create));
            message.SetHeader("Server", "Microsoft-IIS/7.0");
            message.StatusCode = 200;

            // create the writer, indent for readability of the examples.
            ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings() { Indent = true, CheckCharacters = false, BaseUri = new Uri("http://serviceRoot"), Version = version };
            messageWriterSettings.SetContentType(formatKind);
            using (ODataMessageWriter messageWriter = new ODataMessageWriter(message, messageWriterSettings))
            {
                ODataWriter writer = messageWriter.CreateODataEntryWriter();

                // start the entry
                writer.WriteStart(new ODataEntry()
                {
                    // the edit link is relative to the baseUri set on the writer in the case
                    EditLink = new Uri("/Customer(" + dataSource.Customers.First().CustomerID + ")", UriKind.Relative),
                    Id = "Customer(\"" + dataSource.Customers.First().CustomerID + "\")",
                    TypeName = "ConsoleSample.Customer",
                    Properties = new List<ODataProperty>(){
                        new ODataProperty(){ Name = "CustomerID", Value = dataSource.Customers.First().CustomerID },
                        new ODataProperty(){ Name = "CompanyName", Value = dataSource.Customers.First().CompanyName },
                        new ODataProperty(){ Name = "ContactName", Value = dataSource.Customers.First().ContactName },
                        new ODataProperty(){ Name = "ContactTitle", Value = dataSource.Customers.First().ContactTitle },
                        new ODataProperty(){ Name = "ContactAddress", Value = new ODataComplexValue() {  
                            Properties = new List<ODataProperty>() { 
                                new ODataProperty(){ Name = "City", Value = dataSource.Customers.First().Address.City },
                                new ODataProperty(){ Name = "PostalCode", Value = dataSource.Customers.First().Address.PostalCode },
                                new ODataProperty(){ Name = "Street", Value = dataSource.Customers.First().Address.Street },
                                }
                            }
                        }
                    }
                });

                writer.WriteStart(new ODataNavigationLink()
                {
                    IsCollection = true,
                    Name = "Orders",
                    Url = new Uri("http://microsoft.com/Customer(" + dataSource.Customers.First().CustomerID + ")/Orders")
                });
                writer.WriteEnd();

                writer.WriteStart(new ODataNavigationLink()
                {
                    IsCollection = true,
                    Name = "Employees",
                    Url = new Uri("http://microsoft.com/Customer(" + dataSource.Customers.First().CustomerID + ")/Employees")
                });
                writer.WriteEnd();

                writer.WriteEnd();

                writer.Flush();
            }
        }

        /// <summary>
        /// Writes out a single atom:entry element with an expanded collection navigation property to a file
        /// </summary>
        /// <param name="formatKind">The format kind : atom / json</param>
        /// <param name="fileName">The file name to write the output to </param>
        public void WriteFeedWithExpandedCollections(ODataFormat formatKind, ODataVersion version, string fileName)
        {
            string filePath = @".\out\" + fileName + ".txt";
            FileResponseMessage message = new FileResponseMessage(new FileStream(filePath, FileMode.Create));
            message.SetHeader("Server", "Microsoft-IIS/7.0");
            message.StatusCode = 200;

            // create the writer, indent for readability of the examples.
            ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings() { Indent = true, CheckCharacters = false, BaseUri = new Uri("http://serviceRoot"), Version = version };
            messageWriterSettings.SetContentType(formatKind);
            using (ODataMessageWriter messageWriter = new ODataMessageWriter(message, messageWriterSettings))
            {
                //create the feed writer
                ODataWriter writer = messageWriter.CreateODataFeedWriter();

                //begin writing the feed. The count can optionally be specified here. An ID is required
                writer.WriteStart(new ODataFeed()
                {
                    //  Count = (version == ODataVersion.V1 ? null : dataSource.Customers.LongCount()),
                    Id = "Customers"
                });

                //loop through the customers in the data source and write out the customer entries
                foreach (Customer c in dataSource.Customers)
                {
                    //create a new entry and start writing
                    ODataEntry e = new ODataEntry();
                    writer.WriteStart(e);

                    //fill in the basic values for the entity
                    e.Id = "urn:Customer(\"" + c.CustomerID + "\")";
                    e.EditLink = new Uri("http://serviceRoot/Customer(\"" + c.CustomerID + "\")", UriKind.RelativeOrAbsolute);
                    e.TypeName = "ConsoleSample.Customer";

                    //add the property values for the entity
                    e.Properties = new List<ODataProperty>(){
                        new ODataProperty(){ Name = "CustomerID", Value = c.CustomerID },
                        new ODataProperty(){ Name = "CompanyName", Value = c.CompanyName },
                        new ODataProperty(){ Name = "ContactName", Value = c.ContactName },
                        new ODataProperty(){ Name = "ContactTitle", Value = c.ContactTitle },
                        new ODataProperty(){ Name = "ContactAddress", Value = new ODataComplexValue() {  
                            Properties = new List<ODataProperty>() { 
                                new ODataProperty(){ Name = "City", Value = c.Address.City },
                                new ODataProperty(){ Name = "PostalCode", Value = c.Address.PostalCode },
                                new ODataProperty(){ Name = "Street", Value = c.Address.Street },
                                }
                            }
                        }
                    };

                    //write an expanded navigation property
                    writer.WriteStart(new ODataNavigationLink()
                    {
                        //fill in the values for the nav prop (Name/Url/etc)
                        IsCollection = true,
                        Name = "Orders",
                        Url = new Uri("http://microsoft.com/Customer(" + c.CustomerID + ")/Orders")
                    });

                    //start writing the inner feed for the expanded Nav prop
                    writer.WriteStart(new ODataFeed()
                    {
                        Id = "urn:Customer(\"" + c.CustomerID + "\")/Orders"
                    });

                    //write an order entry for each order in the relationship Customer->Orders
                    foreach (Order o in c.Orders)
                    {
                        //create an entry for the order
                        ODataEntry entry = new ODataEntry()
                        {
                            EditLink = new Uri("http://microsoft.com/Customer(" + c.CustomerID + ")/Orders(" + o.OrderId + ")"),
                            Id = "Orders(" + o.OrderId + ")",
                            TypeName = "Consolesample.Order",
                            Properties = new List<ODataProperty>(){
                                new ODataProperty() { Value = o.OrderId, Name = "OrderID" },
                                new ODataProperty() { Value = o.OrderDate, Name = "OrderDate" },
                                new ODataProperty() { Value = o.RequiredDate, Name = "Requireddate" },
                                new ODataProperty() { Name = "ShipAddress", Value = new ODataComplexValue() {
                                    Properties = new List<ODataProperty>() {
                                        new ODataProperty() { Value = o.ShipAddress.City, Name = "City" },
                                        new ODataProperty() { Value = o.ShipAddress.PostalCode, Name = "PostalCode" },
                                        new ODataProperty() { Value = o.ShipAddress.Street, Name = "Street" }
                                    },
                                    TypeName = "Consolesample.ShipAddress"                                
                                }
                                }
                            }
                        };
                        writer.WriteStart(entry);

                        //write a non-expanded link in the order for the relationship back to customer
                        writer.WriteStart(new ODataNavigationLink()
                        {
                            IsCollection = false,
                            Name = "Customer",
                            Url = new Uri("http://microsoft.com/Customer(/Orders(" + o.OrderId + ")/Customer")
                        });
                        writer.WriteEnd();

                        //end writing the customer entry
                        writer.WriteEnd();

                        //flush the entry to the stream
                        writer.Flush();
                    }

                    //end writing the feed
                    writer.WriteEnd();

                    //end writing the navigation link Customers->Orders
                    writer.WriteEnd();

                    //write the non-expanded link to the employees navigation property
                    writer.WriteStart(new ODataNavigationLink()
                    {
                        IsCollection = true,
                        Name = "Employees",
                        Url = new Uri("http://microsoft.com/Customer(\"" + c.CustomerID + "\")/Employees")
                    });
                    writer.WriteEnd();

                    //end writing the entry
                    writer.WriteEnd();

                    //flush the entry to the stream
                    writer.Flush();
                }

                //end writing the customers feed
                writer.WriteEnd();
                writer.Flush();
            }
        }

        /// <summary>
        /// Writes out a single atom:entry element with an expanded reference navigation property to a file
        /// </summary>
        /// <param name="formatKind">The format kind : atom / json</param>
        /// <param name="fileName">The file name to write the output to </param>
        public void WriteFeedWithExpandedReference(ODataFormat formatKind, ODataVersion version, string fileName)
        {
            string filePath = @".\out\" + fileName + ".txt";
            FileResponseMessage message = new FileResponseMessage(new FileStream(filePath, FileMode.Create));
            message.SetHeader("Server", "Microsoft-IIS/7.0");
            message.StatusCode = 200;

            // create the writer, indent for readability of the examples.
            ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings() { Indent = true, CheckCharacters = false, BaseUri = new Uri("http://serviceRoot"), Version = version };
            messageWriterSettings.SetContentType(formatKind);
            using (ODataMessageWriter messageWriter = new ODataMessageWriter(message, messageWriterSettings))
            {
                ODataWriter writer = messageWriter.CreateODataFeedWriter();

                writer.WriteStart(new ODataFeed()
                {
                    Count = dataSource.Orders.LongCount(),
                    Id = "Orders"
                });  // <feed>

                foreach (Order topLevelOrder in dataSource.Orders)
                {
                    ODataEntry orderEntry = new ODataEntry()
                    {
                        Id = "urn:Orders(\"" + topLevelOrder.OrderId + "\")",
                        EditLink = new Uri("http://serviceRoot/Orders(\"" + topLevelOrder.OrderId + "\")", UriKind.RelativeOrAbsolute),
                        TypeName = "Consolesample.Order",
                        Properties = new List<ODataProperty>(){
                                new ODataProperty() { Value = topLevelOrder.OrderId, Name = "OrderID" },
                                new ODataProperty() { Value = topLevelOrder.OrderDate, Name = "OrderDate" },
                                new ODataProperty() { Value = topLevelOrder.RequiredDate, Name = "Requireddate" },
                                new ODataProperty() { Name = "ShipAddress", Value = new ODataComplexValue() {
                                    Properties = new List<ODataProperty>() {
                                        new ODataProperty() { Value = topLevelOrder.ShipAddress.City, Name = "City" },
                                        new ODataProperty() { Value = topLevelOrder.ShipAddress.PostalCode, Name = "PostalCode" },
                                        new ODataProperty() { Value = topLevelOrder.ShipAddress.Street, Name = "Street" }
                                    } 
                                    }
                                }
                            }
                    };
                    writer.WriteStart(orderEntry); // <entry>
                    writer.WriteStart(new ODataNavigationLink()
                    {
                        IsCollection = false,
                        Name = "Customer",
                        Url = new Uri(orderEntry.EditLink.OriginalString + "/Customer")
                    }); // <link name='Customer'>

                    Customer relatedCustomer = topLevelOrder.Customer;

                    ODataEntry customerEntry = new ODataEntry()
                    {
                        Id = "urn:Customer(\"" + relatedCustomer.CustomerID + "\")",
                        EditLink = new Uri("http://serviceRoot/Customer(\"" + relatedCustomer.CustomerID + "\")", UriKind.RelativeOrAbsolute),
                        TypeName = "ConsoleSamplCustomer",
                        Properties = new List<ODataProperty>(){
                            new ODataProperty(){ Name = "CustomerID", Value = relatedCustomer.CustomerID },
                            new ODataProperty(){ Name = "CompanyName", Value = relatedCustomer.CompanyName },
                            new ODataProperty(){ Name = "ContactName", Value = relatedCustomer.ContactName },
                            new ODataProperty(){ Name = "ContactTitle", Value = relatedCustomer.ContactTitle },
                            new ODataProperty(){ Name = "ContactAddress", Value = new ODataComplexValue() {  
                                Properties = new List<ODataProperty>() { 
                                    new ODataProperty(){ Name = "City", Value = relatedCustomer.Address.City },
                                    new ODataProperty(){ Name = "PostalCode", Value = relatedCustomer.Address.PostalCode },
                                    new ODataProperty(){ Name = "Street", Value = relatedCustomer.Address.Street },
                                    }
                                }
                            }}
                    };

                    writer.WriteStart(customerEntry); // <entry>  
                    writer.WriteEnd();  //</entry>

                    writer.WriteEnd(); // </link>


                    writer.WriteEnd();// </entry>
                }
                writer.WriteEnd(); // </feed>
                writer.Flush();
            }
        }

        /// <summary>
        /// Writes out a single atom:entry element  with specific atom extension properties into a file
        /// </summary>
        /// <param name="formatKind">The format kind : atom / json</param>
        /// <param name="fileName">The file name to write the output to </param>
        public void WriteEntryWithAnnotations(ODataFormat formatKind, ODataVersion version, string fileName)
        {
            string filePath = @".\out\" + fileName + ".txt";
            FileResponseMessage message = new FileResponseMessage(new FileStream(filePath, FileMode.Create));
            message.SetHeader("Server", "Microsoft-IIS/7.0");
            message.StatusCode = 200;

            // create the writer, indent for readability of the examples.
            ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings() { Indent = true, CheckCharacters = false, BaseUri = new Uri("http://serviceRoot"), Version = version };
            messageWriterSettings.SetContentType(formatKind);
            using (ODataMessageWriter messageWriter = new ODataMessageWriter(message, messageWriterSettings))
            {
                ODataWriter writer = messageWriter.CreateODataEntryWriter();

                //start the entry
                ODataEntry e = new ODataEntry()
                {
                    //the edit link is relative to the baseUri set on the writer in the case
                    EditLink = new Uri("http://serviceRoot/Customer(" + dataSource.Customers.First().CustomerID + ")", UriKind.RelativeOrAbsolute),
                    Id = "Customer(\"" + dataSource.Customers.First().CustomerID + "\")",
                    TypeName = "ConsoleSample.Customer",
                    Properties = new List<ODataProperty>(){
                        new ODataProperty(){ Name = "CustomerID", Value = dataSource.Customers.First().CustomerID },
                        new ODataProperty(){ Name = "CompanyName", Value = dataSource.Customers.First().CompanyName },
                        new ODataProperty(){ Name = "ContactName", Value = dataSource.Customers.First().ContactName },
                        new ODataProperty(){ Name = "ContactTitle", Value = dataSource.Customers.First().ContactTitle },
                        new ODataProperty(){ Name = "ContactAddress", Value = new ODataComplexValue() {  
                            Properties = new List<ODataProperty>() { 
                                new ODataProperty(){ Name = "City", Value = dataSource.Customers.First().Address.City },
                                new ODataProperty(){ Name = "PostalCode", Value = dataSource.Customers.First().Address.PostalCode },
                                new ODataProperty(){ Name = "Street", Value = dataSource.Customers.First().Address.Street },
                                }
                            }
                        }
                    }
                };
                writer.WriteStart(e);

                //e.Atom().Title.Text = dataSource.Customers.First().CompanyName;
                //e.Atom().Title.Kind = AtomTextConstructKind.Text;
                //e.Atom().Updated = dataSource.Customers.First().ModifiedDate;


                writer.WriteStart(new ODataNavigationLink()
                {
                    IsCollection = true,
                    Name = "Orders",
                    Url = new Uri("http://microsoft.com/Customer(" + dataSource.Customers.First().CustomerID + ")/Orders")
                });
                writer.WriteEnd();

                writer.WriteStart(new ODataNavigationLink()
                {
                    IsCollection = true,
                    Name = "Employees",
                    Url = new Uri("http://microsoft.com/Customer(" + dataSource.Customers.First().CustomerID + ")/Employees")
                });
                writer.WriteEnd();

                writer.WriteEnd();

                writer.Flush();
            }
        }

        /// <summary>
        /// Generates a payload for a single property
        /// </summary>
        /// <param name="formatKind">The format kind, can be either json or xml but not atom</param>
        /// <param name="fileName">output file</param>
        public void WriteProperty(ODataFormat formatKind, ODataVersion version, string fileName)
        {
            string filePath = @".\out\" + fileName + ".txt";
            FileResponseMessage message = new FileResponseMessage(new FileStream(filePath, FileMode.Create));
            message.SetHeader("Server", "Microsoft-IIS/7.0");
            message.StatusCode = 200;

            // create the writer, indent for readability of the examples.
            ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings() { Indent = true, CheckCharacters = false, BaseUri = new Uri("http://serviceRoot"), Version = version };
            messageWriterSettings.SetContentType(formatKind);
            using (ODataMessageWriter messageWriter = new ODataMessageWriter(message, messageWriterSettings))
            {
                messageWriter.WriteProperty(
                     new ODataProperty() { Name = "CompanyName", Value = dataSource.Customers.First().CompanyName }
                    );
            }
        }


        /// <summary>
        /// Generates a payload for a collection of properties
        /// </summary>
        /// <param name="formatKind">The format kind, can be either json or atom</param>
        /// <param name="fileName">output file</param>
        public void WriteCollection(ODataFormat formatKind, ODataVersion version, string fileName)
        {
            string filePath = @".\out\" + fileName + ".txt";
            FileResponseMessage message = new FileResponseMessage(new FileStream(filePath, FileMode.Create));
            message.SetHeader("Server", "Microsoft-IIS/7.0");
            message.StatusCode = 200;

            // create the writer, indent for readability of the examples.
            ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings() { Indent = true, CheckCharacters = false, BaseUri = new Uri("http://serviceRoot"), Version = version };
            messageWriterSettings.SetContentType(formatKind);

            //create a message writer then write a collection
            using (ODataMessageWriter messageWriter = new ODataMessageWriter(message, messageWriterSettings))
            {
                ODataCollectionWriter writer = messageWriter.CreateODataCollectionWriter();

                //the start method accepts the name of the collection to use as a string
                writer.WriteStart(new ODataCollectionResult { Name = "CompanyNames" });
                foreach (Customer c in dataSource.Customers)
                {
                    //for each customer, write the name of the customer as a value. 
                    //if writing a complex type, create an ODataComplexValue to pass in
                    writer.WriteItem(c.CompanyName);
                    writer.Flush();
                }
                writer.WriteEnd();
                writer.Flush();
            }
        }

        /// <summary>
        /// Generates a payload for a collection of properties
        /// </summary>
        /// <param name="formatKind">The format kind, can be either json or atom</param>
        /// <param name="fileName">output file</param>
        public void WriteComplexCollection(ODataFormat formatKind, ODataVersion version, string fileName)
        {
            string filePath = @".\out\" + fileName + ".txt";
            FileResponseMessage message = new FileResponseMessage(new FileStream(filePath, FileMode.Create));
            message.SetHeader("Server", "Microsoft-IIS/7.0");
            message.StatusCode = 200;

            // create the writer, indent for readability of the examples.
            ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings() { Indent = true, CheckCharacters = false, BaseUri = new Uri("http://serviceRoot"), Version = version };
            messageWriterSettings.SetContentType(formatKind);
            using (ODataMessageWriter messageWriter = new ODataMessageWriter(message, messageWriterSettings))
            {
                ODataCollectionWriter writer = messageWriter.CreateODataCollectionWriter();

                writer.WriteStart(new ODataCollectionResult { Name = "CompanyNames" });
                foreach (Customer c in dataSource.Customers)
                {
                    writer.WriteItem(new ODataComplexValue()
                    {
                        Properties = new List<ODataProperty>() { 
                                new ODataProperty(){ Name = "City", Value = c.Address.City },
                                new ODataProperty(){ Name = "PostalCode", Value = c.Address.PostalCode },
                                new ODataProperty(){ Name = "Street", Value = c.Address.Street },
                                },
                        TypeName = "Customer.Address"
                    });
                    writer.Flush();
                }
                writer.WriteEnd();
                writer.Flush();
            }
        }

        /// <summary>
        /// Write an error (non-200 status code response)
        /// </summary>
        /// <param name="formatKind"></param>
        /// <param name="fileName"></param>
        public void WriteError(ODataFormat formatKind, ODataVersion version, string fileName)
        {
            string filePath = @".\out\" + fileName + ".txt";
            FileResponseMessage message = new FileResponseMessage(new FileStream(filePath, FileMode.Create));
            message.StatusCode = 500;

            // create the writer, indent for readability of the examples.
            ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings() { Indent = true, CheckCharacters = false, BaseUri = new Uri("http://serviceRoot"), Version = version };
            messageWriterSettings.SetContentType(formatKind);
            using (ODataMessageWriter messageWriter = new ODataMessageWriter(message, messageWriterSettings))
            {
                //use the message writer to write an error to the stream. This represent a non-200 response to a request.
                messageWriter.WriteError(
                        new ODataError() { ErrorCode = "500", Message = "Internal Server Error", InnerError = new ODataInnerError() { Message = "Exception messsage thrown by runtime" } },
                        true
                    );
            }
        }

        public void WriteBatch(ODataFormat formatKind, ODataVersion version, string fileName)
        {
            string filePath = @".\out\" + fileName + ".txt";

            FileResponseMessage message = new FileResponseMessage(new FileStream(filePath, FileMode.Create));
            message.SetHeader("Server", "Microsoft-IIS/7.0");
            message.StatusCode = 200;

            // create the writer, indent for readability of the examples.
            ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings() { Indent = true, CheckCharacters = false, BaseUri = new Uri("http://serviceRoot"), Version = version };
            using (ODataMessageWriter messageWriter = new ODataMessageWriter(message, messageWriterSettings))
            {
                ODataBatchWriter writer = messageWriter.CreateODataBatchWriter();

                writer.WriteStartBatch();

                writer.WriteStartChangeset();
                writer.Flush();
                IODataResponseMessage batchMessage = writer.CreateOperationResponseMessage();

                using (ODataMessageWriter batchMessageWriter = new ODataMessageWriter(batchMessage, messageWriterSettings))
                {
                    ODataWriter entryWriter = batchMessageWriter.CreateODataEntryWriter();

                    // start the entry
                    entryWriter.WriteStart(new ODataEntry()
                    {
                        // the edit link is relative to the baseUri set on the writer in the case
                        EditLink = new Uri("/Customer(" + dataSource.Customers.First().CustomerID + ")", UriKind.Relative),
                        Id = "Customer(\"" + dataSource.Customers.First().CustomerID + "\")",
                        TypeName = "ConsoleSample.Customer",
                        Properties = new List<ODataProperty>(){
                            new ODataProperty(){ Name = "CustomerID", Value = dataSource.Customers.First().CustomerID },
                            new ODataProperty(){ Name = "CompanyName", Value = dataSource.Customers.First().CompanyName },
                            new ODataProperty(){ Name = "ContactName", Value = dataSource.Customers.First().ContactName },
                            new ODataProperty(){ Name = "ContactTitle", Value = dataSource.Customers.First().ContactTitle },
                            new ODataProperty(){ Name = "ContactAddress", Value = new ODataComplexValue() {  
                                Properties = new List<ODataProperty>() { 
                                    new ODataProperty(){ Name = "City", Value = dataSource.Customers.First().Address.City },
                                    new ODataProperty(){ Name = "PostalCode", Value = dataSource.Customers.First().Address.PostalCode },
                                    new ODataProperty(){ Name = "Street", Value = dataSource.Customers.First().Address.Street },
                                    }
                                }
                            }
                        }
                    });

                    entryWriter.WriteEnd();
                    entryWriter.Flush();
                }

                writer.WriteEndChangeset();
                writer.WriteEndBatch();
                writer.Flush();
            }
        }

        public void WriteServiceDocument(ODataFormat formatKind, ODataVersion version, string fileName)
        {
            string filePath = @".\out\" + fileName + ".txt";

            FileResponseMessage message = new FileResponseMessage(new FileStream(filePath, FileMode.Create));
            message.SetHeader("Server", "Microsoft-IIS/7.0");
            message.StatusCode = 200;

            //create the message writer by specifying an ODataMessageWriter settings
            ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings() { Indent = true, CheckCharacters = false, BaseUri = new Uri("http://serviceRoot") };
            messageWriterSettings.SetContentType(formatKind);
            using (ODataMessageWriter messageWriter = new ODataMessageWriter(message, messageWriterSettings))
            {
                //call the write service document API and specify a new workspace to write.
                messageWriter.WriteServiceDocument(
                    new ODataWorkspace()
                    {
                        //the work space contains a collection of Resource Collections Info. Each info contains the name of the collection.
                        Collections = new List<ODataResourceCollectionInfo>()
                        {
                            new ODataResourceCollectionInfo(){ Url = new Uri("Customers", UriKind.Relative) },
                            new ODataResourceCollectionInfo() { Url = new Uri("Employees", UriKind.Relative) }
                        }
                    });
            }
        }

        public void WriteMetadata(IEdmModel model, ODataVersion version, string fileName)
        {
            string filePath = @".\out\" + fileName + ".txt";

            FileResponseMessage message = new FileResponseMessage(new FileStream(filePath, FileMode.Create));
            message.SetHeader("Server", "Microsoft-IIS/7.0");
            message.StatusCode = 200;

            //create the message writer by specifying an ODataMessageWriter settings
            ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings() { Indent = true, CheckCharacters = false, BaseUri = new Uri("http://serviceRoot") };
            using (ODataMessageWriter messageWriter = new ODataMessageWriter(message, messageWriterSettings, model))
            {
                messageWriter.WriteMetadataDocument();
            }
        }
    }
}
