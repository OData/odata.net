---
layout: post
title: "$skiptoken & $deltatoken"
description: ""
category: "5. OData Features"
---

From ODataLib 6.12.0, it supports to parse $skiptoken & $deltatoken in query options.

# $skiptoken

Let's have an example:

{% highlight csharp %}
~/Customers?$skiptoken=abc
{% endhighlight %}

We can do as follows to parse:

{% highlight csharp %}
var uriParser = new ODataUriParser(...);
string token = parser.ParseSkipToken();
Assert.Equal("abc", token);
{% endhighlight %}

# $deltatoken

Let's have an example:

{% highlight csharp %}
~/Customers?$deltaToken=def
{% endhighlight %}

We can do as follows to parse:

{% highlight csharp %}
var uriParser = new ODataUriParser(...);
string token = parser.ParseDeltaToken();
Assert.Equal("def", token);
{% endhighlight %}
