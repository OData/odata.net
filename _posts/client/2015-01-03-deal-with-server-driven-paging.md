---
layout: post
title: "Deal with server-driven paging"
description: ""
category: "4. Client"
---

The OData Client for .NET deals with server-driven paging with the help of `DataServiceQueryContinuation` and `DataServiceQueryContinuation<T>`. They are classes that contain the next link of the partial set of items.

Example:

{% highlight csharp %}
var context = new DefaultContainer(new Uri("http://services.odata.org/v4/TripPinServiceRW/"));

// DataServiceQueryContinuation<T> contains the next link
DataServiceQueryContinuation<Person> token = null;

// Get the first page
var response = context.People.Execute() as QueryOperationResponse<Person>;

// Loop if there is a next link
while ((token = response.GetContinuation()) != null)
{
    // Get the next page
    response = context.Execute<Person>(token);
}
{% endhighlight %}
