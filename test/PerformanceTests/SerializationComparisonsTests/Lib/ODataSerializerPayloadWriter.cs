using Microsoft.OData;
using Microsoft.OData.Core.NewWriter2;
using Microsoft.OData.Core.NewWriter2.Json.Payload;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentsLib;

internal class ODataSerializerPayloadWriter : IPayloadWriter<IEnumerable<Customer>>
{
    readonly IEdmModel model;
    ODataSerializerOptions _options;
    ODataUri _odataUri;

    public ODataSerializerPayloadWriter(IEdmModel model)
    {
        this.model = model;
        _options = GetOptions();
        _odataUri = GetODataUri();
    }

    private ODataSerializerOptions GetOptions()
    {
        if (_options != null)
        {
            return _options;
        }
        
        _options = new ODataSerializerOptions();
        

        _options.AddValueWriter(new CustomerWriter());
        _options.AddValueWriter(new AddressWriter());

        return _options;
    }

    ODataUri GetODataUri()
    {
        var parser = new ODataUriParser(
            this.model,
            new Uri("https://services.odata.org/V4/OData/OData.svc"),
            new Uri("Customers", UriKind.Relative));

        var odataUri = parser.ParseUri();
        return odataUri;
    }

    public async Task WritePayloadAsync(IEnumerable<Customer> payload, Stream stream, bool includeRawValues = false)
    {
        //var parser = new ODataUriParser(
        //    this.model,
        //    new Uri("https://services.odata.org/V4/OData/OData.svc"),
        //    new Uri("Customers", UriKind.Relative));

        //var odataUri = parser.ParseUri();

        await ODataSerializer.WriteAsync(
            payload,
            stream,
            _odataUri,
            this.model,
            _options).ConfigureAwait(false);
    }

    class CustomerWriter : ODataResourceBaseJsonWriter<Customer>
    {
        protected override ValueTask WritePropertyValue(
            Customer customer,
            IEdmProperty property,
            ODataJsonWriterStack stack,
            ODataJsonWriterContext context)
        {
            if (property.Name == "Id")
            {
                return context.WriteValueAsync(customer.Id, stack);
            }
            else if (property.Name == "Name")
            {
                return context.WriteValueAsync(customer.Name, stack);
            }
            else if (property.Name == "Emails")
            {
                return context.WriteValueAsync(customer.Emails, stack);
            }
            else if (property.Name == "Bio")
            {
                return context.WriteValueAsync(customer.Bio, stack);
            }
            else if (property.Name == "Content")
            {
                return context.WriteValueAsync(customer.Content, stack);
            }
            else if (property.Name == "HomeAddress")
            {
                return context.WriteValueAsync(customer.HomeAddress, stack);
            }
            else if (property.Name == "Addresses")
            {
                return context.WriteValueAsync(customer.Addresses, stack);
            }

            return ValueTask.CompletedTask;
        }
    }

    class AddressWriter : ODataResourceBaseJsonWriter<Address>
    {
        protected override ValueTask WritePropertyValue(
            Address address,
            IEdmProperty property,
            ODataJsonWriterStack stack,
            ODataJsonWriterContext context)
        {
            if (property.Name == "Street")
            {
                return context.WriteValueAsync(address.Street, stack);
            }
            else if (property.Name == "City")
            {
                return context.WriteValueAsync(address.City, stack);
            }
            else if (property.Name == "Misc")
            {
                // hack
                return context.WriteValueAsync(address.Misc.ToString(), stack);
            }

            return ValueTask.CompletedTask;
        }
    }
}
