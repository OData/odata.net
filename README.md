# OData .NET Libraries
## 1. Introduction
The [OData .NET Libraries](http://odata.github.io/odata.net) (or OData .NET, for short) project includes the implementation of core functionalities of OData protocol on the .NET platform which includes URI parsing, request and response reading and writing, Entity Data Model (EDM) building, and also a .Net OData client which can be used to consume OData service. It is a fully open sourced project maintained by Microsoft OData team. The libraries are used by [WebApi](https://github.com/OData/WebApi/ "WeiApi") and [RESTier](https://github.com/odata/RESTier/ "RESTier") which are recommended to be adopted to build new OData Services.

[OData](http://www.odata.org/ "OData") stands for the Open Data Protocol. It was initiated by Microsoft and is now an ISO and OASIS standard. OData enables the creation and consumption of REST APIs, which allow resources, identified using URLs and defined in a data model, to be published and edited by Web clients using simple HTTP requests.

For more information about OData, please refer to the following resources:
- [OData.org](http://www.odata.org/)
- [OASIS Open Data Protocol (OData) Technical Committee](https://www.oasis-open.org/committees/tc_home.php?wg_abbrev=odata)

**For how to adopt this and related libraries to build or consume OData service, please refer to the following resources:**
- [Build an OData v4 Service with RESTier Library](http://odata.github.io/RESTier/#01-01-Introduction)
- [Build an OData v4 Service with OData WebApi Library](http://odata.github.io/WebApi/#01-02-getting-started)
- [OData .Net Client](http://odata.github.io/odata.net/#04-01-basic-crud-operations)

## 2. Project structure
The project currently has six branches: [ODataV4-6.x](https://github.com/OData/odata.net/tree/ODataV4-6.x), [ODataV4-7.x](https://github.com/OData/odata.net/tree/ODataV4-7.x), [master](https://github.com/OData/odata.net/tree/master), [gh-pages](https://github.com/OData/odata.net/tree/gh-pages), [ODATAV3](https://github.com/OData/odata.net/tree/ODATAV3), and [WCFDSV4](https://github.com/OData/odata.net/tree/WCFDSV4).



**ODataV4-6.x branch**

The ODataV4-6.x branch includes the .NET libraries for OData V4 only that are now most actively iterated and maintained by the OData team, it has newest OData lib 6.x release code base. It has the following libraries:
- [ODataLib](http://www.nuget.org/packages/Microsoft.OData.Core/) (namespace `Microsoft.OData.Core`):<br />The ODataLib contains classes to serialize, deserialize and validate OData JSON payloads.
- [EdmLib](http://www.nuget.org/packages/Microsoft.OData.Edm/) (namespace `Microsoft.OData.Edm`):<br />The EdmLib contains classes to represent, construct, parse, serialize and validate entity data models.
- [Microsoft.Spatial](http://www.nuget.org/packages/Microsoft.Spatial/) (namespace `Microsoft.Spatial`):<br />The spatial library contains classes and methods that facilitate geography and geometry spatial operations.
- [OData Client for .NET](http://www.nuget.org/packages/Microsoft.OData.Client/) (namespace `Microsoft.OData.Client`):<br />The client library is built on top of ODataLib and EdmLib that has LINQ-enabled client APIs for issuing OData queries and consuming OData JSON payloads.

For these libraries, we accept bug reports and pull requests. The corresponding fixes and implementations will be included into every new release.

**ODataV4-7.x branch**

The ODataV4-7.x branch includes the .NET libraries for OData V4 only that are in development now. It is evolved from ODataV4-6.x with kinds of improvements, new features and so on. It is in-compatible with ODataV4-6.x release and will become the master branch after it is released. It has same libraries as ODataV4-6.x branch.

For these libraries, we accept feature requirements, but we do not accept bug reports and pull requests before first release is ready.

**Master branch**

The master branch has most recently stable 6.x release code base, and is recommended to use before OData lib 7.x is released.

**gh-pages branch**

The gh-pages branch contains documentation source for OData v4 Lib - tutorials, guides, etc.  The documention source is in Markdown format. It is hosted at [ODataLib Pages](http://odata.github.io/odata.net "ODataLib Pages").

**ODATAV3 branch** (maintenance mode)

The ODATAV3 branch includes the .NET libraries for OData V1-3 only. It has the similar libraries as the master branch except for some differences in namespaces and two additional libraries:
- [ODataLib for OData v1-3](http://www.nuget.org/packages/Microsoft.Data.OData/) (namespace `Microsoft.Data.Core`): <br />It contains classes to serialize, deserialize and validate OData payloads. Enables construction of OData producers and consumers.
- [EdmLib for OData v1-3](http://www.nuget.org/packages/Microsoft.Data.Edm/) (namespace `Microsoft.Data.Edm`): <br />It contains classes to represent, construct, parse, serialize and validate entity data models.
- [System.Spatial for OData v1-3](http://www.nuget.org/packages/System.Spatial/) (namespace `System.Spatial`): <br />It contains classes and methods that facilitate geography and geometry spatial operations.
- [WCF Data Services Client for OData v1-3](http://www.nuget.org/packages/Microsoft.Data.Services.Client/) (namespace `Microsoft.Data.Services.Client`): <br />It contains LINQ-enabled client API for issuing OData queries and consuming OData payloads.
- [WCF Data Services Server for OData v1-3](http://www.nuget.org/packages/Microsoft.Data.Services/) (namespace `Microsoft.Data.Services`): <br />Fully-featured server API for responding to OData queries and consuming/producing OData payloads.
- [WCF Data Services EntityFramework Provider](http://www.nuget.org/packages/Microsoft.OData.EntityFrameworkProvider/) (namespace `Microsoft.OData.EntityFrameworkProvider`): <br />Server API for responding to OData queries and consuming/producing OData payloads based on entity framework version 6.0 or higher.

These libraries are in maintenance mode. Only security bugs will be accepted. The release will be irregular depends on the bugs fixed.

**WCFDSV4 branch** (maintenance mode)  

The WCFDSV4 branch has the source code of the OData V4 parity of the WCF Data Services Server for OData v1-3. It is only for cloning and doesn't accept contributions. There is no binary release of it either. WCF DS is not recommended to be adopted now, instead [WebApi](https://github.com/OData/WebApi/ "WeiApi") or [RESTier](https://github.com/odata/RESTier/ "RESTier") is recommended to be adopted to build new OData Services.

## 3. Building, Testing, Debugging and Release
In the case of VS2013, [SQL Express 2008](https://www.microsoft.com/en-sg/download/details.aspx?id=30438) or above must be installed. In the case of VS2015, LocalDB v12.0 or above will be used which is part of VS2015 and no additional installation is needed. The Database will be automatically initialized by the test code if it doesn't exist.

Note: The project T4CrossPlatformTests.WindowsStore.csproj will not be loaded unless you have installed the Windows 8.1 and Windows Phone 8.0 / 8.1 tools.

### 3.1 Building and Testing in Visual Studio
Simply open the solution files under 'sln' folder and build them in Visual Studio 2013 or 2015.

Here is the usage of each solution file:
- Microsoft.OData.Lite.sln - Product source and minimal functional tests. _Recommended_ for doing general bugfix and feature development. It is built with .Net Framework Portable version 4.0.
- Microsoft.OData.Full.sln - Product source and full functional tests (excluding test cases in Lite and E2E solution). Used to _fully_ test your code.
- Microsoft.OData.E2E.sln - Product source and end-to-end tests.
- Microsoft.OData.Net35.sln - Solution with product source built with .Net Framework version 3.5.
- Microsoft.OData.Net45.sln - Solution with product source built with .Net Framework version 4.5.
- Microsoft.OData.Portable45.sln - Solution with product source built with .Net Framework Portable version 4.5.
- Microsoft.OData.Performance.sln - Product source and performance tests built with .Net Framework version 4.6.
- Microsoft.OData.CodeGen.sln - Source and tests for OData T4 client code generator.

Each solution contains some test projects. Please open it, build it and run all the tests in the test explorer. For running tests within Microsoft.OData.Full.sln and Microsoft.OData.E2E.sln, you need to open Visual Studio IDE as **_Administrator_** so that the test services can be started properly.

### 3.2 One-click build and test script in command line
Open Command Line Window with "**Run as administrator**", cd to the root folder and run following command:

```
build.cmd
```

The build and test will take about 15 minutes. Here are some other usages:
- `build.cmd Nightly`. Build and run all nightly test suites.
- `build.cmd Rolling`. Build and run all rolling test suites (with less legacy tests thus faster).
- `build.cmd SkipStrongName`. Configure strong name skip of OData libraries on your machine and build (no test run).
- `build.cmd DisableSkipStrongName`. Disable strong name skip of OData libraries on your machine and build (no test run).

Notes: If there is build error with message "build.ps1 cannot be loaded", right click "build.ps1" -> Properties -> "Unlock".

### 3.3 Debug
Please refer to the [How to debug](http://odata.github.io/WebApi/10-01-debug-webapi-source).

### 3.4 Nightly Builds
We keep uploading the daily nightly signed NuGet packages for ODataLib/EdmLib/ClientLib/SpatialLib to [our MyGet feed](https://www.myget.org/F/odlnightly).

You can query the latest nightly NuGet packages using [this MAGIC OData query](https://www.myget.org/F/odlnightly/Packages?$select=Id,Version&$orderby=Version%20desc&$top=4&$format=application/json)

### 3.5 Official Release
The release of the component binaries is carried out regularly through [Nuget](http://www.nuget.org/).

## 4. Documentation
Please visit the [ODataLib pages](http://odata.github.io/odata.net). It has detailed descriptions on each feature provided by OData lib, how to use the OData .Net Client to consume OData service etc.

## 5. Community
### 5.1 Contribution
There are many ways for you to contribute to OData .NET. The easiest way is to participate in discussion of features and issues. You can also contribute by sending pull requests of features or bug fixes to us. Contribution to the documentations is also highly welcomed. Please refer to the [CONTRIBUTING.md](https://github.com/OData/odata.net/blob/master/.github/CONTRIBUTING.md) for more details.

### 5.2 Support
- Issues<br />Report issues on [Github issues](https://github.com/OData/odata.net/issues).
- Questions<br />Ask questions on [Stack Overflow](http://stackoverflow.com/questions/ask?tags=odata).
- Feedback<br />Please send mails to [odatafeedback@microsoft.com](mailto:odatafeedback@microsoft.com).
- Team blog<br />Please visit [http://blogs.msdn.com/b/odatateam/](http://blogs.msdn.com/b/odatateam/) and [http://www.odata.org/blog/](http://www.odata.org/blog/).