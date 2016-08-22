---
layout: post
title: "Use ODataSimplified Convention In ODataUriParser"
description: ""
category: "5. OData Features"
---

From ODataLib 6.14.0, we introduce ODataSimplified convention to make `key-as-segment` and `default` convention work side by side. 

Because when user use key-as-segment convention, url like `/Me/Messages/Microsoft.OutlookServices.EventMessage` will always be parsed by uriParser to `{Singleton}/{Navigation}/{Key}` but what customer needs is `{Singleton}/{Navigation}/{Type}`. When you use ODataSimplified convention, we will try parse type first than key as a default priority to slove this problem.

Turn on ODataSimplified is the same way with key-as-segment:
{% highlight csharp %}
var parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/Schools/1/Student/Microsoft.Test.Taupo.OData.WCFService.Customer")) { UrlConventions = ODataUrlConventions.ODataSimplified };
var result = parser.ParsePath();
{% endhighlight %}

The result will be `Path[(EntitySet: Schools)/(Key: SchoolID = 1)/(NavigationProperty: Student)/(Type: Collection([Microsoft.Test.Taupo.OData.WCFService.Customer Nullable=False]))]`.
