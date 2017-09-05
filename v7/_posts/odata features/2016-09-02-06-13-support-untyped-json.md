---
layout: post
title: "Read and Write Untyped values in ODataLib"
description: ""
category: "5. OData Features"
---

Starting with ODataLib 7.0, `ODataMessageReader` & `ODataMessageWriter` are able to read & write untyped primitive, structured, and collection values.  

Values read from a payload are considered untyped if:

1. They represent a property not defined in $metadata and do not contain a property annotation
2. The specified type cannot be resolved against $metadata, or
3. They are explicitly declared as Edm.Untyped, or Collection(Edm.Untyped)

The `ODataMessageReaderSettings` & `ODataMessageWriterSettings` `Validations` property can be set with `~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType` to enable reading/writing undeclared & untyped value properties in a payload. 

Untyped values can be read and written as raw strings or, starting with ODataLib 7.3, can be read and written as structured values.

For compatiblity with ODataLib 7.0, untyped values are read by default as a raw string representing the untyped content. To use the standard OData reader APIs to read untyped content in ODataLib 7.3, set the `ODataMessageReaderSettings.ReadAsString` property to `false`.

Given the following model:

{% highlight csharp %}    

    EdmModel model = new EdmModel();
    var entityType = new EdmEntityType("Server.NS", "ServerEntityType", null, false, false, false);
    entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
    model.AddElement(entityType);

    var container = new EdmEntityContainer("Server.NS", "Container");
    var entitySet = container.AddEntitySet("EntitySet", entityType);
    model.AddElement(container);

{% endhighlight %}

and the following payload, in which the second property (*UndeclaredAddress1*) has a type (*Server.NS.UndefinedAddress*) that is not defined in metadata:

{% highlight csharp %}    

    InMemoryMessage message = new InMemoryMessage();
    const string payload = @"
    {
        ""@odata.context"":""http://www.sampletest.com/$metadata#EntitySet/$entity"",
        ""Id"":61880128,
        ""UndeclaredAddress1"":
        {
            ""@odata.type"":""Server.NS.UndefinedAddress"",
            ""Street"":""No.999,Zixing Rd Minhang"",
            ""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""
        }
    }";

    message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
    message.SetHeader("Content-Type", "application/json");

    ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings
    {
        Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType,
        BaseUri = new Uri("http://www.sampletest.com/")
    };

{% endhighlight %}

the following code will read the content of the second property as a raw string.

{% highlight csharp %}    

    ODataResource entry = null;
    using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, readerSettings, model))
    {
        var reader = msgReader.CreateODataResourceReader(entitySet, entityType);
        while (reader.Read())
        {
            if (reader.State == ODataReaderState.ResourceStart)
			{
				entry = reader.Item as ODataResource;
            }
        }
    }

    Console.WriteLine(entry.Properties.Count()); // 2
    Console.WriteLine((entry.Properties.Last().Value as ODataUntypedValue).RawValue); 
    // @"{""@odata.type"":""Server.NS.UndefinedAddress"",""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}"
    
{% endhighlight %}

By setting `ReadUntypedAsString` to `false`, the same content can be read as a structure value:

{% highlight csharp %}    

    readerSettings.ReadUntypedAsString = false;

    ODataResource entry = null;
    ODataResource address = null;
    using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, readerSettings, model))
    {
        var reader = msgReader.CreateODataResourceReader(entitySet, entityType);
        while (reader.Read())
        {
            if (reader.State == ODataReaderState.ResourceStart)
			{
	            if (entry == null)
                {
					entry = reader.Item as ODataResource;
                }
                else
                {
					address = reader.Item as ODataResource;
                }
			}
        }
    }

    Console.WriteLine(entry.Properties.Count()); // 1
    Console.WriteLine(address.Properties.Count()); //2 
    Console.WriteLine(address.TypeAnnotation.TypeName); //"Server.NS.UndefinedAddress" 
    Console.WriteLine(address.Properties.Last().Value); //"No.10000000999,Zixing Rd Minhang" 

{% endhighlight %}

Note that a new reader state, `ODataReaderState.Primitive`, is added in ODataLib 7.4 in order to support reading primitive values within an untyped collection. Null values within an untyped collection continue to be read as null resources.  

By default, untyped primitive values are returned as boolean if the JSON value is a boolean value, as decimal if the JSON value is numeric, otherwise as string.  A custom primitive type resolver can be used in order to return a more specific type based on the value. If the custom type resolver returns null, then the default resolution is applied. 

For example, the following custom primitive type resolver will recognize strings that look like date, time of day, datetimeoffset, duration, and guid values, will differentiate between integer and decimal values, and will resolve "Male" or "Female" as an enum value from the `Server.NS.Gender` enum type. 

{% highlight csharp %}

	readerSettings.PrimitiveTypeResolver = CustomTypeResolver;

    private static readonly Regex DatePattern = new Regex(@"^(\d{4})-(0?[1-9]|1[012])-(0?[1-9]|[12]\d|3[0|1])$", RegexOptions.Singleline | RegexOptions.Compiled);
    private static readonly Regex TimeOfDayPattern = new Regex(@"^(0?\d|1\d|2[0-3]):(0?\d|[1-5]\d)(:(0?\d|[1-5]\d)(\.\d{1,7})?)?$", RegexOptions.Singleline | RegexOptions.Compiled);
    private static readonly Regex DateTimeOffsetPattern = new Regex(@"^(\d{2,4})-(\d{1,2})-(\d{1,2})(T|(\s+))(\d{1,2}):(\d{1,2})", RegexOptions.Singleline | RegexOptions.Compiled);
    private static readonly Regex DurationPattern = new Regex(@"^P(\d+Y)?(\d+M)?(\d+W)?(\d+D)?(T(\d+H)?(\d+M)?(\d+(\.\d{1,12})?S)?)?$", RegexOptions.Singleline | RegexOptions.Compiled);
    public static IEdmTypeReference CustomTypeResolver(object value, string typeName)
    {
      string stringValue = value as string;
      if (stringValue != null)
      {
        if (DatePattern.IsMatch(stringValue))
        {
           return EdmCoreModel.Instance.GetDate(/*nullable*/ true);
        }
        if (TimeOfDayPattern.IsMatch(stringValue))
        {
           return EdmCoreModel.Instance.GetTimeOfDay(/*nullable*/ true);
        }
        if (DateTimeOffsetPattern.IsMatch(stringValue))
        {
           return EdmCoreModel.Instance.GetDateTimeOffset(/*nullable*/ true);
        }
        if (DurationPattern.IsMatch(stringValue))
        {
           return EdmCoreModel.Instance.GetDuration(/*nullable*/ true);
        }
        Guid guidResult;
        if (Guid.TryParse(stringValue, out guidResult))
        {
           return EdmCoreModel.Instance.GetGuid(/*nullable*/ true);
        }
        if (stringValue == "Male" || stringValue == "Female")
        {
           return new EdmEnumTypeReference(model.FindDeclaredType("Server.NS.Gender") as IEdmEnumType, /*nullable*/ true);
        }
      }
      if (value is int)
      {
        return EdmCoreModel.Instance.GetInt64(/*nullable*/ true);
      }
     return null;
    }

{% endhighlight %}

To write a raw string into the payload, create an `ODataUntypedValue` and set the `RawValue` to the value to be written. Note that there is no validation of the contents of the string written to the payload and it will break clients if it is malformed or if it does not match the expected content-type of the payload.

{% highlight csharp %}

        new ODataProperty
        {
            Name = "UndeclaredAddress1",
            Value = new ODataUntypedValue()
            {
                RawValue=@"{""@odata.type"":""#Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}}"
            }
        }    

{% endhighlight %}

As of ODataLib 7.3, it is possible to write primitive, structured, and collection values to an untyped property, or within an untyped collection. A new `WritePrimitive()` method is added to the `ODataWriter` for writing an `ODataPrimitiveValue` within an untyped collection.

For example, the following writes an untyped collection containing a null, followed by a structured value with a single property, followed by a nested collection containing two primitive values:

{% highlight csharp %}

            writer.WriteStart(new ODataResourceSet
            {
                TypeName = "Collection(Edm.Untyped)"
            });
            writer.WritePrimitive(new ODataPrimitiveValue("CollectionMember1"));
            writer.WriteStart((ODataResource)null);
            writer.WriteEnd(); // null resource
            writer.WriteStart(new ODataResource
            {
                TypeName = "Edm.Untyped",
                Properties = new[]
                    {
                        new ODataProperty {Name = "NestedResourceId", Value = new ODataPrimitiveValue(1)},
                    }
            });
            writer.WriteEnd(); // nested resource
            writer.WriteStart(new ODataResourceSet());
            writer.WritePrimitive(new ODataPrimitiveValue("NestedCollectionMember1"));
            writer.WritePrimitive(new ODataPrimitiveValue("NestedCollectionMember2"));
            writer.WriteEnd(); // nested resource set
            writer.WriteEnd(); // outer resource set

{% endhighlight %}

