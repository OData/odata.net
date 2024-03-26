using ExperimentsLib;
using Microsoft.OData;
using Microsoft.OData.UriParser;
using System.Text.Json;

var model = DataModel.GetEdmModel();
var data = CustomerDataSet.GetCustomers(2);


await CustomersExample("Customers", "Serialize customers entity set with default structural properties");

await CustomersExample("Customers?$select=Id,Name", "Customers entity set with selected Id and Name");

await CustomersExample("Customers?$select=Emails,HomeAddress", "Customers entity set with selected Emails and HomeAddress");


Task<string> WriteCustomerEntitySet(IEnumerable<Customer> payload, string endpoint)
{
    var odataUri = ParseODataUri(endpoint);
    var options = OptionsHelper.CreateJsonSerializerOptions(model, odataUri);

    return WritePayload(payload, options);
}

ODataUri ParseODataUri(string uriString)
{
    var serviceRoot = new Uri("https://services.odata.org/V4/OData/OData.svc/");
    var uri = new Uri($"https://services.odata.org/V4/OData/OData.svc/{uriString}");
    var parser = new ODataUriParser(model, serviceRoot, uri);
    var odataUri = parser.ParseUri();
    return odataUri;
}

async Task<string> WritePayload<T>(T payload, JsonSerializerOptions options)
{
    var stream = new MemoryStream();
    await JsonSerializer.SerializeAsync(stream, payload, options);
    stream.Position = 0;
    StreamReader reader = new StreamReader(stream);
    var content = reader.ReadToEnd();
    return content;
}

async Task CustomersExample(string endpoint, string description)
{
    Console.WriteLine(description);
    var content = await WriteCustomerEntitySet(data, endpoint);
    Console.WriteLine(content);
    Console.WriteLine();
    Console.WriteLine("Press key to continue...");
    Console.Read();
}