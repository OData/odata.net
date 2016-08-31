---
layout: post
title: "Merge complex type and entity type"
description: "Merge complex and entity for OData Serialization and Deserialization"
category: "5. OData Features"
---

From ODataLib 7.0, we merged the public APIs for serializing/deserializing complex values and entities. We did this because complex type and entity type are becoming more and more similar in the protocol, but we continue to pay overhead to make things work for both of them. Since the only really fundamental differences between complex type and entity type at this point are the presence of a key and media link entries, we thought it was best to merge them and just deal with the differences. 

We followed the existing implementation of serializing/deserializing entity instances for both entity instances and complex instances, and the implementation of serializing/deserializing entity collections for both entity collections and complex collections.  This page will provide some simple sample code about how to write these kinds of payload.

# Public APIs used to store complex/entity instances #

`ODataResource` class is used to store information of an entity instance or a complex instance.

`ODataResourceSet` class is used for both a collection of entity or a collection of complex.

`ODataNestedResourceInfo` class is used for both navigation property and complex property. For complex property, this class will be used to store the name of the complex property and a Boolean to indicate whether this property is a single instance or a collection.

For other Public APIs, you can refer to the [Breaking changes about Merge Entity and Complex](#23-17-Merge-Entity-And-Complex-Breaking).


# Model #

Suppose we have a model, in following sections, we will explain how to write/read a complex property or a collection complex property.

```
<?xml version="1.0" encoding="utf-8"?>
<edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx">
  <edmx:DataServices>
    <Schema Namespace="SampleService" xmlns="http://docs.oasis-open.org/odata/ns/edm">
      <ComplexType Name="Location" OpenType="true">
        <Property Name="Address" Type="Edm.String" Nullable="false" />
        <Property Name="City" Type="Edm.String" Nullable="false" />
      </ComplexType>
      <EntityType Name="Person" OpenType="true">
        <Key>
          <PropertyRef Name="UserName" />
        </Key>
        <Property Name="UserName" Type="Edm.String" Nullable="false" />
        <Property Name="HomeAddress" Type="SampleService.Location" />
        <Property Name="OtherAddresses" Type="Collection(SampleService.Location)" />
        </Property>
      </EntityType>
      <EntityContainer Name="DefaultContainer">
        <EntitySet Name="People" EntityType="SampleService.Person" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>
```

# Write a top level complex instance #

In order to write the complex property payload, we need to create an `ODataWriter` first. ODataLib provides `ODataMessageWriter.CreateODataResourceWriter`  to create the writer.

{% highlight csharp %}

    IODataResponseMessage message;
    IEdmStructuredType complexType;
    // Initialize the message and the complexType;
    // message = ...;
    // complexType = ...;
    var settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
    settings.ODataUri = http://localhost/odata/;
    ODataMessageWriter messageWriter = new ODataMessageWriter(message, settings, model);
    var writer = messageWriter.CreateODataResourceWriter(null, complexType);

{% endhighlight %}

Then, we can write a complex property just like writing an entity by using `WriteStart` and `WriteEnd`.

{% highlight csharp %}

    ODataResource complexResource = new ODataResource()
    {
        Properties = new ODataProperty[]
        {
            new ODataProperty { Name = "Address", Value = "Zixing Road" },
            new ODataProperty { Name = "City", Value = "Shanghai" }
        }
    };

    writer.WriteStart(complexResource);
    writer.WriteEnd();

{% endhighlight %}

If we want to write a complex collection property, we can use `CreateODataResourceSetWriter` and write the complex collection property.

{% highlight csharp %}

    writer = messageWriter.CreateODataResourceSetWriter(null, complexType /* item type */);
    ODataResourceSet complexCollection = new ODataResourceSet()
    ODataResource complexResource = new ODataResource()
    {
        Properties = new ODataProperty[]
        {
            new ODataProperty { Name = "Address", Value = "Zixing Road" },
            new ODataProperty { Name = "City", Value = "Shanghai" }
        }
    };

    writer.WriteStart(complexCollection); // write the resource set.
    writer.WriteStart(complexResource); // write each resource.
    writer.WriteEnd(); // end the resource.
    writer.WriteEnd(); // end the resource set.

{% endhighlight %}

# Write a complex property in an entity instance #
To write an entity with a complex property, we can create the `ODataWriter` for the entity by calling `CreateODataResourceWriter`, and then write the entity. we write the complex property just as writing a navigation property.

- Write the property name of the complex property by `WriteStart(ODataNestedResourceInfo nestedResourceInfo)`
- Write the complex instance by `WriteStart(ODataResource resource)`.
- `WriteEnd()` for each part.

Sample:
{% highlight csharp %}

    // Init the entity instance.
    ODataResource entityResource = new ODataResource()
    {
	Properties = new ODataProperty[]
        {...}
    }
    
    // Create the ODataNestedResourceInfo for the HomeAddress property.
    ODataNestedResourceInfo homeAddressInfo = new ODataNestedResourceInfo() { Name = "HomeAddress", IsCollection = false };
    ODataResource complexResource = new ODataResource()
    {
        Properties = new ODataProperty[]
        {
            new ODataProperty { Name = "Address", Value = "Zixing Road" },
            new ODataProperty { Name = "City", Value = "Shanghai" }
        }
    };

    writer.WriteStart(homeAddressInfo); // write the nested resource info. 
    writer.WriteStart(complexResource); // write then resource.
    writer.WriteEnd(); // end the resource.
    writer.WriteEnd(); // end the nested resource info

{% endhighlight %}
To write a complex collection property in an entity, we need to set `ODataNestedResourceInfo.IsCollection` is set to true, and then write the resource set.
{% highlight csharp %}

    // Init the entity instance.
    ODataResource entityResource = new ODataResource()
    {
	Properties = new ODataProperty[]
        {...}
    }

    // Create the ODataNestedResourceInfo for the OtherAddresses property.
    ODataNestedResourceInfo otherAddressesInfo = new ODataNestedResourceInfo() { Name = "OtherAddresses", IsCollection = true };
    ODataResourceSet complexCollection = new ODataResourceSet()
    ODataResource complexResource = new ODataResource()
    {
        Properties = new ODataProperty[]
        {
            new ODataProperty { Name = "Address", Value = "Zixing Road" },
            new ODataProperty { Name = "City", Value = "Shanghai" }
        }
    };

    writer.WriteStart(entityResource); // write the entity instance.
    writer.WriteStart(otherAddressesInfo); // write the nested resource info.
    writer.WriteStart(complexCollection); // write the resource set.
    writer.WriteStart(complexResource); // write each resource.
    writer.WriteEnd(); // end the resource.
    writer.WriteEnd(); // end the resource set.
    writer.WriteEnd(); // end the nested resource info
    writer.WriteEnd(); // end the entity instance.

{% endhighlight %}

Developers can  follow the same way to write a nested complex property in a complex instance. 

# Write complex or entity instances in Uri parameters #

To write an entity (or a complex) instance or a collection of entity (or a collection of complex) in Uri parameters, ODataLib provides `ODataMessageWriter.CreateODataUriParameterResourceWriter` and `ODataMessageWriter.CreateODataUriParameterResourceSetWriter` to create the `ODataWriter`. Then, you can follow the same sample code  in the above two sections to write the related parameters.

# Read a  top level complex instance #

To read a complex (or an entity) instance, ODataLib provides `ODataMessageReader.CreateODataResourceReader` to create the `ODataReader`. 

{% highlight csharp %}

    IODataResponseMessage message;
    IEdmStructuredType complexType;
    // Initialize the message and the complexType;
    // message = ...;
    // complexType = ...;
    var settings = new ODataMessageReaderSettings { EnableMessageStreamDisposal = true };
    ODataMessageReader messageReader = new ODataMessageReader(message, settings, model);
    var reader = messageReader.CreateODataResourceReader(null, complexType);

{% endhighlight %}

Then, developers can read the complex (or entity) instance, nested complex ( or complex collection) properties or navigation properties. 

{% highlight csharp %}

    while (reader.Read())
    {
         switch (reader.State)
         {
             case ODataReaderState.ResourceEnd:
                 // reader.Item is a resource which reprensents the complex instance or its nested resources 
                 // 1. nested complex instance of a complex property
                 // 2. nested complex instances of a complex collection property
                 // 3. entity instances or a navigation property.
                 complex = reader.Item as ODataResource;
                 break;
             case ODataReaderState.ResourceSetEnd:
                 // reader.Item is a resource set which represents a collection of complex or entity.
                 // 1. nested complex collection
                 // 2. navigated entity collection
                 complex = reader.Item as ODataResourceSet;
                 break;
             case ODataReaderState.NestedResourceInfoEnd:
                 // reader.Item is a the nested resource info which represents a complex property, a complex collection property or a navigation property.
                 complex = reader.Item as ODataNestedResourceInfo;
                 break;
             default:
                 break;
         }
    }
{% endhighlight %}

if this reader is created for an entity type, the same code can be used to read an entity instance.

# Read a top level complex collection #

To read a complex collection ( or an entity collection) , ODataLib provides `ODataMessageReader.CreateODataResourceSetReader` to create the `ODataReader`. 

{% highlight csharp %}

    var reader = messageReader.CreateODataResourceSetReader(null, complexType);
    while (reader.Read())
    {
         switch (reader.State)
         {
             case ODataReaderState.ResourceEnd:
                 // reader.Item is a resource which reprensents the complex instance or its nested resources
                 // 0. the top level complex instance.  
                 // 1. nested complex instance of a complex property.
                 // 2. nested complex instances of a complex collection property.
                 // 3. entity instances or a navigation property.
                 complex = reader.Item as ODataResource;
                 break;
             case ODataReaderState.ResourceSetEnd:
                 // reader.Item is a resource set which represents a collection of complex or entity.
                 // 0. the top level complex collection.
                 // 2. netsed complex collection.
                 // 2. navigated entity collection.
                 complex = reader.Item as ODataResourceSet;
                 break;
             case ODataReaderState.NestedResourceInfoEnd:
                 // reader.Item is a the nested resource info which represents a complex property, a complex collection property or a navigation property.
                 complex = reader.Item as ODataNestedResourceInfo;
                 break;
             default:
                 break;
         }
    }
{% endhighlight %}

if this reader is created for an entity collection, the same code can be used to read an entity collection.

# Read complex or entity instances in Uri parameters #

To read an entity (or a complex) instance or a collection of entity (or a collection of complex) in Uri parameters, ODataLib provides `ODataMessageReader.CreateODataUriParameterResourceReader` and `ODataMessageReader.CreateODataUriParameterResourceSetReader` to create the `ODataReader`. Then, you can follow the same sample code in the above two sections to read the related parameters.