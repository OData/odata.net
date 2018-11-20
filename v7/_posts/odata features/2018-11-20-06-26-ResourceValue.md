---
layout: post
title: "Resource (Complex & Entity) Value"
description: "OData Resource (Complex & Entity) Value"
category: "5. OData Features"
---

# 1. Introduction
## Abstract

`ODataComplexValue` is widely used in OData libraries v5.x and v6.x. However, it’s removed in OData library v7.x because complex type should support the navigation property. We should treat the complex same as the entity. 

So, the main changes OData v7.x design are:

- Remove ODataComplexValue
- Rename ODataEntry as ODataResource, use that to represent the instance of entity and complex.
- Rename ODataFeed as ODataResourceSet, use that to represent the instance of collection of entity or complex.

## Problems

Along with more and more customers upgrade from ODL v6.x to ODL v7.x, many customers find it’s hard to use the library without the `ODataComplexValue`.
Because most of OData customers:

*	Don’t need navigation property on complex type.
*	Can’t convert the instance of entity or complex easily from literal or to literal. Like the Json.Net
*	Can’t create the instance annotation with “complex or entity, or collection of them”
*	Can’t write or read the top-level resource
*	Can’t write or read the top-level property with resource or collection of resource value
*	Can’t read the parameter resource or collection of resource value directly

# 2. Proposal

## Current structure

Below is the main inheritance of the ODataValue vs ODataItem in ODL v6.x.

![]({{site.baseurl}}/assets/2018-11-20-ODLv6xInheritance .png)

Below is the main inheritance of the ODataValue vs ODataItem in ODL v7.x.

![]({{site.baseurl}}/assets/2018-11-20-ODLv7xInheritance1Class.png)

The main changes from 6.x to 7.x is:

*	`ODataComplexValue` is removed.
*	`ODataValue` is derived from `ODataItem`.

## Proposal structure

We should introduce a new class named `ODataResourceValue` derived from `ODataValue`, same as:


## 3. Main Works

### `ODataResourceValue` class

We will introduce a new class named `ODataResourceValue`, it should look like (Same as `ODataComplexValue`):

{% highlight csharp %}

public sealed class ODataResourceValue : ODataValue
{
  public string TypeName { get; set; }
  public IEnumerable<ODataProperty> Properties { get; set; }
  public ICollection<ODataInstanceAnnotation> InstanceAnnotations { get;set; }
}

{% endhighlight %}

Where:
  * **TypeName**: save the resource type name.
  *	**Properties**: save the all properties, include the property with resource value or collection of resource value.
  *	**InstanceAnnotations**: save the instance annotations for this resource value.

### `ODataCollectionValue` class
We don’t need to change anything for `ODataCollectionValue`, because it also supports to have `ODataResourceValue` as its element.

### `ODataProperty` class
We don’t need to change anything for `ODataProperty`, because it also supports to create `ODataProperty` with `ODataResourceValue` or `ODataCollectionValue`.

### `ODataResource` class

We don’t need to change anything in `ODataResource` except to verify the properties don’t include any property whose value is an `ODataResourceValue` or Collection of ODataResourceValue.

{% highlight csharp %}

public class ODataResource
{
   ...
   public IEnumerable<ODataProperty> Properties
   {
      get { return this.MetadataBuilder.GetProperties(this.properties); }
      set
      {
          // TODO: Add validation here. It's not allowed a property with value as "ODataResourceValue" or a collection of "ODataResourceValue"
          this.properties = value;
      }
    } 
}

{% endhighlight %}

### `ODataResourceSet` class

We don’t need to change anything in `ODataResourceSet`.

## Write `ODataResourceValue`

### Write in value writer

#### Write `ODataResourceValue`

We should add a new method named `WriteResourceValue(…)` to write an `ODataResourceValue` in *[ODataJsonLightValueSerializer](https://github.com/OData/odata.net/blob/master/src/Microsoft.OData.Core/JsonLight/ODataJsonLightValueSerializer.cs)*.

{% highlight csharp %}
public void WriteResourceValue(ODataResourceValue resourceValue, …)
{
    // TODO
}
{% endhighlight %}
This method is the key method in writing scenario.	

#### Write `ODataCollectionValue` with `ODataResourceValue`

We should update `WriteCollectionValue(...)` method to call above `WriteResourceValue(...)` if the item is an `ODataResourceValue`.

{% highlight csharp %}
foreach (object item in items)
{
     ODataResourceValue itemAsResourceValue = item as ODataResourceValue;
     if (itemAsResourceValue != null)
     {
        this.WriteResourceValue(…);
     }
     else
     {
        …// primitive, enum
     }
}
{% endhighlight %}

### Write in property Writer

We should update *[ODataJsonLightPropertyWriter.cs](https://github.com/OData/odata.net/blob/master/src/Microsoft.OData.Core/JsonLight/ODataJsonLightPropertySerializer.cs)* to support writing the property with `ODataResourceValue` or collection of resource value.
This change should in `WriteProperty(...)` method, it supports to write top-level property and non-top-level property (Nested Property).
{% highlight csharp %}
private void WriteProperty(…)
{
   ......
   ODataResourceValue resourceValue = value as ODataResourceValue;
   if (resourceValue != null)
   {
       this.JsonWriter.WriteName(propertyName);
       this.JsonLightValueSerializer.WriteResourceValue(resourceValue, …);
       return;
    }
}
{% endhighlight %}
We don’t need to change any codes for property with value as `ODataCollectionValue` which element is `ODataResourceValue`.

### Write in instance annotation writer

We should update *[JsonLightInstanceAnnotationWriter.cs](https://github.com/OData/odata.net/blob/master/src/Microsoft.OData.Core/Json/JsonLightInstanceAnnotationWriter.cs)* to support:
#### Write the `ODataInstanceAnnotation` with value as `ODataResourceValue`

We should add the below codes in `WriteInstanceAnnotation(...)` method by calling the `WriteResourceValue(...)` if the instance annotation value is an `ODataResourceValue`.
{% highlight csharp %}
ODataResourceValue resourceValue = value as ODataResourceValue;
if (resourceValue != null)
{
   this.WriteInstanceAnnotationName(propertyName, name);
   this.valueSerializer.WriteResourceValue(resourceValue,…);
   return;
}
{% endhighlight %}

#### Write the `ODataInstanceAnnotation` with value as collection of `ODataResourceValue`.
We don’t need to do anything. Because it supports to write the `ODataCollectionValue`, which in turns will call `WriteResourceValue()` for each `ODataResourceValue` elements.

### Write in collection writer

We should update *[ODataJsonLightCollectionWriter.cs](https://github.com/OData/odata.net/blob/master/src/Microsoft.OData.Core/JsonLight/ODataJsonLightCollectionWriter.cs)* to support write collection with `ODataResourceValue` item.

{% highlight csharp %}
protected override void WriteCollectionItem(object item, IEdmTypeReference expectedItemType)
{
    ODataResourceValue resourceValue = item as ODataResourceValue;
    if (resourceValue != null)
    {
        this.jsonLightCollectionSerializer.WriteResourceValue(resourceValue,…);
    }
…
}
{% endhighlight %}

### Write in parameter writer
We should update *[ODataJsonLightParameterWriter.cs](https://github.com/OData/odata.net/blob/master/src/Microsoft.OData.Core/JsonLight/ODataJsonLightParameterWriter.cs)* to support write resource or collection of resource value.

{% highlight csharp %}
protected override void WriteValueParameter(string parameterName, object parameterValue, IEdmTypeReference expectedTypeReference)
{
    ......
    ODataResourceValue resourceValue = parameterValue as ODataResourceValue;
    if (resourceValue != null)
    {
        this.jsonLightValueSerializer.WriteResourceValue(resourceValue, ….);                      
    }
}
{% endhighlight %}

**TBD**: 
Normally, if you want to write a Collection parameter, you should do:
{% highlight csharp %}
var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
parameterWriter.WriteStart();
var collectionWriter = parameterWriter.CreateCollectionWriter("collection");
        collectionWriter.WriteStart(new ODataCollectionStart());
          collectionWriter.WriteItem("item1");
        collectionWriter.WriteEnd();
 parameterWriter.WriteEnd();
{% endhighlight %}
However, i think we should support to write the collection value directly if customer call `WriteValueParameter()` method with the `ODataCollectionValue`.  

Basically, we don’t need to change any codes for the “Collection parameter value” writer. Customer still can use “CreateCollectionWriter” to write the collection with more information.

Besides, We don’t need to change any codes for Resource or ResourceSet parameter writer. Customer still can use them to writer `ODataResource` or `ODataResourceSet` one by one. See:
*	CreateFormatResourceWriter
*	CreateFormatResourceSetWriter

### Convert `ODataResourceValue` to Uri literal

We should support to convert `ODataResourceValue` and collection of it to Uri literal when customer call `ConvertToUriLiteral(...)` in *[ODataUriUitl.cs](https://github.com/OData/odata.net/blob/master/src/Microsoft.OData.Core/Uri/ODataUriUtils.cs#L109)*.

{% highlight csharp %}
public static string ConvertToUriLiteral(object value, ODataVersion version, IEdmModel model)
{
   ......
	 ODataResourceValue resourceValue = value as ODataResourceValue;
   if (resourceValue != null)
   {
      return ODataUriConversionUtils.ConvertToResourceLiteral(resourceValue, model, version);
   }

   ......
}
{% endhighlight %}

## Read `ODataResourceValue`
### Read ODataResourceValue in value reader

We should update *[ODataJsonLightPropertyAndValueDeserialier.cs](https://github.com/OData/odata.net/blob/master/src/Microsoft.OData.Core/JsonLight/ODataJsonLightPropertyAndValueDeserializer.cs)* to read the resource value.
{% highlight csharp %}
private ODataResourceValue ReadResourceValue(
            bool insideJsonObjectValue,
            bool insideComplexValue,
            string propertyName,
            IEdmStructuredTypeReference structuredTypeReference,
            string payloadTypeName,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector)
{…}
{% endhighlight %}
This method is the key method in reading scenario, it should support to:
* Read its own instance annotation
* Read all properties value, include nested resource value.

The above method is called from:

{% highlight csharp %}
private object ReadNonEntityValueImplementation(…)
{
    ......

    case EdmTypeKind.Complex: // nested complex
    case EdmTypeKind.Entity: // nested entity
         ......
	 result = ReadResourceValue(......);
	 break;
}
{% endhighlight %}

For the collection of resource, owing that `ReadCollectionValue()` will call `ReadNonEntityValueImplemenation(…)` to read its elements, so, if the item is `entity or complex`, it will return `DataResourceValue`. We don’t need to change any codes.

### Read ODataResourceValue in instance annotation reader

*[ODataJsonLightPropertyAndValueDeserialier.cs](https://github.com/OData/odata.net/blob/master/src/Microsoft.OData.Core/JsonLight/ODataJsonLightPropertyAndValueDeserializer.cs)* has the following method to read instance annotation value:
{% highlight csharp %}
internal object ReadCustomInstanceAnnotationValue (PropertyAndAnnotationCollector propertyAndAnnotationCollector, string name)
{
object customInstanceAnnotationValue = this.ReadNonEntityValueImplementation(…);
}
{% endhighlight %}
So, we don’t need to change any codes for it.

### Read ODataResourceValue in collection reader
*[ODataJsonLightCollectionDeserializer.cs](https://github.com/OData/odata.net/blob/master/src/Microsoft.OData.Core/JsonLight/ODataJsonLightCollectionDeserializer.cs)* will call `ReadNonEntityValueImplementation`. We don’t need change any code.

However, there’s some validation codes that need to change.

### Read ODataResourceValue as OData error value

*[ODataJsonLightErrorDeserializer.cs](https://github.com/OData/odata.net/blob/master/src/Microsoft.OData.Core/JsonLight/ODataJsonLightErrorDeserializer.cs)* will call `ReadNonEntityValueImplementation`. We don’t need change any code.

### Read in Resource deserializer

*[ODataJsonLightResourceDeserializer.cs](https://github.com/OData/odata.net/blob/master/src/Microsoft.OData.Core/JsonLight/ODataJsonLightResourceDeserializer.cs)* will call `ReadNonEntityValueImplementation` to read the value of its property.
However, 
* It is not used to read the “complex and collection of complex”
* It is not used to read the “navigation property”
* It’s ONLY used it to read the primitive, enum and collection of them.
And, for the “complex and collection of complex”, we still create nested resource info. So, we don’t need to change anything.

### Read resource in parameter

*[ODataJsonLightParameterDeserialzer.cs](https://github.com/OData/odata.net/blob/master/src/Microsoft.OData.Core/JsonLight/ODataJsonLightParameterDeserializer.cs)* is used to read parameter value. So far, for the entity, complex, it only returns a parameter state as “Resource”, for the collection of them, return a parameter state of “ResourceSet” as below:

* Primitive type, read as primitive value.
* Enum type, read as “ODataEnumValue”
* TypeDefintion, read as “TypeDefinition” value
* Complex, Entity, read nothing, just return “ReaderState.Resource”
* Collection, 
  * If element is primitive, read as primitive value.
  * If element is enum, read nothing, just return “ReaderState.Collection”
  * If element is complex, entity, read nothing, just return “ReaderState.ResourceSet”.
So, we should have a configuration enable customer to change the logic.
For example: On ODataMessageReader, we can enable customer to create a parameter reader which can read all parameter as value.
{% highlight csharp %}
public ODataParameterReader CreateODataParameterReader(IEdmOperation operation, bool readAllAsValue)
{
  ......
}
{% endhighlight %}
So, if customer call the above method using *true* for `readAllAsValue`, he can get:
* Complex, Entity, read as “ODataResourceValue”
* Collection, read as “ODataCollectionValue”.

### Read Top-Level Property

*[ODataJsonLightPropertyAndValueDeserializer.cs](https://github.com/OData/odata.net/blob/master/src/Microsoft.OData.Core/JsonLight/ODataJsonLightPropertyAndValueDeserializer.cs)* can read the top-level property into a ODataProperty. So, we can read a top-level complex, entity, or collection or complex, entity property.

### Convert ODatResourceValue from Url literal 
We should convert the `ODataResourceValue` from JSON Uri literal in `ConvertFromUriLiteral(...)` in *[ODataUriUtils.cs](https://github.com/OData/odata.net/blob/master/src/Microsoft.OData.Core/Uri/ODataUriUtils.cs#L44)*.


## 4. Open Questions

### What’s the string output if convert a null “ODataCollectionValue”?
So far, if you create:
{% highlight csharp %}
ODataCollectionValue value = null;
String str = ConvertToUriLiteral(value, ODataVersion.V4, model);
Assert.Equal("null", str); // true?
{% endhighlight %}
However, it should be "[]" ? 

### Do we write the instance annotation if call `ConvertToUriLiteral`?
In the 6.x version, if a complex value has instance annotations, those instance annotations will not write out when we call like:
{% highlight csharp %}
ODataComplexValue  value = new ODataComplexValue()
{
       TypeName = "TestModel.Address",
       Properties = new ODataProperty[] { new ODataProperty() { Name = "Street", Value = "street" }} ,
       InstanceAnnotations = new[]
           {
                   new ODataInstanceAnnotation("Custom.Ok", new ODataComplexValue
                     {
                          TypeName = "TestModel.Address",
                          Properties = new ODataProperty[] { new ODataProperty() { Name = "Street", Value = "street" }})
                        }
           }

string str = ODataUriUtils.ConvertToUriLiteral(value, ODataVersion.40, model);
{% endhighlight %}

Where, `str` doesn’t include the instance annotation? But, we should include the instance annotation.
