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

<strong>2. Use the new converter in serializer</strong>

The new `ConvertToPayloadValue` would be used to write the DateTimeOffset type value.

{% highlight csharp %}
// Prepare serializer
var messageInfo = new ODataMessageInfo
{
	MessageStream = stream,
	MediaType = new ODataMediaType("application", "json"),
	Encoding = Encoding.Default,
	IsResponse = true,
	IsAsync = false,
	Model = model,
	Container = ContainerBuilderHelper.BuildContainer(
	    builder => builder.AddService<ODataPayloadValueConverter, DateTimeOffsetCustomFormatPrimitivePayloadValueConverter>(ServiceLifetime.Singleton))
};
var context = new ODataJsonLightOutputContext(messageInfo, settings);
var serializer = new ODataJsonLightValueSerializer(context);

// Write primitive value
var df = EdmCoreModel.Instance.GetDateTimeOffset(false);
var value = new DateTimeOffset(2012, 4, 13, 2, 43, 10, TimeSpan.FromHours(8));
serializer.WritePrimitiveValue(value, df);

// Get writer stream
serializer.JsonWriter.Flush();
stream.Position = 0;
var streamReader = new StreamReader(stream);

// Verify the result
streamReader.ReadToEnd().Should().Be("\"Thu, 12 Apr 2012 18:43:10 GMT\"");
{% endhighlight %}

As shown in the code, the new converter is registered to DI through:
{% highlight csharp %}
ContainerBuilderHelper.BuildContainer(
	    builder => builder.AddService<ODataPayloadValueConverter, DateTimeOffsetCustomFormatPrimitivePayloadValueConverter>(ServiceLifetime.Singleton))
{% endhighlight %}

<strong>3. Deserializer</strong>

Deserializer will automatically use the registered new `ConvertFromPayloadValue` to read `Birthday` which is defined as DateTimeOffset in model.

For example, for the payload:

{% highlight csharp %}
const string payload =
"{" +
"\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
"\"@odata.id\":\"http://mytest\"," +
"\"Id\":0," +
"\"Birthday\":\"Thu, 12 Apr 2012 18:43:10 GMT\"" +
"}";
{% endhighlight %}

The payload will be read as an `ODataResource`, and `Birthday` will be read as DateTimeOffset. 

{% highlight csharp %}
IList<ODataProperty> propertyList = odataResource.Properties.ToList();
var birthday = propertyList[1].Value as DateTimeOffset?;
birthday.Value.Should().Be(new DateTimeOffset(2012, 4, 12, 18, 43, 10, TimeSpan.Zero));
{% endhighlight %}
