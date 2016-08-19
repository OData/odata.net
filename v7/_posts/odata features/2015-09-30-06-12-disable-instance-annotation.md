---
layout: post
title: "Disable instance annotation materialization in .NET client"
description: ""
category: "6. OData Features"
---

From 6.12.0, OData .Net client is able to disable instance annotation materialization by turning on the flag `DisableInstanceAnnotationMaterialization` in `DataServiceContext`.

Let's have an example to demonstrate:

The response payload for the example is:

    {
     "@odata.context":"http://localhost/$metadata#People/$entity",
     "PersonID":1,
     "FirstName":"Bob",
     "LastName":"Cat",
     "HomeAddress":{
        "@odata.type":"#Microsoft.Test.OData.Services.ODataWCFService.HomeAddress",
        "@Microsoft.Test.OData.Services.ODataWCFService.AddressType":"Home",
        "Street":"1 Microsoft Way",
        "City":"Tokyo",
        "PostalCode":"98052"
        }
    }

Here we compare the effects by turning off and on the `DisableInstanceAnnotationMaterialization` flag.
{% highlight csharp %}
Context.SendingRequest2 += (sender, eventArgs) => ((HttpWebRequestMessage)eventArgs.RequestMessage).SetHeader("Prefer", "odata.include-annotations=*");

// By default, DisableInstanceAnnotationMaterialization = false
var people = Context.People.ByKey(1).GetValue();
Context.TryGetAnnotation<string>(people.HomeAddress, "Microsoft.Test.OData.Services.ODataWCFService.AddressType", out annotation);
Assert.AreEqual("Home", annotation);

// Here we set DisableInstanceAnnotationMaterialization to true
Context.DisableInstanceAnnotationMaterialization = true;

people = Context.People.ByKey(1).GetValue();
Context.TryGetAnnotation<string>(people.HomeAddress, "Microsoft.Test.OData.Services.ODataWCFService.AddressType", out annotation);
Assert.AreEqual(null, annotation);        // We are not able to get any annotation out. 
{% endhighlight %}
