---
layout: post
title: "Allow serialization of additional properties"
description: ""
category: "5. OData Features"
---

We now support serializing additional properties which are not advertised in metadata since ODataLib 6.13.0. To achieve this, users just need to turn off the `ThrowOnUndeclaredPropertyForNonOpenType` validation setting when constructing `ODataMessageWriterSettings`.

Here is a full example which is trying to write an extra property `Prop1` in the entity. The implementation of `InMemoryMessage` used in this sample can be found [here](https://github.com/OData/odata.net/blob/ODataV4-7.x/test/FunctionalTests/Microsoft.OData.Core.Tests/InMemoryMessage.cs).

{% highlight csharp %}
//Construct the model
EdmModel model = new EdmModel();
var entityType = new EdmEntityType("Namespace", "EntityType", null, false, true, false);
entityType.AddKeys(entityType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32, false));
entityType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(isNullable: true), null);

model.AddElement(entityType);

var container = new EdmEntityContainer("Namespace", "Container");
var entitySet = container.AddEntitySet("EntitySet", entityType);

// Create ODataMessageWriterSettings
MemoryStream outputStream = new MemoryStream();
IODataResponseMessage message = new InMemoryMessage { Stream = outputStream };
message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
ODataUri odataUri = new ODataUri
{
    ServiceRoot = new Uri("http://example.org/odata.svc")
};
ODataMessageWriterSettings settings = new ODataMessageWriterSettings
{
    ODataUri = odataUri
};
settings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;

// Write the payload with extra property "Prop1"
var entity = new ODataResource
{
    Properties = new[]
    {
        new ODataProperty { Name = "ID", Value = 102 },
        new ODataProperty { Name = "Name", Value = "Bob" },
        new ODataProperty { Name = "Prop1", Value = "Var1" }
    }
};

using (var messageWriter = new ODataMessageWriter(message, settings, model))
{
    ODataWriter writer = messageWriter.CreateODataResourceWriter(entitySet, entityType);
    writer.WriteStart(entity);
    writer.WriteEnd();

    outputStream.Seek(0, SeekOrigin.Begin);
    var output = new StreamReader(outputStream).ReadToEnd();
    Console.WriteLine(output);
    Console.ReadLine();
}
{% endhighlight %}

`Prop1` will appear in the payload:

{% highlight json %}
{
    "@odata.context":"http://example.org/odata.svc/$metadata#EntitySet/$entity",
    "ID":102,
    "Name":"Bob",
    "Prop1":"Var1"
}
{% endhighlight %}
