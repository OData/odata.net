---
layout: post
title: "Centralized control for OData Simplified Options"
description: ""
category: "5. OData Features"
---

In previous OData library, the control of writing/reading/parsing key-as-segment uri path is separated in ODataMessageWriterSettings/ODataMessageReaderSettings/ODataUriParser. The same as writing/reading OData annotation without prefix. From ODataV7.0, we add a centialized control class for them, which is ODataSimplifiedOptions.

The following API can be used in ODataSimplifiedOptions:

{% highlight csharp %}
public class ODataSimplifiedOptions
{
    ...
    public bool EnableParsingKeyAsSegmentUrl { get; set; }
    public bool EnableReadingKeyAsSegment { get; set; }
    public bool EnableReadingODataAnnotationWithoutPrefix { get; set; }
    public bool EnableWritingKeyAsSegment { get; set; }
    public bool EnableWritingODataAnnotationWithoutPrefix { get; set; }
    ...
}
{% endhighlight %}

ODataSimplifiedOptions is registered by DI with singleton ServiceLifetime (Please refer to  [Dependency Injection support](#01-04-di-support)).