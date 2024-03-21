using ExperimentsLib;

Console.WriteLine("Hello, World!");

var writers = DefaultWriterCollection.Create();
var writer = writers.GetWriter("NewWriter");
var data = CustomerDataSet.GetCustomers(2);

var stream = new MemoryStream();

await writer.WritePayloadAsync(data, stream);

stream.Position = 0;
StreamReader reader = new StreamReader(stream);
var content = reader.ReadToEnd();
Console.WriteLine(content);

Console.ReadLine();