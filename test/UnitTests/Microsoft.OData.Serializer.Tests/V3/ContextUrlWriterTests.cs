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
    [InlineData("Users?$select=*", "Users(*)")]
    [InlineData("Users?$expand=Files", "Users(Files())")]
    [InlineData("Users?$expand=Files($select=Id,FileName)", "Users(Files(Id,FileName))")]
    [InlineData("Users?$select=Id,Name$expand=Files($select=Id,FileName)", "Users(Id,Name,Files(Id,FileName))")]
    [InlineData("Users?$select=*$expand=Files($select=Id,FileName)", "Users(*,Files(Id,FileName))")]
    [InlineData("Users?$select=Name$expand=Files($select=*;$expand=Stats)", "Users(Name,Files(*,Stats()))")]
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

        var userType = model.AddEntityType("NS", "User");
        userType.AddKeys(userType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.String));
        userType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
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
}
