---
layout: post
title: "Add additional prefer header"
description: ""
category: "5. OData Features"
---

<strong>[odata.track-changes](http://docs.oasis-open.org/odata/odata/v4.0/errata02/os/complete/part1-protocol/odata-v4.0-errata02-os-part1-protocol-complete.html#_Toc406398239)</strong>, <strong>[odata.maxpagesize](http://docs.oasis-open.org/odata/odata/v4.0/errata02/os/complete/part1-protocol/odata-v4.0-errata02-os-part1-protocol-complete.html#_Toc406398238)</strong>, <strong>[odata.maxpagesize](http://docs.oasis-open.org/odata/odata/v4.0/errata02/os/complete/part1-protocol/odata-v4.0-errata02-os-part1-protocol-complete.html#_Toc406398236)</strong> are supported to add in prefer header since ODataLib 6.11.0.

# Create request message with prefer header

{% highlight csharp %}
var requestMessage = new HttpWebRequestMessage(new Uri("http://example.com", UriKind.Absolute));
requestMessage.PreferHeader().ContinueOnError = true;
requestMessage.PreferHeader().MaxPageSize = 1024;
requestMessage.PreferHeader().TrackChanges = true;
{% endhighlight %}

Then in the http request header, we will have:
`Prefer: odata.continue-on-error,odata.maxpagesize=1024,odata.track-changes`
