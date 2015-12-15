# OData .NET Libraries
## Introduction
The [OData .NET Libraries](http://odata.github.io/odata.net) (or OData.NET, for short) project includes the implementation of the OData protocol on the .NET platform. It is a fully open sourced project maintained by the OData team of Microsoft.

OData stands for the Open Data Protocol. It was initiated by Microsoft and is now an OASIS standard. OData enables the creation and consumption of REST APIs, which allow resources, identified using URLs and defined in a data model, to be published and edited by Web clients using simple HTTP messages.

For more information about OData, please refer to the following resources:

 - [OData.org](http://www.odata.org/)
 - [OASIS Open Data Protocol (OData) Technical Committee](https://www.oasis-open.org/committees/tc_home.php?wg_abbrev=odata)

## Project structure
The project currently has a few branches: master, gh-pages, ODATAV3, and WCFDSV4. 

### Master branch
The master branch includes the .NET libraries for OData V4 only that are now most actively iterated and maintained by the OData team. It has the following libraries: 

 - [ODataLib](http://www.nuget.org/packages/Microsoft.OData.Core/) (namespace `Microsoft.OData.Core`):<br />
 The ODataLib contains classes to serialize, deserialize and validate OData JSON payloads. 
 - [EdmLib](http://www.nuget.org/packages/Microsoft.OData.Edm/) (namespace `Microsoft.OData.Edm`):<br />
 The EdmLib contains classes to represent, construct, parse, serialize and validate entity data models.
 - [OData Client for .NET](http://www.nuget.org/packages/Microsoft.OData.Client/) (namespace `Microsoft.OData.Client`):<br />
 The client library is built on top of ODataLib and EdmLib that has LINQ-enabled client APIs for issuing OData queries and consuming OData JSON payloads.
 - [Microsoft.Spatial](http://www.nuget.org/packages/Microsoft.Spatial/) (namespace `Microsoft.Spatial`):<br />
 The spatial library contains classes and methods that facilitate geography and geometry spatial operations.

For these libraries, we accept bug reports, feature requirements and pull requests. The corresponding fixes and implementations will be included into every new release.

The release of the component binaries is carried out monthly through [Nuget](http://www.nuget.org/).

### gh-pages
The [gh-pages](https://github.com/OData/odata.net/tree/gh-pages) branch contains documentation source for OData v4 Lib - tutorials, guides, etc.  The documention source is in Markdown format.

### ODATAV3 (maintenance mode)

The ODATAV3 branch includes the .NET libraries for OData V1-3 only. It has the similar libraries as the master branch does except for some differences in namespaces and two additional libraries: 

 - [ODataLib for OData v1-3](http://www.nuget.org/packages/Microsoft.Data.OData/) (namespace `Microsoft.Data.Core`):
 - [EdmLib for OData v1-3](http://www.nuget.org/packages/Microsoft.Data.Edm/) (namespace `Microsoft.Data.Edm`):
 - [WCF Data Services Client for OData v1-3](http://www.nuget.org/packages/Microsoft.Data.Services.Client/) (namespace `Microsoft.Data.Services.Client`):
 - [System.Spatial for OData v1-3](http://www.nuget.org/packages/System.Spatial/) (namespace `System.Spatial`):
 - [WCF Data Services Server for OData v1-3](http://www.nuget.org/packages/Microsoft.Data.Services/) (namespace `Microsoft.Data.Services`):<br />
 Fully-featured server API for responding to OData queries and consuming/producing OData payloads.
 - [WCF Data Services EntityFramework Provider](http://www.nuget.org/packages/Microsoft.OData.EntityFrameworkProvider/) (namespace `Microsoft.OData.EntityFrameworkProvider`):<br />
 Server API for responding to OData queries and consuming/producing OData payloads based on entity framework version 6.0 or higher Supports OData v3.

These libraries are in maintenance mode. Only security bugs will be accepted. The release will be irregular depends on the bugs fixed.

### WCFDSV4 (maintenance mode)

The WCFDSV4 branch has the source code of the OData V4 parity of the WCF Data Services Server for OData v1-3. It is only for cloning and doesn't accept contributions. There is no binary release of it either.

## Getting started

### Building

Simply open the solution files under 'sln' and build them in Visual Studio IDE.

Here is the usage of each solution file:
 - Microsoft.OData.Lite.sln - Product source and minimal functional tests. *Recommended* for doing general bugfix and feature development.
 - Microsoft.OData.Full.sln - Product source and full functional tests. Used to *fully* test your code.
 - Microsoft.OData.E2E.sln - Product source and end-to-end tests.
 - Microsoft.OData.CodeGen.sln - Source and tests for OData T4 client code generator.

### Testing

Each solution contains some test projects. Please open it, build it and run all the tests in the test explorer. For running tests within Microsoft.OData.Full.sln and Microsoft.OData.E2E.sln, you need to open the solution as *Administrator* so that the test services can be started properly. You must have either SQL Express 2008+ (in the case of VS2013) or LocalDB v12.0+ (in the case of VS2015) installed because a test database is needed and it will be automatically initialized by the test code if it doesn't exist.

#### One-click build and test script

In an elevated command prompt "Run as administrator", cd to the root folder:
```
build.cmd
```
The build and test will take about 15 minutes. Here are some other usages:

 - `build.cmd Nightly`. Build and run all nightly test suites.
 - `build.cmd Rolling`. Build and run all rolling test suites (with less legacy tests thus faster).
 - `build.cmd SkipStrongName`. Configure strong name skip of OData libraries on your machine and build (no test run).
 - `build.cmd DisableSkipStrongName`. Disable strong name skip of OData libraries on your machine and build (no test run).
 
### Nightly Builds

We keep uploading the daily nightly signed NuGet packages for ODataLib/EdmLib/ClientLib/SpatialLib to our MyGet feed: https://www.myget.org/F/odlnightly.

You can query the latest nightly NuGet packages using the following MAGIC OData query:
https://www.myget.org/F/odlnightly/Packages?$select=Id,Version&$orderby=Version%20desc&$top=4&$format=application/json

### Documentation
Please visit the [ODataLib pages](http://odata.github.io/odata.net).

### Debug

Please refer to the [How to debug](http://odata.github.io/WebApi/10-01-debug-webapi-source).

## Community
### Contribution
Please refer to the [CONTRIBUTION.md](https://github.com/OData/odata.net/blob/master/CONTRIBUTION.md).

### Support
 - Team blog<br />
 Please visit http://blogs.msdn.com/b/odatateam/.
 - Feedback<br />
 Please send mails to [odatafeedback@microsoft.com](mailto:odatafeedback@microsoft.com).
 - Questions<br />
 Ask questions on [Stack Overflow](http://stackoverflow.com/questions/ask?tags=odata).
