---
layout: post
title: "Get Response Content for Data Modification Requests"
description: ""
category: Code Generated Client
---

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
