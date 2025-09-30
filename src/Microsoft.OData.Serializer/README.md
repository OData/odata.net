# `Microsoft.OData.Serializer`

This is a preview of a new official serializer for .NET OData libraries. It address major performance
limitations with the existing serialization stack. It also makes improvements to usability.

The `Microsoft.OData.Serialization` is packaged a standalone library while in preview so that it can be easy for early adoptors
to test it and give us feedback. However, we plan to bundle it with the main `Microsoft.OData.Core` library in the official release.
After the official release, the standalone `Microsoft.OData.Serialization` NuGet package will be retired.

To serializer preview depends on `Microsoft.OData.Core` and `Micrososft.OData.Edm` packages. It supports version 7.x, 8.x and 9.x-preview.
The serializer supports multiple OData version to allow more people to test it during preview without having to upgrade their versions
of OData libraries. However, the official release will be tied to `Microsoft.OData.Core` 9.x.

To demonstrate how it works, here's a sample application that demonstrates the new serializer in action.

- Create a new .NET Console application
- Install the following packages:
  - `Microsoft.OData.Edm`
  - `Microsoft.OData.Core`
  - You may also need to install `Microsoft.Extensions.DepencyInjection.Abstractions`
  - `Microsoft.OData.Serialization` version `0.1.0-preview1`.

```csharp
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Serializer;
using Microsoft.OData.UriParser;
using System.Xml;

var csdlSchema =
"""
<edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx">
  <edmx:DataServices>
    <Schema Namespace="ODataDemo" xmlns="http://docs.oasis-open.org/odata/ns/edm">
      <EntityType Name="Product">
        <Key>
          <PropertyRef Name="ID"/>
        </Key>
        <Property Name="ID" Type="Edm.Int32" Nullable="false"/>
        <Property Name="Name" Type="Edm.String"/>
        <Property Name="Description" Type="Edm.String"/>
        <Property Name="ReleasedAt" Type="Edm.DateTimeOffset"/>
        <Property Name="Rating" Type="Edm.Int32"/>
        <Property Name="Price" Type="Edm.Decimal"/>
      </EntityType>
      <EntityContainer Name="DemoService">
        <EntitySet Name="Products" EntityType="ODataDemo.Product"/>
      </EntityContainer>
    </Schema>
 </edmx:DataServices>
</edmx:Edmx>
""";
var xmlReader = XmlReader.Create(new StringReader(csdlSchema));
IEdmModel model = CsdlReader.Parse(xmlReader);


List<Product> products = [
    new Product
    {
        ID = 1,
        Name = "Laptop",
        Description = "A high-performance laptop.",
        ReleaseDate = new DateTimeOffset(2023, 1, 15, 0, 0, 0, TimeSpan.Zero),
        Rating = 5,
        Price = 999.99m
    },
    new Product
    {
        ID = 2,
        Name = "Smartphone",
        Description = "A latest model smartphone.",
        ReleaseDate = new DateTimeOffset(2023, 3, 10, 0, 0, 0, TimeSpan.Zero),
        Rating = 4,
        Price = 699.99m
    }
];

var serializerOptions = new ODataSerializerOptions();


var odataUri = new ODataUriParser(
    model,
    new Uri("http://localhost/odata"),
    new Uri("Products", UriKind.Relative))
  .ParseUri();


using var stream = Console.OpenStandardOutput();

await ODataSerializer.WriteAsync(products, stream, odataUri, model, serializerOptions);

Console.ReadKey();

[ODataType("ODataDemo.Product")]
class Product
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    [ODataPropertyName("ReleasedAt")]
    public DateTimeOffset ReleaseDate { get; set; }
    public int Rating { get; set; }
    public decimal Price { get; set; }
}
```

If you run the application, you'll see the following output printed on the console (pretty-printed for clarity):

```json
{
  "@odata.context": "http://localhost/odata/$metadata#Products",
  "value": [
    {
      "ID": 1,
      "Name": "Laptop",
      "Description": "A high-performance laptop.",
      "ReleasedAt": "2023-01-15T00:00:00Z",
      "Rating": 5,
      "Price": 999.99
    },
    {
      "ID": 2,
      "Name": "Smartphone",
      "Description": "A latest model smartphone.",
      "ReleasedAt": "2023-03-10T00:00:00Z",
      "Rating": 4,
      "Price": 699.99
    }
  ]
}
```
