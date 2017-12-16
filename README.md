# OData .NET Libraries
 Build  | Status
--------|---------
Rolling | <img src="https://identitydivision.visualstudio.com/_apis/public/build/definitions/2cfe7ec3-b94f-4ab9-85ab-2ebff928f3fd/422/badge"/>
Nightly | <img src="https://identitydivision.visualstudio.com/_apis/public/build/definitions/2cfe7ec3-b94f-4ab9-85ab-2ebff928f3fd/107/badge"/>

## 1. Introduction

The [OData .NET Libraries](http://odata.github.io/odata.net) (or OData .NET, for short) project includes the implementation of core functionalities of OData protocol on the .NET platform which includes URI parsing, request and response reading and writing, Entity Data Model (EDM) building, and also a .Net OData client which can be used to consume OData service. It is a fully open sourced project maintained by Microsoft OData team. The libraries are used by [WebApi](https://github.com/OData/WebApi/ "WebApi") and [RESTier](https://github.com/odata/RESTier/ "RESTier") which are recommended to be adopted to build new OData Services.

[OData](http://www.odata.org/ "OData") stands for the Open Data Protocol. It was initiated by Microsoft and is now an [ISO](https://www.oasis-open.org/news/pr/iso-iec-jtc-1-approves-oasis-odata-standard-for-open-data-exchange) approved and [OASIS](https://www.oasis-open.org/committees/tc_home.php?wg_abbrev=odata) standard. OData enables the creation and consumption of REST APIs, which allow resources, identified using URLs and defined in a data model, to be published and edited by Web clients using simple HTTP requests.

For more information about OData, please refer to the following resources:

- [OData.org](http://www.odata.org/)
- [OASIS Open Data Protocol (OData) Technical Committee](https://www.oasis-open.org/committees/tc_home.php?wg_abbrev=odata)

**For how to adopt this and related libraries to build or consume OData service, please refer to the following resources:**

- [Build an OData v4 Service with RESTier Library](http://odata.github.io/RESTier/#01-01-Introduction)
- [Build an OData v4 Service with OData WebApi Library](http://odata.github.io/WebApi/#01-02-getting-started)
- [OData .Net Client](http://odata.github.io/odata.net/#04-01-basic-crud-operations)

## 2. Project structure

The project currently has six branches: [master](https://github.com/OData/odata.net/tree/master), [release](https://github.com/OData/odata.net/tree/release), [gh-pages](https://github.com/OData/odata.net/tree/gh-pages), [maintenance-6.x](https://github.com/OData/odata.net/tree/maintenance-6.x), [maintenance-5.x](https://github.com/OData/odata.net/tree/maintenance-5.x), and [maintenance-wcf-dataservice-v4](https://github.com/OData/odata.net/tree/maintenance-wcf-dataservice-v4).

**master branch:**

This master branch is the development branch for ODataV4 7.x and is now most actively iterated. It builds upon the OData 6.15 release which is now on [maintenance-6.x branch](https://github.com/OData/odata.net/tree/maintenance-6.x) and produces both [PCL (Portable Class Libraries) Profile111](https://msdn.microsoft.com/library/gg597391.aspx) and [.NET Standard 1.1](https://docs.microsoft.com/en-us/dotnet/articles/standard/library) libraries. The branch builds mostly with Visual Studio 2015, and it is undergoing migration towards Visual Studio 2017; currently, the .NET Standard and .NET Core projects have been migrated. Due to the number of test projects, complete migration to the latest version of Visual Studio will be broken down into multiple steps. The code is shared between the .NET Framework and .NET Standard platforms, and you may use either Visual Studio 2015/2017 to contribute.

For each profile above, it has the following libraries:

- [ODataLib](http://www.nuget.org/packages/Microsoft.OData.Core/) (namespace `Microsoft.OData.Core`): The ODataLib contains classes to serialize, deserialize and validate OData JSON payloads.
- [EdmLib](http://www.nuget.org/packages/Microsoft.OData.Edm/) (namespace `Microsoft.OData.Edm`): The EdmLib contains classes to represent, construct, parse, serialize and validate entity data models.
- [Microsoft.Spatial](http://www.nuget.org/packages/Microsoft.Spatial/) (namespace `Microsoft.Spatial`): The spatial library contains classes and methods that facilitate geography and geometry spatial operations.
- [OData Client for .NET](http://www.nuget.org/packages/Microsoft.OData.Client/) (namespace `Microsoft.OData.Client`): The client library is built on top of ODataLib and EdmLib that has LINQ-enabled client APIs for issuing OData queries and consuming OData JSON payloads.

For these libraries, we accept bug reports and pull requests. The corresponding fixes and implementations will be included into every new release.

Note: Per the [.NET Standard 1.1](https://docs.microsoft.com/en-us/dotnet/articles/standard/library) support chart, OData supports apps for Windows 8.0; however, Visual Studio 2013 was the last version that supported apps for Windows 8.0. Keeping Visual Studio 2013 conflicted with several goals (one of which is to simplify number of installations on a dev machine) and therefore, all tests for Windows 8.0 apps have been removed. Please be forewarned that if you choose to develop applications for Windows 8.0 (which has been superseded by Windows 8.1 and 10), OData no longer offers tests for that platform. You may, however, review the commit history to retrieve the tests housed in `Microsoft.Test.OData.Tests.Client.Portable.WindowsStore.csproj`.

**release branch:**

This branch is for ODataV4 7.x release developed on the [master branch](https://github.com/OData/odata.net/tree/master), contains most recently stable ODataV4 7.x release code base.

**gh-pages branch:**

The gh-pages branch contains documentation source for OData v4 Lib - tutorials, guides, etc.  The documention source is in Markdown format. It is hosted at [ODataLib Pages](http://odata.github.io/odata.net "ODataLib Pages").

**maintenance-6.x branch:** (maintenance mode)

The maintenance-6.x branch includes the .NET libraries for ODataV4 6.x maintenance releases.

**maintenance-5.x branch:** (maintenance mode)

The maintenance-5.x branch includes the .NET libraries for OData V1-3 releases only. It has the similar libraries as the maintenance-6.x branch except for some differences in namespaces and two additional libraries:

- [ODataLib for OData v1-3](http://www.nuget.org/packages/Microsoft.Data.OData/) (namespace `Microsoft.Data.Core`): It contains classes to serialize, deserialize and validate OData payloads. Enables construction of OData producers and consumers.
- [EdmLib for OData v1-3](http://www.nuget.org/packages/Microsoft.Data.Edm/) (namespace `Microsoft.Data.Edm`): It contains classes to represent, construct, parse, serialize and validate entity data models.
- [System.Spatial for OData v1-3](http://www.nuget.org/packages/System.Spatial/) (namespace `System.Spatial`): It contains classes and methods that facilitate geography and geometry spatial operations.
- [WCF Data Services Client for OData v1-3](http://www.nuget.org/packages/Microsoft.Data.Services.Client/) (namespace `Microsoft.Data.Services.Client`): It contains LINQ-enabled client API for issuing OData queries and consuming OData payloads.
- [WCF Data Services Server for OData v1-3](http://www.nuget.org/packages/Microsoft.Data.Services/) (namespace `Microsoft.Data.Services`): Fully-featured server API for responding to OData queries and consuming/producing OData payloads.
- [WCF Data Services EntityFramework Provider](http://www.nuget.org/packages/Microsoft.OData.EntityFrameworkProvider/) (namespace `Microsoft.OData.EntityFrameworkProvider`): Server API for responding to OData queries and consuming/producing OData payloads based on entity framework version 6.0 or higher.

These libraries are in maintenance mode. Only security bugs will be accepted. The release will be irregular depends on the bugs fixed.

**maintenance-wcf-dataservice-v4 branch:** (maintenance mode)

The maintenance-wcf-dataservice-v4 branch has the source code of the OData V4 parity of the WCF Data Services Server for OData v1-3. It is only for cloning and doesn't accept contributions. There is no binary release of it either. WCF DS is not recommended to be adopted now, instead [WebApi](https://github.com/OData/WebApi/ "WebApi") or [RESTier](https://github.com/odata/RESTier/ "RESTier") is recommended to be adopted to build new OData Services.

## 3. Building, Testing, Debugging and Release

In the case of VS2013, [SQL Express 2008](https://www.microsoft.com/en-sg/download/details.aspx?id=30438) or above must be installed. In the case of VS2015, LocalDB v12.0 or above will be used which is part of VS2015 and no additional installation is needed. The Database will be automatically initialized by the test code if it doesn't exist.

Note: The project T4CrossPlatformTests.WindowsStore.csproj will not be loaded unless you have installed the Windows 8.1 and Windows Phone 8.0 / 8.1 tools.

### 3.1 Building and Testing in Visual Studio

Simply open the shortcut `OData.sln` at the root level folder to launch a solution that contains the product source and relevant unit tests. Should you see the need to modify or add additional tests, please see the `sln` folder for the whole set of solution files.

Here is the usage of each solution file (the `OData.sln` shortcut opens the one marked default):

- OData.CodeGen.sln - OData T4 client code generator product source and tests.
- OData.Net35.sln - Product source built with .Net Framework 3.5.
- OData.Net45.sln (default) - Product source built with .Net Framework Portable 4.5 and contains corresponding unit tests. _Recommended_ for doing general bug fixes and feature development.
- OData.NetStandard.sln - Product source built with .Net Standard 1.1.
- OData.Tests.E2E.sln - Product source built with .Net Framework 4.5. Contains exhaustive list of tests (unit, E2E, and regression) and not intended to be opened frequently. The `Build.cmd` script will run all tests from here and this solution is used to _fully_ test your code.
- OData.Tests.E2E.NetCore.VS2017.sln - Product source built with .Net Standard 1.1 and E2E OData Client tests build with .Net Core 1.0-2.0. The purpose of this solution is to house .Net Core E2E test cases. The `Build.cmd` script will run all tests from here and this solution is used to _fully_ test the .Net Standard versions of the product code.
- OData.Tests.NetStandard.VS2017.sln - Product source built with .Net Standard 1.1 and contains corresponding unit tests written in .NET Core. Note that once OData has migrated to VS2017, this solution will replace OData.NetStandard.sln (and take its name) to reduce the number of solutions.
- OData.Tests.Performance.sln - Product source and performance tests built with .Net Framework version 4.6.
- OData.Tests.WindowsApps.sln - Product source and test harnesses written in Windows Portable and Windows Phone 8.1.

Each solution contains some test projects. Please open it, build it and run all the tests in the test explorer. For running tests within OData.Tests.E2E.sln and OData.Tests.E2E.NetCore.VS2017.sln, you need to open Visual Studio IDE as **_Administrator_** so that the test services can be started properly.

### 3.2 One-click build and test script in command line

Open Command Line Window with "**Run as administrator**", `cd` to the root folder and run following command:

`build.cmd`

This will build the full product and run all tests. It will take about 60 minutes. Use the to ensure your change compiles and passes tests before submitting a pull request.

Optionally, you can run following command:

`build.cmd quick`

This will build a single set of product Dlls and run unit tests. It will take about 5 minutes. Use this for quickly testing a change.

Here are some other usages or `build.cmd`:

- `build.cmd` or `build.cmd Nightly` - Build and run all nightly test suites.
- `build.cmd Quick` or `build.cmd -q` - Build and run all unit test suites (with less legacy tests thus faster).
- `build.cmd EnableSkipStrongName` - Configure strong name skip of OData libraries on your machine and build (no test run).
- `build.cmd DisableSkipStrongName` - Disable strong name skip of OData libraries on your machine and build (no test run).

Notes: If there is build error with message "build.ps1 cannot be loaded", right click "build.ps1" -> Properties -> "Unlock".

### 3.3 Debug

Please refer to the [How to debug](http://odata.github.io/WebApi/10-01-debug-webapi-source).

### 3.4 Nightly Builds

The nightly build process will upload a NuGet packages for ODataLib (Core, Edm, Spatial, Client) to the [MyGet.org odlnightly feed](https://www.myget.org/gallery/odlnightly).

To connect to odlnightly feed, use this feed URL: [odlnightly MyGet feed URL](https://www.myget.org/F/odlnightly).

You can query the latest nightly NuGet packages using this query: [MAGIC OData query](https://www.myget.org/F/odlnightly/Packages?$select=Id,Version&$orderby=Version%20desc&$top=4&$format=application/json)

### 3.5 Official Release

The release of the component binaries is carried out regularly through [Nuget](http://www.nuget.org/).

## 4. Documentation

Please visit the [ODataLib pages](http://odata.github.io/odata.net). It has detailed descriptions on each feature provided by OData lib, how to use the OData .Net Client to consume OData service etc.

## 5. Community

### 5.1 Contribution

There are many ways for you to contribute to OData .NET. The easiest way is to participate in discussion of features and issues. You can also contribute by sending pull requests of features or bug fixes to us. Contribution to the documentations is also highly welcomed. Please refer to the [CONTRIBUTING.md](https://github.com/OData/odata.net/blob/master/.github/CONTRIBUTING.md) for more details.

### 5.2 Support

- Issues: Report issues on [Github issues](https://github.com/OData/odata.net/issues).
- Questions: Ask questions on [Stack Overflow](http://stackoverflow.com/questions/ask?tags=odata).
- Feedback: Please send mails to [odatafeedback@microsoft.com](mailto:odatafeedback@microsoft.com).
- Team blog: Please visit [http://blogs.msdn.com/b/odatateam/](http://blogs.msdn.com/b/odatateam/) and [http://www.odata.org/blog/](http://www.odata.org/blog/).

### Thank you

We’re using NDepend to analyze and increase code quality.

[![NDepend](images/ndependlogo.png)](http://www.ndepend.com)
