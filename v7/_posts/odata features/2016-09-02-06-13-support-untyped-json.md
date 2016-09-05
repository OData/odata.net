---
layout: post
title: "Support undeclared & untyped property value in ODataLib"
description: ""
category: "5. OData Features"
---

From ODataLib 7.0, `ODataMessageReader` & `ODataMessageWriter` are able to read & write arbitrary JSON as raw string in the payload. The undeclared & untyped property is supported by ODataLib in a slightly different way than that in ODataLib 5.x.

The `ODataMessageReaderSettings` & `ODataMessageWriterSettings` `Validations` property can be set with `~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType` to enable reading/writing undeclared & untyped value properties in payload. 

Given the following model.

    EdmModel model = new EdmModel();
    var entityType = new EdmEntityType("Server.NS", "ServerEntityType", null, false, false, false);
    entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
    model.AddElement(entityType);

    var container = new EdmEntityContainer("Server.NS", "Container");
    var entitySet = container.AddEntitySet("EntitySet", entityType);
    model.AddElement(container);

The below `messageWriterSettings` will enable reading undeclared/untyped property. It reads an untype JSON as `ODataUntypedValue`. And `ODataUntypedValue.RawJson` has the raw JSON string.

{% highlight csharp %}    

    InMemoryMessage message = new InMemoryMessage();
    const string payload = @"
    {
        ""@odata.context"":""http://www.sampletest.com/$metadata#EntitySet/$entity"",
        ""Id"":61880128,
        ""UndeclaredAddress1"":
        {
            ""@odata.type"":""Server.NS.AddressInValid"",
            'Street':""No.999,Zixing Rd Minhang"",
            ""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'
        }
    }";

    message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
    message.SetHeader("Content-Type", "application/json");

    ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings
    {
        Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType,
        BaseUri = new Uri("http://www.sampletest.com/")
    };

    ODataResource entry = null;
    using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, readerSettings, model))
    {
        var reader = msgReader.CreateODataResourceReader(entitySet, entityType);
        while (reader.Read())
        {
            entry = reader.Item as ODataResource;
        }
    }

    Console.WriteLine(entry.Properties.Count()); // 2
    // @"{""@odata.type"":""Server.NS.AddressInValid"",""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}"
    Console.WriteLine((entry.Properties.Last().Value as ODataUntypedValue).RawValue); 
    

{% highlight csharp %}

And this is how to write undeclared & untyped property:

{% highlight csharp %}

    MemoryStream outputStream = new MemoryStream();
    IODataResponseMessage message = new InMemoryMessage() { Stream = outputStream };
    message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");

    ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings
    {
        Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType,
        ODataUri = new ODataUri()
        {
            ServiceRoot = new Uri("http://www.sampletest.com/"),
        };
    };

    var entry = new ODataResource
    {
        TypeName = "Server.NS.ServerEntityType",
        Properties = new[]
            {
        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
        new ODataProperty{Name = "UndeclaredFloatId", Value = new ODataPrimitiveValue(12.3D)},
        new ODataProperty
        {
            Name = "UndeclaredAddress1",
            Value = new ODataUntypedValue()
            {
                RawValue=@"{""@odata.type"":""#Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}}"
            }
        },
            }
    };

    using (var msgWriter = new ODataMessageWriter((IODataResponseMessage)message, writerSettings, model))
    {
        var writer = msgWriter.CreateODataResourceWriter(entitySet, entityType);

        writer.WriteStart(entry);
        writer.WriteEnd();
    }
    

{% endhighlight %}
