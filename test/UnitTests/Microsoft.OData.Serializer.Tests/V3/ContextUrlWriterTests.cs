using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.V3;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Tests.V3;

public class ContextUrlWriterTests
{
    [Theory]
    [InlineData("Users", "Users")]
    [InlineData("users", "Users")]
    [InlineData("Users?$select=Id,Name", "Users(Id,Name)")]
    [InlineData("Users?$select=Id,Name,Address", "Users(Id,Name,Address)")]
    [InlineData("Users?$select=id, name, address", "Users(Id,Name,Address)")]
    [InlineData("Users?$select=*", "Users(*)")]
    [InlineData("Users?$expand=Files", "Users(Files())")]
    [InlineData("Users?$select=Id,Name,Address&$expand=Files", "Users(Id,Name,Address,Files())")]
    [InlineData("Users?$expand=Files($select=Id,FileName)", "Users(Files(Id,FileName))")]
    [InlineData("Users?$select=Name,Address/Coordinates", "Users(Name,Address/Coordinates)")]
    [InlineData("Users?$select=Name,Address/Coordinates,Address", "Users(Name,Address)")]
    [InlineData("Users?$select=Name,Address/Coordinates/Longitude,Address/City,Address/Coordinates", "Users(Name,Address/City,Address/Coordinates)")]
    [InlineData("Users?$select=Name,*,Address/Coordinates/Longitude,Address/City,Address/Coordinates", "Users(*)")]
    [InlineData("Users?$select=Id,Name&$expand=Files($select=Id,FileName)", "Users(Id,Name,Files(Id,FileName))")]
    [InlineData("Users?$select=*$expand=Files($select=Id,FileName)", "Users(*,Files(Id,FileName))")]
    [InlineData("Users?$select=Name&$expand=Files($select=*;$expand=Stats)", "Users(Name,Files(*,Stats()))")]

    public async Task WritesCorrectContextUrl_WhenResponseIsEntitySet(string requestUrl, string expectedContextUrl)
    {
        List<User> users = [];


        var options = new ODataSerializerOptions();
        var output = new MemoryStream();

        var model = CreateModel();
        var uriParser = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri(requestUrl, UriKind.Relative)
        )
        {
            Resolver = new UnqualifiedODataUriResolver { EnableCaseInsensitive = true }
        };
        var odataUri = uriParser.ParseUri();

        await ODataSerializer.WriteAsync(users, output, odataUri, model, options);

        output.Position = 0;

        var actual = new StreamReader(output).ReadToEnd();
        var actualParsed = JsonDocument.Parse(actual);
        var actualContextUrl = actualParsed.RootElement.GetProperty("@odata.context").GetString();

        Assert.Equal($"http://service/odata/$metadata#{expectedContextUrl}", actualContextUrl);
    }

    [Theory]
    [InlineData("Users('id')", "Users/$entity")]
    [InlineData("users('id')", "Users/$entity")]
    [InlineData("Users('id')?$select=Id,Name", "Users(Id,Name)/$entity")]
    [InlineData("Users('id')?$select=Id,Name,Address", "Users(Id,Name,Address)/$entity")]
    [InlineData("Users('id')?$select=id, name, address", "Users(Id,Name,Address)/$entity")]
    [InlineData("Users('id')?$select=*", "Users(*)/$entity")]
    [InlineData("Users('id')?$expand=Files", "Users(Files())/$entity")]
    [InlineData("Users('id')?$select=Id,Name,Address&$expand=Files", "Users(Id,Name,Address,Files())/$entity")]
    [InlineData("Users('id')?$expand=Files($select=Id,FileName)", "Users(Files(Id,FileName))/$entity")]
    [InlineData("Users('id')?$select=Name,Address/Coordinates", "Users(Name,Address/Coordinates)/$entity")]
    [InlineData("Users('id')?$select=Name,Address/Coordinates,Address", "Users(Name,Address)/$entity")]
    [InlineData("Users('id')?$select=Name,Address/Coordinates/Longitude,Address/City,Address/Coordinates", "Users(Name,Address/City,Address/Coordinates)/$entity")]
    [InlineData("Users('id')?$select=Name,*,Address/Coordinates/Longitude,Address/City,Address/Coordinates", "Users(*)/$entity")]
    [InlineData("Users('id')?$select=Id,Name&$expand=Files($select=Id,FileName)", "Users(Id,Name,Files(Id,FileName))/$entity")]
    [InlineData("Users('id')?$select=*$expand=Files($select=Id,FileName)", "Users(*,Files(Id,FileName))/$entity")]
    [InlineData("Users('id')?$select=Name&$expand=Files($select=*;$expand=Stats)", "Users(Name,Files(*,Stats()))/$entity")]
    public async Task WritesCorrectContextUrl_WhenResponseIsEntityFromEntitySet(string requestUrl, string expectedContextUrl)
    {
        var user = new User
        {
            Id = "id",
            Name = "Name",
            Address = new Address
            {
                Street = "123 Main St",
                City = "Metropolis",
                Coordinates = new Coordinates
                {
                    Latitude = 40.7128,
                    Longitude = -74.0060
                }
            },
            Files = []
        };


        var options = new ODataSerializerOptions();
        var output = new MemoryStream();

        var model = CreateModel();
        var uriParser = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri(requestUrl, UriKind.Relative)
        )
        {
            Resolver = new UnqualifiedODataUriResolver { EnableCaseInsensitive = true }
        };
        var odataUri = uriParser.ParseUri();

        await ODataSerializer.WriteAsync(user, output, odataUri, model, options);

        output.Position = 0;

        var actual = new StreamReader(output).ReadToEnd();
        var actualParsed = JsonDocument.Parse(actual);
        var actualContextUrl = actualParsed.RootElement.GetProperty("@odata.context").GetString();

        Assert.Equal($"http://service/odata/$metadata#{expectedContextUrl}", actualContextUrl);
    }

    private static IEdmModel CreateModel()
    {
        var model = new EdmModel();

        var activityStatType = model.AddEntityType("NS", "ActivityStat");
        activityStatType.AddKeys(activityStatType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.String));
        activityStatType.AddStructuralProperty("Opened", EdmPrimitiveTypeKind.Boolean);
        activityStatType.AddStructuralProperty("Read", EdmPrimitiveTypeKind.Boolean);

        var fileType = model.AddEntityType("NS", "File");
        fileType.AddKeys(fileType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.String));
        fileType.AddStructuralProperty("FileName", EdmPrimitiveTypeKind.String);
        fileType.AddStructuralProperty("Extension", EdmPrimitiveTypeKind.String);
        fileType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
        {
            Name = "Stats",
            Target = activityStatType,
            TargetMultiplicity = EdmMultiplicity.Many,
            ContainsTarget = true
        });
        model.AddElement(fileType);

        var coordinatesType = model.AddComplexType("NS", "Coordinates");
        coordinatesType.AddStructuralProperty("Latitude", EdmPrimitiveTypeKind.Double);
        coordinatesType.AddStructuralProperty("Longitude", EdmPrimitiveTypeKind.Double);

        var addressType = model.AddComplexType("NS", "Address");
        addressType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
        addressType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
        addressType.AddStructuralProperty("Coordinates", new EdmComplexTypeReference(coordinatesType, isNullable: false));

        var userType = model.AddEntityType("NS", "User");
        userType.AddKeys(userType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.String));
        userType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
        userType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, isNullable: false));
        userType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
        {
            Name = "Files",
            Target = fileType,
            TargetMultiplicity = EdmMultiplicity.Many,
            ContainsTarget = true
        });
        model.AddElement(userType);


        var container = model.AddEntityContainer("NS", "Container");
        container.AddEntitySet("Users", userType);

        return model;
    }

    class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public List<FileItem> Files { get; set; }
    }

    class FileItem
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public List<ActivityStat> Stats { get; set; }
    }

    class ActivityStat
    {
        public string Id { get; set; }
        public bool Opened { get; set; }
        public bool Read { get; set; }
    }

    class Address
    {
        public string Street { get; set; }
        public string City { get; set; }

        public Coordinates Coordinates { get; set; }
    }

    class Coordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
