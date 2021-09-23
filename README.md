﻿﻿# OData .NET Libraries
 Build  | Status
--------|---------
Rolling | <img src="https://identitydivision.visualstudio.com/_apis/public/build/definitions/2cfe7ec3-b94f-4ab9-85ab-2ebff928f3fd/422/badge"/>
Nightly | <img src="https://identitydivision.visualstudio.com/_apis/public/build/definitions/2cfe7ec3-b94f-4ab9-85ab-2ebff928f3fd/107/badge"/>

## 1. Introduction

The [OData .NET Libraries](http://odata.github.io/odata.net) (or OData .NET, for short) project includes the implementation of core functionalities of OData protocol on the .NET platform, including URI parsing, request and response reading and writing, Entity Data Model (EDM) building, and a .Net OData client which can be used to consume an OData service. It is a fully open sourced project maintained by Microsoft OData team. The libraries are used by the [WebApi](https://github.com/OData/WebApi) and [RESTier](https://github.com/odata/RESTier) libraries for building OData Services.

[OData](https://www.odata.org) stands for the Open Data Protocol. It was initiated by Microsoft and is now an [ISO](https://www.oasis-open.org/news/pr/iso-iec-jtc-1-approves-oasis-odata-standard-for-open-data-exchange) ratified [OASIS](http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html) standard. OData enables the creation and consumption of REST APIs, which allow resources, identified using URLs and defined in a data model, to be published and edited by Web clients using simple HTTP requests.

For more information about OData, please refer to the following resources:

- [OData.org](https://www.odata.org/)
- [OASIS Open Data Protocol (OData) Technical Committee](https://www.oasis-open.org/committees/tc_home.php?wg_abbrev=odata)

**For how to adopt this and related libraries to build or consume OData service, please refer to the following resources:**

- [Build an OData v4 Service with RESTier Library](http://odata.github.io/RESTier/#01-01-Introduction)
- [Build an OData v4 Service with OData WebApi Library](https://docs.microsoft.com/en-us/odata/webapi/getting-started)
- [OData .Net Client](https://docs.microsoft.com/en-us/odata/client/getting-started)

## 2. Project structure

The project currently has a  [master](https://github.com/OData/odata.net/tree/master) branch and three archived branches: [maintenance-6.x](https://github.com/OData/odata.net/tree/maintenance-6.x), [maintenance-5.x](https://github.com/OData/odata.net/tree/maintenance-5.x), and [maintenance-wcf-dataservice-v4](https://github.com/OData/odata.net/tree/maintenance-wcf-dataservice-v4).

**master branch:**

The master branch is the active development branch for ODataV4 7.x. It produces libraries targeting .NET 4.5 as well as [.NET Standard 1.1 and 2.0](https://docs.microsoft.com/en-us/dotnet/articles/standard/library). The branch builds with Visual Studio 2019.

For each profile above, it has the following libraries:

- [ODataLib](http://www.nuget.org/packages/Microsoft.OData.Core/) (namespace `Microsoft.OData.Core`): The ODataLib contains classes to serialize and deserialize OData JSON payloads, and to parse OData Urls.
- [EdmLib](http://www.nuget.org/packages/Microsoft.OData.Edm/) (namespace `Microsoft.OData.Edm`): The EdmLib contains classes to represent, construct, parse, serialize and validate entity data models.
- [Microsoft.Spatial](http://www.nuget.org/packages/Microsoft.Spatial/) (namespace `Microsoft.Spatial`): The spatial library contains classes and methods that facilitate geography and geometry spatial operations.
- [OData Client for .NET](http://www.nuget.org/packages/Microsoft.OData.Client) (namespace `Microsoft.OData.Client`): The client library is built on top of ODataLib and EdmLib and provides LINQ-enabled client APIs for issuing OData queries and constructing and consuming OData JSON payloads.

For these libraries, we accept [issue reports](https://github.com/OData/odata.net/issues) and welcome contributions through [pull requests](https://github.com/OData/odata.net/pulls).

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

The maintenance-wcf-dataservice-v4 branch has the source code of the OData V4 parity of the WCF Data Services Server for OData v1-3. It is provide for demo purposes only; it has no binary release and does not accept contributions. [WebApi](https://github.com/OData/WebApi/ "WebApi") or [RESTier](https://github.com/odata/RESTier/ "RESTier") is recommended for building new OData Services.

## 3. Building, Testing, Debugging and Release

Since we are building this on VS2019 LocalDB v12.0 or above will be used which is part of VS2019 and no additional installation is needed. The Database will be automatically initialized by the test code if it doesn't exist.

Note: The project T4CrossPlatformTests.WindowsStore.csproj will not be loaded unless you have installed the Windows 8.1 and Windows Phone 8.0 / 8.1 tools.

### 3.1 Building and Testing in Visual Studio

Simply open the shortcut `OData.sln` at the root level folder to launch a solution that contains the product source and relevant unit tests. Should you see the need to modify or add additional tests, please see the `sln` folder for the whole set of solution files.

Here is the usage of each solution file (the `OData.sln` shortcut opens the one marked default):

- OData.sln - Product source built with .Net Framework 4.5, .Net Standard 1.1 (except for OData Client ), .Net Standard 2.0. Unit Tests built with .Net Framework 4.5.2, .Net Core 1.1, .Net Core 2.1, .Net Core 3.1
- OData.E2E.sln - Product source built with .Net Framework 4.5. Contains exhaustive list of tests (unit, E2E, and regression). The `Build.cmd` script will run all tests from here and this solution is used to _fully_ test your code.

Each solution contains some test projects. Please open it, build it and run all the tests in the test explorer. For running tests within OData.E2E.sln  you need to open Visual Studio IDE as **_Administrator_** so that the test services can be started properly.

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

Notes: If your receive build error with message "build.ps1 cannot be loaded", right click "build.ps1" -> Properties -> "Unlock".
 
### 3.3 Debug

Please refer to the [How to debug](https://docs.microsoft.com/en-us/odata/webapi/debugging).

### 3.4 Nightly Builds

The nightly build process will upload NuGet packages for ODataLib (Core, Edm, Spatial, Client) to the [MyGet.org odlnightly feed](https://www.myget.org/gallery/odlnightly).

To connect to odlnightly feed, use this feed URL: [odlnightly MyGet feed URL](https://www.myget.org/F/odlnightly).

You can query the latest nightly NuGet packages using this query: [MAGIC OData query](https://www.myget.org/F/odlnightly/Packages?$select=Id,Version&$orderby=Version%20desc&$top=4&$format=application/json)

### 3.5 Official Release

The release of the component binaries is carried out regularly through [Nuget](http://www.nuget.org/).

### 3.6 Performance benchmarks

#### Installation

The easiest way to run the perf benchmarks is to use the [Microsoft.Crank](https://github.com/dotnet/crank) toolset.

- Install the [Crank controller](https://www.nuget.org/packages/Microsoft.Crank.Controller), the CLI used to run benchmarks:

    ```text
    dotnet tool install -g Microsoft.Crank.Controller --version "0.2.0-*"
    ```
- Install the [Crank agent](https://www.nuget.org/packages/Microsoft.Crank.Agent), service that executes benchmark jobs. This should be installed on the server(s) where the benchmarks will run. Install locally if you intend to run benchmarks locally.

    ```text
    dotnet tool install -g Microsoft.Crank.Agent --version "0.2.0-*" 
    ```
- Verify installation was complete by running:

    ```text
    crank
    ```

#### Start the agent

- Start the agent by running the following command.:
    
    ```text
    crank-agent
    ```

    Once the agent has started, you should see output like:

    ```text
    Now listening on: http://[::]:5010
    ...
    Agent ready, waiting for jobs...
    ```

#### Run benchmarks locally

- Run benchmarks for different components (reader, writer, batch, URI parser, etc.) using in-memory payloads:
    
    ```text
    crank --config benchmarks.yml --scenario Components --profile local
    ```

- Run benchmarks for end-to-end scenarios against a local OData service:
    
    ```text
    crank --config benchmarks.yml --scenario Service --profile local
    ```

- Run only ODataReader tests:

    ```text
    crank --config benchmarks.yml --scenario Reader --profile local
    ```
- Run only ODataWriter tests:

    ```text
    crank --config benchmarks.yml --scenario Writer --profile local
    ```

- Run only UriParser tests:

    ```text
    crank --config benchmarks.yml --scenario UriParser --profile local
    ```

- Run tests that compare serialization performance of ODataWriter and System.Text.Json

    ```text
    crank --config benchmarks.yml --scenario SerializerBaselines --profile local
    ```

#### Run benchmarks on remote dedicated agents

The `local` profile is provided for testing purposes, but it's not ideal for running benchmarks.
For more stable results and results that we can more reliably compare, the following profiles are also
available and should be preferred whenever possible:

Profile       | Machine Architecture | OS
--------------|----------------------|------
`lab-windows` | INTEL, 12 Cores      | Windows Server 2016
`lab-linux`   | INTEL, 12 Cores      | Ubuntu 18.04, Kernel 4.x

Use the `--profile` argument to specify the profile you want to use. For example,
to run the components benchmark on the Windows agent, run the following command:

```
crank --config benchmarks.yml --scenario Components --profile lab-windows
```

And to run on the Linux agent:

```
crank --config benchmarks.yml --scenario Components --profile lab-linux
```

PS: We should not use these machines to run automated scheduled benchmarks.

#### Run benchmarks against the official repo

To run benchmarks against the official repo instead of your local repo, pass
the `base=true` variable to the command, e.g.:

```text
crank --config benchmarks.yml --scenario Service --profile local --variable base=true
```

This will cause the crank agent to clone the official repo and run the tests against the `master` branch.

You can specify a different branch, commit or tag using the `baseBranch` variable:

```text
crank --config benchmarks.yml --scenario Service --profile local --variable base=true --variable baseBranch=v7.6.4
```

## 4. Documentation

Please visit the [ODataLib docs](https://docs.microsoft.com/en-us/odata/) on docs.microsoft.com. It has detailed descriptions on each feature provided by OData lib, how to use the OData .Net Client to consume OData service etc.

## 5. Community

### 5.1 Contribution

There are many ways for you to contribute to OData .NET. The easiest way is to participate in discussion of features and issues. You can also contribute by sending pull requests of features or bug fixes to us. Contribution to the documentations is also highly welcomed. Please refer to the [CONTRIBUTING.md](https://github.com/OData/odata.net/blob/master/.github/CONTRIBUTING.md) for more details.

### 5.2 Support

- Issues: Report issues on [Github issues](https://github.com/OData/odata.net/issues).
- Questions: Ask questions on [Stack Overflow](http://stackoverflow.com/questions/ask?tags=odata).
- Feedback: Please send mails to [odatafeedback@microsoft.com](mailto:odatafeedback@microsoft.com).
- Team blog: Please visit [https://devblogs.microsoft.com/odata/](https://devblogs.microsoft.com/odata/) and [http://www.odata.org/blog/](http://www.odata.org/blog/).

### Thank you

We’re using NDepend to analyze and increase code quality.

[![NDepend](images/ndependlogo.png)](http://www.ndepend.com)

### Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.


