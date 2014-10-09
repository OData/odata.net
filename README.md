# OData .NET Libraries
## Introduction
The OData .NET Libraries (or OData.NET, for short) project includes the implementation of the OData protocol on the .NET platform. It is a fully open sourced project maintained by the OData team of Microsoft.

OData stands for the Open Data Protocol. It was initiated by Microsoft and is now an OASIS standard. OData enables the creation and consumption of REST APIs, which allow resources, identified using URLs and defined in a data model, to be published and edited by Web clients using simple HTTP messages.

For more information about OData, please refer to the following resources:

 - [OData.org](http://www.odata.org/)
 - [OASIS Open Data Protocol (OData) Technical Committee](https://www.oasis-open.org/committees/tc_home.php?wg_abbrev=odata)

## Project structure
The project currently has three branches: master, ODATAV3, and WCFDSV4. 

### Master branch
The master branch includes the .NET libraries for OData V4 only that are now most actively iterated and maintained by the OData team. It has the following libraries: 

 - [ODataLib](http://www.nuget.org/packages/Microsoft.OData.Core/) (namespace `Microsoft.OData.Core`):<br />
 The ODataLib contains classes to serialize, deserialize and validate OData JSON payloads. 
 - [EdmLib](http://www.nuget.org/packages/Microsoft.OData.Edm/) (namespace `Microsoft.OData.Edm`):<br />
 The EdmLib contains classes to represent, construct, parse, serialize and validate entity data models.
 - [OData Client for .NET](http://www.nuget.org/packages/Microsoft.OData.Client/) (namespace `Microsoft.OData.Client`):<br />
 The client library is built on top of ODataLib and EdmLib that has LINQ-enabled client APIs for issuing OData queries and consuming OData JSON payloads.
 - [Microsoft.Spatial](http://www.nuget.org/packages/Microsoft.Spatial/) (namespace `Microsoft.OData.Client`):<br />
 The spatial library contains classes and methods that facilitate geography and geometry spatial operations.

For these libraries, we accept bug reports, feature requirements and pull requests. The corresponding fixes and implementations will be included into every new release.

The release of the component binaries is carried out monthly through [Nuget](http://www.nuget.org/).

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
This section is being worked on.

### Testing
This section is being worked on.

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
