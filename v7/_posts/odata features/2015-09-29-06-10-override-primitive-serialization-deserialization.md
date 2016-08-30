---
layout: post
title: "Override primitive serialization and deserialization of payload"
description: ""
category: "5. OData Features"
---

Since ODataLib 6.12.0, it supports to customize the payload value converter to override the primitive serialization and deserialization of payload.

### New public API

The new class `ODataPayloadValueConverter` provides a default implementation for value conversion, and also allows developer to override by implemementing `ConvertFromPayloadValue` and `ConvertToPayloadValue`.

{% highlight csharp %}
public class Microsoft.OData.ODataPayloadValueConverter {
	public ODataPayloadValueConverter ()

	public virtual object ConvertFromPayloadValue (object value, Microsoft.OData.Edm.IEdmTypeReference edmTypeReference)
	public virtual object ConvertToPayloadValue (object value, Microsoft.OData.Edm.IEdmTypeReference edmTypeReference)
}
{% endhighlight %}

And in ODataLib 7.0, a custom converter is registered through [DI](http://odata.github.io/odata.net/v7/#01-04-di-support).

### Sample

Here we are trying to override the default converter to support the "R" format of date and time.   

<strong>1. Define DataTimeOffset converter</strong>

{% highlight csharp %}
internal class DateTimeOffsetCustomFormatPrimitivePayloadValueConverter : ODataPayloadValueConverter
{
    public override object ConvertToPayloadValue(object value, IEdmTypeReference edmTypeReference)
    {
        if (value is DateTimeOffset)
        {
            return ((DateTimeOffset)value).ToString("R", CultureInfo.InvariantCulture);
        }

        return base.ConvertToPayloadValue(value, edmTypeReference);
    }

    public override object ConvertFromPayloadValue(object value, IEdmTypeReference edmTypeReference)
    {
        if (edmTypeReference.IsDateTimeOffset() && value is string)
        {
            return DateTimeOffset.Parse((string)value, CultureInfo.InvariantCulture);
        }

        return base.ConvertFromPayloadValue(value, edmTypeReference);
    }
}
{% endhighlight %}

<strong>2. Register new converter to DI container</strong>

{% highlight csharp %}
ContainerBuilderHelper.BuildContainer(
	    builder => builder.AddService<ODataPayloadValueConverter, DateTimeOffsetCustomFormatPrimitivePayloadValueConverter>(ServiceLifetime.Singleton))
{% endhighlight %}

Please refer [here](http://odata.github.io/odata.net/v7/#01-04-di-support) about DI details.

Then DateTimeOffset can be serialized to "Thu, 12 Apr 2012 18:43:10 GMT", and payload like "Thu, 12 Apr 2012 18:43:10 GMT" can be deserialized to DateTimeOffset.
