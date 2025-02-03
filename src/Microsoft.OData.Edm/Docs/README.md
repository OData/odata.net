# Microsoft.OData.Edm

The `Microsoft.OData.Edm` library provides APIs to build, parse, and validate `Entity Data Model (EDM)` that conform to the [`OData protocol`](https://www.odata.org/documentation/). It is a core component of the `OData .NET libraries`, enabling you to work with OData metadata.

Think of it as the schema that describes the kind of data your service exposes. The `Microsoft.OData.Edm` library, also known as `EdmLib`, provides `classes` and `interfaces` for working with OData models. It includes classes for reading/parsing a schema file in CSDL in `XML` or `JSON` formats, writing such files (`CsdlReader` and `CsdlWriter`), as well as creating a model directly in-memory (`EdmModel`).

The `IEdmModel` interface is used extensively across `OData` libraries as it provides the base layer for retrieving information about the types and functionality exposed by an OData service.

The `EdmModel` class (which implements the `IEdmModel` interface) allows you to manually create a model/schema in memory.

## Installation

You can install the `Microsoft.OData.Edm` package via NuGet:

```sh
dotnet add package Microsoft.OData.Edm
```

Or via the NuGet Package Manager Console:

```sh
Install-Package Microsoft.OData.Edm
```

## Getting Started

### Creating an EDM Model

Here's a simple example of how to create an EDM model using `Microsoft.OData.Edm`:

```csharp
using Microsoft.OData.Edm;
using System;

namespace EdmLibSample;

public class SampleModelBuilder
{
    public static IEdmModel GetEdmModel()
    {
        // Create an empty model
        var model = new EdmModel();

        // Define an entity type
        var productType = new EdmEntityType("NS", "Product");
        productType.AddKeys(productType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
        productType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
        model.AddElement(productType);

        // Define an entity container
        var container = new EdmEntityContainer("NS", "Container");
        model.AddElement(container);

        // Define an entity set
        var products = container.AddEntitySet("Products", productType);

        // Output the model
        Console.WriteLine("EDM Model created successfully.");

        return model;
    }
}
```

### Parsing an EDM Model

You can also parse an `EDM model` from a CSDL (Common Schema Definition Language) document:

```csharp
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm;
using System.Xml;

string csdl = @"
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name='Product'>
            <Key>
                <PropertyRef Name='ID' />
            </Key>
            <Property Name='ID' Type='Edm.Int32' Nullable='false' />
            <Property Name='Name' Type='Edm.String' />
        </EntityType>
        <EntityContainer Name='Container'>
            <EntitySet Name='Products' EntityType='NS.Product' />
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

IEdmModel model;
using (var reader = XmlReader.Create(new StringReader(csdl)))
{
    model = CsdlReader.Parse(reader);
    Console.WriteLine("EDM Model parsed successfully.");
}
```

## Documentation

For more detailed information, please refer to the [official documentation](https://learn.microsoft.com/odata/odatalib/edm/build-basic-model).

## Community

### Contribution

There are many ways for you to contribute to [`OData .NET`](https://github.com/OData/odata.net). The easiest way is to participate in discussion of features and issues. You can also contribute by sending pull requests of features or bug fixes to us. Contribution to the documentations is also highly welcomed. Please refer to the [CONTRIBUTING.md](https://github.com/OData/odata.net/blob/main/.github/CONTRIBUTING.md) for more details.

### Reporting Security Issues

Security issues and bugs should be reported privately, via email, to the Microsoft Security Response Center (MSRC) <secure@microsoft.com>. You should receive a response within 24 hours. If for some reason you do not, please follow up via email to ensure we received your original message. Further information, including the MSRC PGP key, can be found in the [Security TechCenter](https://www.microsoft.com/msrc/faqs-report-an-issue).

### 5.3 Support

- **Issues**: Report issues on [Github issues](https://github.com/OData/odata.net/issues).
- **Questions**: Ask questions on [Stack Overflow](http://stackoverflow.com/questions/ask?tags=odata).
- **Feedback**: Please send mails to [odatafeedback@microsoft.com](mailto:odatafeedback@microsoft.com).
- **Team blog**: Please visit [https://devblogs.microsoft.com/odata/](https://devblogs.microsoft.com/odata/) and [http://www.odata.org/blog/](http://www.odata.org/blog/).

### Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.