using Microsoft.OData;
using Microsoft.OData.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentsLib
{
    /// <summary>
    /// Writes Customer collection OData JSON format using <see cref="IJsonWriter"/> directly.
    /// </summary>
    public class ODataJsonWriterBasicServerWriter : IServerWriter<IEnumerable<Customer>>
    {
        private readonly Func<Stream, IJsonWriter> jsonWriterFactory;
        private bool _simulateTypedResourceGeneration = true;

        public ODataJsonWriterBasicServerWriter(Func<Stream, IJsonWriter> jsonWriterFactory, bool simulateTypedResourceGeneration = false)
        {
            this.jsonWriterFactory = jsonWriterFactory;
            _simulateTypedResourceGeneration = simulateTypedResourceGeneration;
        }

        public Task WritePayload(IEnumerable<Customer> payload, Stream stream)
        {
            var sw = new Stopwatch();
            sw.Start();

            var serviceRoot = new Uri("https://services.odata.org/V4/OData/OData.svc/");
            var jsonWriter = jsonWriterFactory(stream);


            var resourceSet = new ODataResourceSet();
            //Console.WriteLine("Start writing resource set");
            jsonWriter.StartObjectScope();
            jsonWriter.WriteName("@odata.context");
            jsonWriter.WriteValue($"{serviceRoot}$metadata#Customers");
            jsonWriter.WriteName("value");
            jsonWriter.StartArrayScope();

            //Console.WriteLine("About to write resources {0}", payload.Count());
            foreach (var _customer in payload)
            {
                Customer customer = _customer;
                if (_simulateTypedResourceGeneration)
                {
                    customer = new Customer
                    {
                        Id = _customer.Id,
                        Name = _customer.Name,
                        Emails = new List<string>(_customer.Emails),
                        HomeAddress = new Address
                        {
                            City = _customer.HomeAddress.City,
                            Street = _customer.HomeAddress.Street
                        },
                        Addresses = _customer.Addresses.Select(a => new Address { City = a.City, Street = a.Street }).ToList()
                    };
                }

                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("Id");
                jsonWriter.WriteValue(customer.Id);
                jsonWriter.WriteName("Name");
                jsonWriter.WriteValue(customer.Name);
                jsonWriter.WriteName("Emails");
                jsonWriter.StartArrayScope();
                foreach (var email in customer.Emails)
                {
                    jsonWriter.WriteValue(email);
                }
                jsonWriter.EndArrayScope();


                // -- HomeAddress
                // start write homeAddress
                jsonWriter.WriteName("HomeAddress");
                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("City");
                jsonWriter.WriteValue(customer.HomeAddress.City);
                jsonWriter.WriteName("Street");
                jsonWriter.WriteValue(customer.HomeAddress.Street);

                // end write homeAddress
                jsonWriter.EndObjectScope();
                // -- End HomeAddress

                // -- Addresses
                jsonWriter.WriteName("Addresses");
                jsonWriter.StartArrayScope();

                // start addressesResourceSet
                foreach (var address in customer.Addresses)
                {
                    jsonWriter.StartObjectScope();
                    jsonWriter.WriteName("City");
                    jsonWriter.WriteValue(address.City);
                    jsonWriter.WriteName("Street");
                    jsonWriter.WriteValue(address.Street);
                    jsonWriter.EndObjectScope();
                }

                // end addressesResourceSet
                jsonWriter.EndArrayScope();
                // -- End Addresses

                // end write resource
                jsonWriter.EndObjectScope();
            }

            jsonWriter.EndArrayScope();
            jsonWriter.EndObjectScope();
            jsonWriter.Flush();

            return Task.CompletedTask;
        }
    }
}
