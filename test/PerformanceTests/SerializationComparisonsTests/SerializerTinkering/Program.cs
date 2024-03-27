using ExperimentsLib;
using Microsoft.OData;
using Microsoft.OData.UriParser;
using System.Net;
using System.Text.Json;

var model = DataModel.GetEdmModel();
var data = CustomerDataSet.GetCustomers(2);


await CustomersExample("Customers", "Serialize customers entity set with default structural properties");

await CustomersExample("Customers?$select=Id,Name", "Customers entity set with selected Id and Name");

await CustomersExample("Customers?$select=Emails,HomeAddress", "Customers entity set with selected Emails and HomeAddress");

await WriteExample(
    data.Select(d => new { Id = d.Id, Name = d.Name }),
    "Customers?$select=Id,Name",
    "LINQ-projected customer entities with $select=Id,Nmae"
    );

ODataUri ParseODataUri(string uriString)
{
    var serviceRoot = new Uri("https://services.odata.org/V4/OData/OData.svc/");
    var uri = new Uri($"https://services.odata.org/V4/OData/OData.svc/{uriString}");
    var parser = new ODataUriParser(model, serviceRoot, uri);
    var odataUri = parser.ParseUri();
    return odataUri;
}

async Task<string> WritePayload<T>(T payload, string endpoint)
{
    var odataUri = ParseODataUri(endpoint);
    var options = OptionsHelper.CreateJsonSerializerOptions(model, odataUri);
    var stream = new MemoryStream();
    await JsonSerializer.SerializeAsync(stream, payload, options);
    stream.Position = 0;
    StreamReader reader = new StreamReader(stream);
    var content = reader.ReadToEnd();
    return content;
}

async Task CustomersExample(string endpoint, string description)
{
    await WriteExample(data, endpoint, description);
}

async Task WriteExample<T>(T payload, string endpoint, string description)
{
    Console.WriteLine(description);
    var content = await WritePayload(payload, endpoint);
    Console.WriteLine(content);
    Console.WriteLine();
    Console.WriteLine("Press key to continue...");
    Console.Read();
}