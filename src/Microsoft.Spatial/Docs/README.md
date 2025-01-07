# Microsoft.Spatial

The `Microsoft.Spatial` library provides classes and methods for geographic and geometric operations. It is a core component of the OData .NET libraries, enabling you to work with geospatial data types such as points in 2D/3D space, lat/long, lines, and polygons.

## Installation

You can install the `Microsoft.Spatial` package via NuGet:

```sh
dotnet add package Microsoft.Spatial
```

Or via the NuGet Package Manager Console:

```sh
Install-Package Microsoft.Spatial
```

## Getting Started

### Creating and Using Spatial Data

Here's a simple example of how to create and use spatial data types with `Microsoft.Spatial`:

```csharp
using Microsoft.Spatial;

// Create a GeographyPoint
var point = GeographyPoint.Create(47.6097, -122.3331);

// Output the point's coordinates
Console.WriteLine($"Latitude: {point.Latitude}, Longitude: {point.Longitude}");

// Create a GeographyLineString
var lineString = GeographyFactory.LineString()
    .LineTo(47.6097, -122.3331)
    .LineTo(47.6205, -122.3493)
    .Build();

// Output the line string's points
foreach (var position in lineString.Points)
{
    Console.WriteLine($"Point: Latitude {position.Latitude}, Longitude {position.Longitude}");
}
```

## Documentation

For more detailed information, please refer to the [official documentation](https://learn.microsoft.com/odata/spatial/define-property).


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