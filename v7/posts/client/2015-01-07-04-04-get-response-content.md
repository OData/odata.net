---
layout: post
title: "Get Response Content for Data Modification Requests"
description: ""
category: "4. Client"
---

When the service doesn't respond with `204 No Content` to data modification requests, the response contains a non-empty body. The code below helps to retrieve the body content:

{% highlight csharp %}
static void Main(string[] args)
{
    var context = new DefaultContainer(new Uri("http://services.odata.org/v4/(S(b0vguruqwzqbmfoanwq1guxc))/TripPinServiceRW/"));

    var person = Person.CreatePerson("russell", "Russell", "Whyte", new long());

    context.AddToPeople(person);

    var responses = context.SaveChanges();

    foreach (var response in responses)
    {
        var changeResponse = (ChangeOperationResponse) response;
        var entityDescriptor = (EntityDescriptor) changeResponse.Descriptor;
        var personCreated = (Person) entityDescriptor.Entity; // the person created on the service
    }
}
{% endhighlight %}
