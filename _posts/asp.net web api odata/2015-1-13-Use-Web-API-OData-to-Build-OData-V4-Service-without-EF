---
title : "Use Web API OData to build an OData V4 service without Entity Framework"
layout: post
category: ASP.NET Web API OData
---

There are quite a lot of [tutorials](http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/odata-v4/create-an-odata-v4-endpoint) showing how to create OData services using Web API OData, but these requires Entity Framework and a database server behind. If you want a quick try or you have your own way of implementing data sources, these tutorials may not be the best fit. In this article, we will show how to build an OData service using **in-memory data** as data source and with basic function.

#### Create the solution
Create a new solution following File -> New -> Project -> Web, then choose `ASP.NET Web Application`. Name it with `Demo`. Then in the upcoming dialogue box, choose `Empty` and check `Web API`, click OK. 

#### Install NuGet packages
Run the following command in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console).

`PM> Install-Package Microsoft.AspNet.OData`

#### Add Models
In this getting-started example, we just add two model class `Person.cs` and `Trip.cs` under folder `Models`. `Person` can navigate to `Trips`.
```
using System;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class Person
    {
        [Key]
        public String ID { get; set; }

        [Required]
        public String Name { get; set; }
        
        public String Description { get; set; }

        public List<Trip> Trips { get; set; }
    }
}
```
```
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Demo.Models
{
    public class Trip
    {
        [Key]
        public String ID { get; set; }
        [Required]
        public String Name { get; set; }
    }
}
```


The attributes `[Key]` and `[Required]` are all from `System.ComponentModel.DataAnnotations` meaning the property is key and required seperately.

#### In-Memory data source
This tutorial uses in-memory data source, which is more flexible. Below are only one way to implement, you can definitely have your only way.

Add a folder `DataSource` and add a class file `DemoDataSources.cs` with the code below.
```
using Demo.Models;
using System.Collections.Generic;

namespace Demo.DataSource
{
    public class DemoDataSources
    {
        private static DemoDataSources instance = null;
        public static DemoDataSources Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DemoDataSources();
                }
                return instance;
            }
        }

        public List<Person> People { get; set; }

        private DemoDataSources()
        {
            this.Reset();
            this.Initialize();
        }

        public void Reset()
        {
            this.People = new List<Person>();
        }

        public void Initialize()
        {
            this.Trips.AddRange(new List<Trip>()
            {
                new Trip()
                {
                    ID = "0",
                    Name = "Trip 0"
                },
                new Trip()
                {
                    ID = "1",
                    Name = "Trip 1"
                },
                new Trip()
                {
                    ID = "2",
                    Name = "Trip 2"
                },
                new Trip()
                {
                    ID = "3",
                    Name = "Trip 3"
                }
                
            });
            this.People.AddRange(new List<Person>
            {
                new Person()
                {
                    ID = "001",
                    Name = "Angel",
                    Trips = new List<Trip>{Trips[0], Trips[1]}
                },
                new Person()
                {
                    ID = "002",
                    Name = "Clyde",
                    Description = "Contrary to popular belief, Lorem Ipsum is not simply random text.",
                    Trips = new List<Trip>{Trips[2], Trips[3]}
                },
                new Person()
                {
                    ID = "003",
                    Name = "Elaine",
                    Description = "It has roots in a piece of classical Latin literature from 45 BC, making Lorems over 2000 years old."
                }
            });
        }
    }
}
```
#### Add Controllers
Since there are two entity set, we will add two controller class under the folder `Controllers`

```
using Demo.DataSource;
using System.Linq;
using System.Web.Http;
using System.Web.OData;

namespace Demo.Controllers
{
    [EnableQuery]
    public class PeopleController : ODataController
    {
        public IHttpActionResult Get()
        {
            return Ok(DemoDataSources.Instance.People.AsQueryable());
        }
    }
}
```

```
using Demo.DataSource;
using System.Linq;
using System.Web.Http;
using System.Web.OData;

namespace Demo.Controllers
{
    public class TripsController : ODataController
    {
        public IHttpActionResult Get()
        {
            return Ok(DemoDataSources.Instance.Trips.AsQueryable());
        }
    }
}

```

In this very simple implementation, only simple `Get` with query options are allowed. If you want to enable more capabilities in your controller, the code is quite similar with what's done with EF as data source. Please refer to 
[ASP.NET Web API OData V4 Samples](https://aspnet.codeplex.com/SourceControl/latest#Samples/WebApi/OData/v4/) and [Create an OData v4 Endpoint Using ASP.NET Web API 2.2](http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/odata-v4/create-an-odata-v4-endpoint). 

#### Configure the Endpoint
The last step is to modify the `WebApiConfig.cs` file under `App_Start`.

```
using Demo.Models;
using Microsoft.OData.Edm;
using System.Web.Http;
using System.Web.OData.Batch;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
namespace Demo
{
    public static class WebApiConfig
    {

        public static void Register(HttpConfiguration config)
        {
            config.MapODataServiceRoute("odata", null, GetEdmModel(), new DefaultODataBatchHandler(GlobalConfiguration.DefaultServer));
            config.EnsureInitialized();
        }

        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.Namespace = "Demos";
            builder.ContainerName = "DefaultContainer";

            builder.EntitySet<Person>("People");
            builder.EntitySet<Trip>("Trips");

            var edmModel = builder.GetEdmModel();

            return edmModel;
        }
    }
}
```

#### Try with it
It's done to create a very basic OData endpoint with in-memory data source using Web API OData. 
You can try it out now! All samples below are all `GET` method.

Service document

`http://localhost:[portNumber]/`

Service metadata

`http://localhost:21830/$metadata`

Get People

`http://localhost:21830/People`

Queries 

`http://localhost:21830/People?$filter=contains(Description,'Lorem')`

`http://localhost:21830/People?$select=Name`

`http://localhost:21830/People?$expand=Trips`





