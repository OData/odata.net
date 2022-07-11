using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData;
using Microsoft.OData.Json;

namespace ExperimentsLib
{
    /// <summary>
    /// Writes Customer collection OData JSON format using <see cref="IJsonWriter"/> directly,
    /// i.e. without creating intermediate <see cref="ODataResource"/> objects.
    /// </summary>
    public class ODataJsonWriterDirectPayloadWriter : IPayloadWriter<IEnumerable<Customer>>
    {
        private readonly Func<Stream, IJsonWriter> jsonWriterFactory;
        private bool _simulateTypedResourceGeneration = true;

        public ODataJsonWriterDirectPayloadWriter(Func<Stream, IJsonWriter> jsonWriterFactory, bool simulateTypedResourceGeneration = false)
        {
            this.jsonWriterFactory = jsonWriterFactory;
            _simulateTypedResourceGeneration = simulateTypedResourceGeneration;
        }

        public Task WritePayload(IEnumerable<Customer> payload, Stream stream)
        {
            var serviceRoot = new Uri("https://services.odata.org/V4/OData/OData.svc/");
            var jsonWriter = jsonWriterFactory(stream);


            var resourceSet = new ODataResourceSet();
            jsonWriter.StartObjectScope();
            jsonWriter.WriteName("@odata.context");
            jsonWriter.WriteValue($"{serviceRoot}$metadata#Customers");
            jsonWriter.WriteName("value");
            jsonWriter.StartArrayScope();

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
