---
layout: post
title: "3.2 Create spatial instances"
description: "Create spatial instances"
category: "3. Spatial"
---

This section shows how to create spatial instances using Spatial APIs and return them as property values of OData entries.

### Create *GeometryPoint* and *GeographyPoint* instances
In order to use spatial types, please add the following `using` directive:

{% highlight csharp %}
using Microsoft.Spatial;
{% endhighlight %}

The following code shows how to create `GeometryPoint` and `GeographyPoint` instances:

{% highlight csharp %}
// Create a 2D GeometryPoint.
GeometryPoint point1 = GeometryPoint.Create(x: 12.34, y: 56.78);

// Create a 3D GeometryPoint (z is height).
GeometryPoint point2 = GeometryPoint.Create(x: 12.34, y: 56.78, z: 9.0);

// Create a 3D GeometryPoint (m is measures).
GeometryPoint point3 = GeometryPoint.Create(x: 12.34, y: 56.78, z: 9.0, m: 321.0);

// Create a 2D GeographyPoint.
GeographyPoint point4 = GeographyPoint.Create(latitude: 12.34, longitude: 56.78);

// Create a 3D GeographyPoint (z is elevation).
GeographyPoint point5 = GeographyPoint.Create(latitude: 12.34, longitude: 56.78, z: 9.0);

// Create a 3D GeographyPoint (m is measures).
GeographyPoint point6 = GeographyPoint.Create(latitude: 12.34, longitude: 56.78, z: 9.0, m: 321.0);
{% endhighlight %}

Spatial instances can be directly put into `ODataPrimitiveValue` as property values. Using the `Address` type from the last section:

![]({{site.baseurl}}/assets/2015-04-21-csdl.png)

An `ODataResource` for the `Address` type could be constructed as follows:

{% highlight csharp %}
var addressValue = new ODataResource
{
    Properties = new ODataProperty[]
    {
        new ODataProperty { Name = "Street", Value = new ODataPrimitiveValue("Zi Xing Rd") },
        new ODataProperty { Name = "City", Value = new ODataPrimitiveValue("Shanghai") },
        new ODataProperty { Name = "Postcode", Value = new ODataPrimitiveValue("200000") },
        new ODataProperty { Name = "GeometryLoc", Value = new ODataPrimitiveValue(GeometryPoint.Create(12.34, 56.78)) },
        new ODataProperty { Name = "GeographyLoc", Value = new ODataPrimitiveValue(GeographyPoint.Create(12.34, 56.78)) },
    }
};
{% endhighlight %}

### Construct more complex spatial instances
Directly creating these instances using Spatial APIs would be a bit complicated. So we **highly** recommend that you download and add the [SpatialFactory.cs](https://github.com/OData/odata.net/blob/master/test/FunctionalTests/Microsoft.OData.TestCommon/SpatialFactory.cs) file to your project and use the `GeometryFactory` or the `GeographyFactory` class to construct more complex spatial instances.

Here are some sample code of how to use the factory classes to create spatial instances:

{% highlight csharp %}
// Create a GeographyMultiPoint.
GeographyMultiPoint multiPoint = GeographyFactory.MultiPoint().Point(-90.0, 0.0).Point(0.0, 90.0).Build();

// Create a GeometryMultiPolygon.
GeometryMultiPolygon multiPolygon = GeometryFactory.MultiPolygon()
    .Polygon().Ring(-5, -5).LineTo(0, -5).LineTo(0, -2)
    .Polygon().Ring(-10, -10).LineTo(-5, -10).LineTo(-5, -7).Build();

// Create a GeometryLineString.
GeometryLineString lineString = GeometryFactory.LineString(10, 20).LineTo(20, 30).Build();

// Create a GeometryCollection.
GeometryCollection collection = GeometryFactory.Collection()
    .MultiPoint().Point(5, 5).Point(10, 10)
    .LineString(0, 0).LineTo(0, 5)
    .Collection()
        .Point(5, 5);
{% endhighlight %}

More samples could be found in the test cases of the `Microsoft.Spatial.Tests` project. Please find the source code [here](https://github.com/OData/odata.net/tree/master/test/FunctionalTests/Microsoft.Spatial.Tests).

### References
[[Tutorial & Sample] Using Geospatial Data](http://blogs.msdn.com/b/odatateam/archive/2011/10/17/using-geospatial-data.aspx).