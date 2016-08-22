---
layout: post
title: "Allow serialization of additional properties"
description: ""
category: "5. OData Features"
---

We are now supporting to serialize addtional properties which are not advertised in Metadata from ODataLib 6.13.0. To achieve this, it is just needed to turn off full validation when creating the `ODataMessageWriterSettings`.

Here is a full example which is trying to write an extra property `Prop1` in the Entry. The implementation of InMemoryMessage in this sample can be found [here](https://github.com/OData/odata.net/blob/ae0dd29c1cf430255a8ec9c4225b4745e25cad64/test/FunctionalTests/Tests/DataOData/Tests/OData.TDD.Tests/Common/InMemoryMessage.cs).

{% highlight csharp %}
//Construct the model
EdmModel model = new EdmModel();
var entityType = new EdmEntityType("Namespace", "EntityType", null, false, false, false);
entityType.AddKeys(entityType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
entityType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(isNullable: true), null);

model.AddElement(entityType);

var container = new EdmEntityContainer("Namespace", "Container");
var entitySet = container.AddEntitySet("EntitySet", entityType);

// Create ODataMessageWriterSettings in which set EnableFullValidation to false
MemoryStream outputStream = new MemoryStream();
IODataResponseMessage message = new InMemoryMessage() { Stream = outputStream };   
message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
ODataUri odataUri = new ODataUri()
{
    ServiceRoot = new Uri("http://example.org/odata.svc"),
};
ODataMessageWriterSettings settings = new ODataMessageWriterSettings()
{
    AutoComputePayloadMetadataInJson = true,
    EnableFullValidation = false,
    ODataUri = odataUri,
};

// Write the payload with extra property "Prop1"
var entry = new ODataEntry
{
    Properties = new[]
    {
        new ODataProperty { Name = "ID", Value = 102 },
        new ODataProperty { Name = "Name", Value = "Bob" },
        new ODataProperty { Name = "Prop1", Value = "Var1" }
    },
};

ODataItem[] itemsToWrite = { entry };            

using (var messageWriter = new ODataMessageWriter(message, settings, model))
{
    ODataWriter writer = messageWriter.CreateODataEntryWriter(entitySet, entityType);
    writer.WriteStart(entry);
    writer.WriteEnd();

    outputStream.Seek(0, SeekOrigin.Begin);
    var output = new StreamReader(outputStream).ReadToEnd();
    Console.WriteLine(output);
    Console.ReadLine();
}
{% endhighlight %}

Then `Prop1` can be shown in the payload:

    {"@odata.context":"http://example.org/odata.svc/$metadata#EntitySet/$entity",
        "ID":102,
        "Name":"Bob",
        "Prop1":"Var1"
    }
