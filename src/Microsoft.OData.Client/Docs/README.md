# Microsoft.OData.Client

The `Microsoft.OData.Client` library allows you to consume data from and interact with OData services from .NET applications. It provides LINQ-enabled client APIs for issuing OData queries and constructing and consuming OData JSON payloads. It supports OData [`v4` and `v4.01`](https://www.odata.org/documentation/).

## Installation

You can install the `Microsoft.OData.Client` package via NuGet:

```sh
dotnet add package Microsoft.OData.Client
```

Or via the NuGet Package Manager Console:

```sh
Install-Package Microsoft.OData.Client
```

## Getting Started

### Creating an OData Client

To create an OData client, you can use the [`OData Connected Service` in `Visual Studio`](https://learn.microsoft.com/odata/connectedservice/getting-started) to generate strongly-typed client code for your OData service.

1. **Install the OData Connected Service extension**:
   - Go to the `Extensions` menu in `Visual Studio`.
   - Select `Manage Extensions`.
   - Search for `OData Connected Service` and install it.
   - Alternatively, you can get the extension at Visual Studio Marketplace: [OData Connected Service](https://marketplace.visualstudio.com/items?itemName=marketplace.ODataConnectedService) and [OData Connected Service 2022+](https://marketplace.visualstudio.com/items?itemName=marketplace.ODataConnectedService2022)

2. **Add the OData Connected Service to your project**:
   - Right-click your project in the Solution Explorer.
   - Select Add > Connected Service.
   - Choose OData Connected Service and follow the wizard to configure your service.


### Example Usage

Here's a simple example of how to use the generated client to interact with an OData service:

```csharp
using Microsoft.OData.SampleService.Models.TripPin;

var serviceUri = new Uri("https://services.odata.org/V4/TripPinServiceRW/");
var context = new DefaultContainer(serviceUri);

// Querying data
var people = await context.People.ExecuteAsync();
foreach (var person in people)
{
    Console.WriteLine($"{person.FirstName} {person.LastName}");
}
```

## Documentation

For more detailed information, please refer to the [official documentation](https://learn.microsoft.com/odata/client/getting-started)

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