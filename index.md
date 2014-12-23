---
layout: default
---

# Getting started

OData is an [OASIS standard](https://www.oasis-open.org/committees/tc_home.php?wg_abbrev=odata) for creating and consuming RESTful APIs. The OData .NET libraries implement the OData version 4.0.

To build an OData V4 **client application**, start by following:

- ["How to use OData Client Code Generator to generate client-side proxy class"](http://blogs.msdn.com/b/odatateam/archive/2014/03/12/how-to-use-odata-client-code-generator-to-generate-client-side-proxy-class.aspx)

To build an OData V4 **service**, start by following:

- ["Create an OData v4 Endpoint Using ASP.NET Web API OData"](http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/odata-v4/create-an-odata-v4-endpoint)

To learn about OData first:

- ["Open Data Protocol by Example"](http://msdn.microsoft.com/en-us/library/ff478141.aspx)
- ["Learning OData in 6 steps"](http://www.odata.org)
- [Protocol specifications](https://www.oasis-open.org/committees/tc_home.php?wg_abbrev=odata#technical)

# Introduction to libraries and tooling

**Libraries**

- Core libraries
	- [**ODataLib**](http://www.nuget.org/packages/Microsoft.OData.Core/), namespace `Microsoft.OData.Core`. It contains classes to serialize, deserialize and validate OData JSON payloads.
	- [**EdmLib**](http://www.nuget.org/packages/Microsoft.OData.Edm/), namespace `Microsoft.OData.Edm`. It contains classes to represent, construct, parse, serialize and validate entity data models.
	- [**Microsoft.Spatial**](http://www.nuget.org/packages/Microsoft.Spatial/), namespace `Microsoft.Spatial`. It contains classes and methods that facilitate geography and geometry spatial operations.
    - [**OData Client for .NET**](http://www.nuget.org/packages/Microsoft.OData.Client/), namespace `Microsoft.OData.Client`. It contains LINQ-enabled client APIs for issuing OData queries and consuming OData JSON payloads.
- [**ASP.NET Web API OData**](http://www.nuget.org/packages/Microsoft.AspNet.OData/), namespace `System.Web.OData`. It contains everything you need to create OData v4.0 endpoints using ASP.NET Web API and to support OData query syntax for your web APIs.

**Tooling**

- [**OData v4 Client Code Generator**](https://visualstudiogallery.msdn.microsoft.com/9b786c0e-79d1-4a50-89a5-125e57475937) (An item template that simplifies the process of accessing OData v4 services by generating C# and VB.Net client-side proxy classes)

**Relationship**

![Client side]({{site.baseurl}}/assets/library-relationship.png)

# Samples

- [ASP.NET Web API OData](https://aspnet.codeplex.com/SourceControl/latest#Samples/WebApi/OData/v4/)
- [Sample services, clients and others](https://github.com/OData/ODataSamples)
- [OData Team's MSDN blog](http://blogs.msdn.com/b/odatateam/)
- [StackOverflow](http://stackoverflow.com/questions/tagged/odata)

# Source code

- [Core libraries](https://github.com/odata/odata.net)
- [ASP.NET Web API OData](http://aspnetwebstack.codeplex.com/SourceControl/latest#OData/)