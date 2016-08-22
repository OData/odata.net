---
layout: post
title: "Override primitive serialization and deserialization of payload"
description: ""
category: "6. OData Features"
---

Since ODataLib 6.12.0, it supports to customize the payload value converter to override the primitive serialization and deserialization of payload.

### New public API

The new class `ODataPayloadValueConverter` provides a default implementation for value conversion, and also allows developer to override by implemementing `ConvertFromPayloadValue` and `ConvertToPayloadValue`.

{% highlight csharp %}
public class Microsoft.OData.Core.ODataPayloadValueConverter {
	public ODataPayloadValueConverter ()

	public virtual object ConvertFromPayloadValue (object value, Microsoft.OData.Edm.IEdmTypeReference edmTypeReference)
	public virtual object ConvertToPayloadValue (object value, Microsoft.OData.Edm.IEdmTypeReference edmTypeReference)
}
{% endhighlight %}

New helper functions in `ODataObjectModelExtensions` to set converter to model so the new converter can be applied:

{% highlight csharp %}
public sealed class Microsoft.OData.Core.ODataObjectModelExtensions {
  ...
  public static Microsoft.OData.Core.ODataPayloadValueConverter GetPayloadValueConverter (Microsoft.OData.Edm.IEdmModel model)
  public static void SetPayloadValueConverter (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Core.ODataPayloadValueConverter converter)
  ...
}
{% endhighlight %}

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

<strong>2. Set converter to model</strong>

{% highlight csharp %}
model.SetPayloadValueConverter(new DateTimeOffsetCustomFormatPrimitivePayloadValueConverter());
{% endhighlight %}

<strong>3. Serialize</strong>

The new `ConvertToPayloadValue` would be used to write the DateTimeOffset type value.

{% highlight csharp %}
var df = EdmCoreModel.Instance.GetDateTimeOffset(false);

var result = this.SetupSerializerAndRunTest(serializer =>
{
    var value = new DateTimeOffset(2012, 4, 13, 2, 43, 10, TimeSpan.FromHours(8));
    serializer.WritePrimitiveValue(value, df);
});

result.Should().Be("\"Thu, 12 Apr 2012 18:43:10 GMT\"");
{% endhighlight %}

<strong>4. Deserialize</strong>

The new `ConvertFromPayloadValue` would be used to read `Birthday` which is defined as DateTimeOffset in model.

{% highlight csharp %}
const string payload =
"{" +
"\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
"\"@odata.id\":\"http://mytest\"," +
"\"Id\":0," +
"\"Birthday\":\"Thu, 12 Apr 2012 18:43:10 GMT\"" +
"}";

ODataEntry entry = null;
this.ReadEntryPayload(model, payload, entitySet, entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });

IList<ODataProperty> propertyList = entry.Properties.ToList();
var birthday = propertyList[1].Value as DateTimeOffset?;
birthday.Value.Should().Be(new DateTimeOffset(2012, 4, 12, 18, 43, 10, TimeSpan.Zero));
{% endhighlight %}
