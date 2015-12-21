---
layout: post
title: "7.3 Navigation Property in Complex Type Design"
description: "Design doc for navigation property, complex type"
category: "7. Design"
---
< < Inital draft, Improvment frequently > >

# 1	Design Summary
## 1.1 Overview
This doc describes the design about supporting the navigation property in complex type. It is related to the following components/libraries:

* OData CSDL & Edm
* OData Core
* OData Client
* Web API

## 1.2 Goals/Scopes

* Edm
  - Construct and validate navigation property in complex type from EdmModel.
  - Define navigation property in complex type, include the navigation property binding.
  - Read/write navigation property in complex type from/to CSDL.

* Core
  - Parse navigation property in complex type in UriParser.
  - Serialize/Deserialize navigation property in complex type from/to payload.

* Client
  - Track complex type with navigation property.
  - Gen navigation property in complex type codes.

* Web API
  - Build EdmModel with navigation property in complex type.
  - Provide the routing logic for navigation property in complex type.
  - Serialization/ Deserialization navigation property in complex type in payload

## 1.3 Non-Goals

* Containment navigation property in complex type.
*	Dynamic navigation property in open type
*	Update navigation property in Collection complex property

# 2	Design Details
## 2.1	CSDL and EDM
### 2.1.1	Construct navigation property in complex type

From [OData Spec](http://docs.oasis-open.org/odata/odata/v4.0/errata02/os/complete/part1-protocol/odata-v4.0-errata02-os-part1-protocol-complete.html#_Toc406398201):
  *	_**Entity types**_ are named structured types with a key. They define the named properties and relationships of an entity.
  
  *	_**Complex types**_ are keyless named structured types consisting of a set of properties.
  
So, both entity types and complex types are structured types with properties, include declared properties and navigation properties. Below picture shows the class relationship between complex type and entity type, navigation property and structural property in ODataLib so far.

![]({{site.baseurl}}/assets/2015-12-21-ODL-Class-Relationship.png)

From above picture, we can find that the DeclaredProperties of IEdmStruturedType is defined as a List of IEdmProperty, which can hold either the navigation property or the structural property. So from interface perspective, the navigation property in complex type is supported without any change.

However, we need to do as follows to allow the customer to define navigation property on complex type:

1 Promote the following public/private APIs from EdmEntityType to EdmStructuredType.

```C#
public EdmNavigationProperty AddUnidirectionalNavigation(EdmNavigationPropertyInfo propertyInfo)
public EdmNavigationProperty AddBidirectionalNavigation(EdmNavigationPropertyInfo propertyInfo, EdmNavigationPropertyInfo partnerInfo)
private EdmNavigationPropertyInfo FixUpDefaultPartnerInfo(EdmNavigationPropertyInfo propertyInfo, EdmNavigationPropertyInfo partnerInfo)
```
Then, developers can call as follows to define the navigation property on complex type, for example:
```C#
EdmEntityType customer = new EdmEntityType(“NS”, “Customer”);
….
EdmComplexType address = new EdmComplexType("NS", "Address");
…
address.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
{
     Name = "Customer",
     TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
     Target = customer
});
```

2 Modify `EdmNavigationProperty` class.

  *	a) Change the private constructor to accept the IEdmStructuredType.
  *	b) Change DeclaringEntityType property as following: 
```C#
public IEdmEntityType DeclaringEntityType
{
  get
  {
    if (DeclaringType is IEdmEntityType)
    {
      return (IEdmEntityType)this.DeclaringType;
    }
    return null;
  }
}
```
  *	c) Add a similar property named DeclaringComplexType, the implementation is same as #b.
  *	d) Add new public APIs as follows:
```C#
public static EdmNavigationProperty CreateNavigationProperty(IEdmComplexType declaringType, EdmNavigationPropertyInfo propertyInfo)
private static EdmNavigationProperty CreateNavigationProperty(IEdmStructuredType declaringType, EdmNavigationPropertyInfo propertyInfo)
```
  *	e) Make the original CreateNavigationProperty() function and new added public API for complex type to call the new added private function, same as follows:
  ```C#
public static EdmNavigationProperty CreateNavigationProperty(IEdmEntityType declaringType, EdmNavigationPropertyInfo propertyInfo)
{
  return CreateNavigationProperty((IEdmStructuredType)declaringType, propertyInfo);
}
```

3	Add the following extension methods:
```C#
public static IEnumerable<IEdmNavigationProperty> DeclaredNavigationProperties(this IEdmComplexType type)
public static IEnumerable<IEdmNavigationProperty> NavigationProperties(this IEdmComplexType type)
public static IEnumerable<IEdmNavigationProperty> NavigationProperties(this IEdmComplexTypeReference type)
public static IEnumerable<IEdmNavigationProperty> DeclaredNavigationProperties(this IEdmComplexTypeReference type)
public static IEdmNavigationProperty FindNavigationProperty(this IEdmComplexTypeReference type, string name)
```

### 2.1.2	Write navigation property in complex type in CSDL

There is a logic to write the complex type and its declared properties. We can add the navigation properties writing logic after writing declared properties.  So, We should change the function **ProcessComplexType()** in _EdmModelCsdlSerializationVisitor_ class as follows to write navigation properties in complex type:
```C#
protected override void ProcessComplexType(IEdmComplexType element)
{
  this.BeginElement(element, this.schemaWriter.WriteComplexTypeElementHeader);

  this.VisitProperties(element.DeclaredStructuralProperties());
  this.VisitProperties(element.DeclaredNavigationProperties());

  this.EndElement(element);
}
```
Then, the complex type in metadata document may have navigation property. Let’s have an example:
```xml
  <ComplexType Name="Address">
    <Property Name="Street" Type="Edm.String" />
    …
    <Property Name="Country" Type="Edm.String" />
    <NavigationProperty Name="Customer" Type="NS.Customer" />
  </ComplexType>
```
### 2.1.3	Read navigation property in complex type in CSDL

Reading/Parse the navigation property in complex type is a lit bit complex. We can analysis the entity type and complex type class inheritance in CSDL. Below picture shows the class relationship between CSDL complex type and CSDL entity type. Both are derived from _CsdlNamedStructuredType_, then derived from _CsdlStructuredType_:

![]({{site.baseurl}}/assets/2015-12-21-CSDL-Class-Relationship.png)

So, we should modify as follows:

* Promote everything about the navigation property from CsdlEntityType to CsdlStructuredType.

* Modify the constructors of CsdlStructuredType, CsldNamedStructuredType, CsdlComplexType to accept the navigation properties. For example:
```C#
protected CsdlNamedStructuredType(string name, string baseTypeName, bool isAbstract, bool isOpen, IEnumerable<CsdlProperty> properties,   IEnumerable<CsdlNavigationProperty> navigationProperties, CsdlDocumentation documentation, CsdlLocation location)
  : base(properties, navigationProperties, documentation, location)
{
  …
}
```
* Modify **CreateRootElementParser()** function in _CsdlDocumentParser_. Add the following templates for Complex type element:
```C#
//// <NavigationProperty>
CsdlElement<CsdlNamedElement>(CsdlConstants.Element_NavigationProperty, this.OnNavigationPropertyElement, documentationParser,
  //// <ReferentialConstraint/>
  CsdlElement<CsdlReferentialConstraint>(CsdlConstants.Element_ReferentialConstraint, this.OnReferentialConstraintElement, documentationParser),
  //// <OnDelete/>
  CsdlElement<CsdlOnDelete>(CsdlConstants.Element_OnDelete, this.OnDeleteActionElement, documentationParser),
    //// <Annotation/>
    annotationParser),
```

* Modify CsdlSemanticsNavigationProperty class to accept CsdlSemanticsStructuredTypeDefinition.

*	Override the ComputeDeclaredProperties() function in CsdlSemanticsComplexTypeDefinition

### 2.1.4	Construct navigation property binding in complex type

OData spec says:
```TXT
13.4.1 Attribute Path
A navigation property binding MUST name a navigation property of the entity set’s, singleton's, or containment navigation property's entity type or one of its subtypes in the Path attribute. If the navigation property is defined on a subtype, the path attribute MUST contain the QualifiedName of the subtype, followed by a forward slash, followed by the navigation property name. If the navigation property is defined on a complex type used in the definition of the entity set’s entity type, the path attribute MUST contain a forward-slash separated list of complex property names and qualified type names that describe the path leading to the navigation property.
```
From the highlight part, we can find that the property path is necessary for navigation property binding in complex type.  So, we should save the property path for navigation property in complex type. Let’s have an example to illustrate the property path for navigation property in complex type.
```C#
Customer (Entity)
{
  …
  Address Location;
}

Address (Complex)
{
  …
  City City;
}

City (Entity)
{…}

EntitySet: Customers (Customer)
EntitySet: Cities (City)
```
The binding path of the navigation property _“City”_ of entity set _“Customers”_ should be **“Location/NS.Address/City”**. Or we can just remove the type cast if it is not the sub type as **“Location/City”**.
As a result, we should add a new public API for _EdmNavigationSource_ class to let customer to define the property path for navigation property in complex type:
```C#
public void AddNavigationTarget(IEdmNavigationProperty property, IEdmNavigationSource target, IList<IEdmStructuralProperty> path)
or
public void AddNavigationTarget(IEdmNavigationProperty property, IEdmNavigationSource target, IList<string> path)

```
Let’s have a detail example to illustrate how the users (developers) to add the navigation binding:

1)	Add a complex type:
```C#
// complex type address
 EdmComplexType address = new EdmComplexType("NS", "Address");
 address.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
      …
 model.AddElement(address);
 ```
 2)	Add “Customer” entity type:
```C#
EdmEntityType customer = new EdmEntityType("NS", "Customer");
customer.AddKeys(customer.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
…
var location = customer.AddStructuralProperty("Location", new EdmComplexTypeReference(address, isNullable: true));
model.AddElement(customer);
```
3)	Add “City” entity type
```C#
EdmEntityType city = new EdmEntityType("NS", "City");
city.AddKeys(city.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
…
model.AddElement(city);	
```
4)	Add a navigation property for “Address” complex type
```C#
EdmNavigationProperty addressNavProp = address.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
{
Name = "City",
TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
Target = city
});
```
5)	Add an entity set and the navigation property binding
```C#
EdmEntityContainer container = new EdmEntityContainer("NS", "Default");
model.AddElement(container);
EdmEntitySet customers = container.AddEntitySet("Customers", customer);
EdmEntitySet cities = container.AddEntitySet(“Cities”, city);
customers.AddNavigationTarget(addressNavProp, cities, new[] { location });
```
6)	Therefore, we can have the navigation property binding as:
```xml
  <EntityContainer Name="Default">
    <EntitySet Name="Customers" EntityType="NS.Customer">
      <NavigationPropertyBinding Path="Location/NS.Address/City" Target="Cities" />
    </EntitySet>
  </EntityContainer>
```
### 2.1.5	Validation rules for navigation property in complex type

There’re a lot of validation rules related to navigation property, entity type and complex type. So, we should:

* Remove the old validation rules which disallow the navigation property in complex type.  If any, we have to remove the validation methods from active rules. We can’t remove the validation methods because they are public.
* Add new validation rules for navigation property in complex type, since it is a bit different with navigation property in entity type, including:
  -	Partner MUST NOT be specified for navigation property of complex type, according to the spec.
  -	ContainsTarget is not true for navigation property of complex type, since we are not going to enable it now.
  
## 2.2	OData Core
### 2.2.1	Uri Parser
The navigation property in complex type can be in path/segment or query option. We have to support all of these. Let’s see some Uri templates for navigation property in complex type:

*	Navigation property segment of complex type:
  - ~/entityset/key/complexproperty/navigation
  - ~/entityset/key/complexproperty /…/navigation
  - ~/entityset/key/complexproperty /…/navigation/property
  - ~/entityset/key/complexproperty /…/navigation/$count  (for collection)
* Function/action after navigation property segment of complex type:
  - ~/entityset/key/complexproperty /…/navigation/boundfunction
  - ~/entityset/key/complexproperty /…/navigation/boundaction
* Navigation property in complex type in query options
  - $expand=property/navigationproperty
  - $select=property/navigationproperty

#### 2.2.1.1	Parse Path Segments
We can use the existing classes _NavigationPropertySegment_ and _NavigationPropertyLinkSegment_ to represent the navigation property of complex type without any change.

However, we should pass the previous navigation source from structural property to the navigation property belong to this.  So, we should change the **CreatePropertySegment()** function in _ODataPathParser_ to save the previous navigation source in property segment as follows:
```C#
private void CreatePropertySegment(ODataPathSegment previous, IEdmProperty property, string queryPortion)
{
  …
  segment.TargetEdmNavigationSource = previous.TargetEdmNavigationSource;
  …
}
```

Let’s have a request example:
```C#
http://localhost/Orders(1)/Location/City
```
Then, the result of Uri parser can be:
```C#
~/EntitySetSegment/KeySegment/PropertySegment/NavigationPropertySegment
```
#### 2.2.1.2	Parse query option

So far, SelectExpandBinder only supports the following expand clause:
* $expand=NavigationProperty[,…]
* $expand=TypeCast/NavigationProperty/$ref
*	…

As navigation property be allowed in complex type, the _SelectExpandBinder_ should support more expand and select clauses as follows:
*	~/../ complexproperty?$select=navigation
*	$expand= complexproperty /navigation
*	$expand= complexproperty /typecast/ navigation
*	~/../ complexproperty?$expand=navigation
*	~/../ complexproperty?$expand=navigation

So, we should modify the codes as follows:

1. Add a flag for Uri resolver to configure whether the navigation property is allowed in complex type
2. Modify GenerateExpandItem(ExpandTermToken) function in SelectExpandBinder
3. Modify SelectEpxandPathBinder to add a new function to process the property segment in expand clause.

Let’s have an example: ** $expand=Location/City **
```C#
IDictionary<string, string> queryOptions = new Dictionary<string, string>
  {
    { "$expand", "Location/City" }
  };
var _order = _model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(e => e.Name == "Order");
var _orders = _model.FindDeclaredEntitySet("Orders");
var parser = new ODataQueryOptionParser(_model, _order, _orders, queryOptions);

var selectAndExpand = parser.ParseSelectAndExpand();
```
Then, _selectAndExpand_ has one SelectedItems with the following OData path with segments:

1. PropertySegment
2. NavigationPropertySegment 

### 2.2.2	Serialize navigation property in complex type payload
#### 2.2.2.1	Serialization process

Let’s take a look about the serialization flow about navigation property in entity type. Below picture shows the simple flow about the serialization of entry, includes a) entry without expanded navigation property, b) entry with expanded navigation properties.

![]({{site.baseurl}}/assets/2015-12-21-ODL-Serialize-flow.png)

From the picture, we can find that the serialization flow is more complicated if entry with expanded navigation property. Moreover, there’s no way to expand the navigation property in complex type property, owing that the complex type property is serialized completely in WriteStart process for entry same as other structural properties.
So, as navigation property be allowed in complex type, we should stop the write process for entry once a complex type property with navigation property is met. Then, we can use the process same as navigation property in entity type to write the complex property with expanded navigation property.
So far, we have the following proposal, (other proposal please refer to appendix):
Create a new class, for example ODataExpandableProperty, and add write start API on this class. For example:
```C#
public void WriteStart(ODataExpandableProperty property);
```

#### 2.2.2.2	Single expandable property in entry
Let’s see how to serialize the entry with complex type property in which the navigation properties are expanded.  Based on the above proposal, the basic serialization flow for entry with expandable property with expanded navigation property should be as follows:

![]({{site.baseurl}}/assets/2015-12-21-Expandable-Serialize-flow.png)

The corresponding server side codes to serialize the navigation property in complex type should be as:
```C#
ODataEntry entry1 = new ODataEntry();
entry1.AddProperty(property); // normal properties
…
ODataExpandableProperty expandableProperty = new ODataExpandableProperty();// the property with the NP.
ODataEntry entry2 = new ODataEntry(); // Expanded entry belongs to above expandable property
ODataNavigationLink navigationLink =  …
writer.WriteStart(entry1); // write start of entry1 and normal properties
writer.WriteStart(expandableProperty); // write the expandable property
    writer.WriteStart(navigationLink);
        	writer.WriteStart(entry2); 
writer.WriteEnd();   // End entry2
         writer.WriteEnd();  // End navigationLink
    writer.WriteEnd();  // End pr expandableProperty
writer.WriteEnd(); // End entry1
```
So, we can do as follows:	
1.	Create a new class
```C#
public sealed class ODataExpandableProperty : ODataItem
{
   …
}
```
Basically, _ODataExpandableProperty_ can have the same structure of _ODataProperty_, but it should be derived from _ODataItem_, or it can be embedded with _ODataProperty_.
2.	Add two new abstract APIs in ODataWirter
```C#
public abstract void WriteStart(ODataExpandableProperty property);
public abstract Task WriteStartAsync(ODataExpandableProperty property);
```
The users (developers, service) can call these APIs to write the expandable property.
3.	In ODataWriterCore, give an implementation for the above new abstract APIs.  
4.	Add new item in WriterState enum type:
```C#
internal enum WriterState
{
…
  /// <summary>The writer is currently writing an expandable property.</summary>
  ExpanableProperty,
…
}
```
5.	Add two new abstract API as follows in _ODataWirterCore_
```C#
protected abstract void StartExpandableProperty(ODataExpandableProperty property);
protected abstract void EndExpandableProperty(ODataExpandableProperty property);
```
Then, to implement them in _ODataAtomWriter & ODataJsonLightWriter_. So far, leave the implementation in _ODataAtomWriter_ to throw **NotImplementedException**.

6.	In ODataJsonLightWriter, we can have the following prototype codes:
```C#
protected override void StartExpandableProperty(ODataExpandableProperty property)
{
    …    
}
```
7.	In OdataJsonLightPropertySerializer, add new internal API **WriteExpandableProperty(…)** to write the structural properties of the expandable property. 
8.	Modify the related write scope to make parent of navigation property scope can be expandable property.

Let’s have an example:

Where the model schema can be:
  * Entity type “NS.Order” has a complex property named “Location” with “NS.Address” complex type
	* “NS.Address” has a navigation property named “City” with “NS.City” entity type.

So, we can construct all related object as follows:
```C#
ODataEntry order = new ODataEntry()
{
    TypeName = "NS.Order",
    Properties = new[]
    {
        new ODataProperty {Name = "ID", Value = 1},
        new ODataProperty {Name = "Amount", Value = 20}
    },
};

ODataEntry city = new ODataEntry()
{
    TypeName = "NS.City",
    Properties = new[]
    {
        new ODataProperty {Name = "Name", Value = "Minhang"},
        new ODataProperty {Name = "State", Value = "Shanghai"},
        new ODataProperty {Name = "Country", Value = "CN"}
    }
};

ODataComplexValue location = new ODataComplexValue
{
    Properties = new[]
    {
        new ODataProperty { Name = "Street", Value = "ZiXing Rd" },
        new ODataProperty { Name = "ZipCode", Value = "9001"}
    },
    TypeName = "NS.Address"
};

ODataProperty complex = new ODataProperty
{
    Name = "Location",
    Value = location
};

ODataExpandableProperty expandable = new ODataExpandableProperty
{
    Property = complex
};

ODataNavigationLink navigationLink = new ODataNavigationLink
{
    Name = "City",
    IsCollection = false
};
```
Then, we can write the navigation property in complex type as:
```C#
writer.WriteStart(order);
          writer.WriteStart(expandable);

                writer.WriteStart(navigationLink);

                    writer.WriteStart(city);
                    writer.WriteEnd();

                writer.WriteEnd(); // end of navigation link
          writer.WriteEnd();// end of expandable property
 writer.WriteEnd();
 ```
We can have the following payload:

```json
{
  "@odata.context":"http://.../$metadata#Orders/$entity",
  "ID":1,
  "Amount":20,
  "Location":{
       "Street":"ZiXing Rd",
       "ZipCode":"9001",
       "City":{
             "Name":"Minhang",
             "State":"Shanghai",
             "Country":"CN"
    }
  }
}
```
2.2.2.3	Collection expandable property in entry

We can reuse the ODataExpandableProperty class to serialize the collection expandable property. For example:
```C#
ODataComplexValue address1 = new ODataComplexValue()
ODataComplexValue address2 = new ODataComplexValue()
ODataProperty addresses = new ODataProperty
{
  Name = "Addresses",
  Value = new ODataCollectionValue
  {
    TypeName = "Collection(NS.Address)", 
   Items = new[] { address1, address2 }
  }
};
ODataExpandableProperty expandable = new ODataExpandableProperty
{
  Property = addresses
};
```
Then the service side codes can be as follows:
```C#
ODataEntry entry1 = new ODataEntry();
entry1.AddProperty(property); // normal properties
…
ODataExpandableProperty expandableProperty = new ODataExpandableProperty();// the property with the NP.
ODataEntry entry2 = new ODataEntry(); // Expanded entry belongs to above expandable property
ODataNavigationLink navigationLink =  …
writer.WriteStart(entry1); // write start of entry1 and normal properties
writer.WriteStart(expandableProperty); // write the expandable collection property
    foreach (item in collection expandable property)
    {
            ODataExpandableProperty expandable = new ODataExpandableProperty…
            writer.WrtierStart(expandable);
                 writer.WriteStart(navigationLink);
        	           writer.WriteStart(entry2); 
           writer.WriteEnd();   // End entry2
                     writer.WriteEnd();  // End navigationLink
                writer.WriteEnd();  // End expandable item
         }
    writer.WriterEnd(); // End of collection expandable item
writer.WriteEnd(); // End entry1
```
#### 2.2.2.4	Top level expandable property 

The API _ODataMessageWriter.WriteProperty()_ is used to write property without navigation property. To support navigation property in complex type, we can’t use it, because it cannot be used to write expanded entry and feed. Similar to delta entry writer, we should have the following classes to support top level expandable property with navigation property:
```C#
public abstract class ODataExpandblePropertyWriter
{
  public abstract void WriteStart(ODataExpandableProperty property);
  public abstract void WriteEnd();
  …
}
```
And make the implementation in a new class as follows:
```C#
internal sealed class ODataJsonLightExpandblePropertyWriter : ODataExpandblePropertyWriter, IODataOutputInStreamErrorListener
{
  public override void WriteStart(ODataExpandableProperty property)
  {
    …
  }

  public override void WriteEnd()
  {
    …
  }
}
```

#### 2.2.2.5	Top level collection of expandable property

Similar to _ODataCollectionWriter_, we can provide _ODataCollectionExpandablePropertyWriter_ to writer the top level collection of expandable property.
```C#
public abstract class ODataCollectionExpandablePropertyWriter
{

  public abstract void WriteStart(…);
  public abstract void WriteItem(…);
  public abstract void EndItem(…);
  public abstract void WriteEnd();
  …
}
```

### 2.2.3	Deserialization navigation property in complex type payload

#### 2.2.3.1	Deserialization process

The deserialization, or parse payload, or read payload is a process to covert the payload string into OData object, for example, ODataEntry, ODataProperty, etc. The process uses a state to track the reading. So, there are many read states transferred from one to anther in one deserialization process. 
Let’s have look about the basic entry payload deserialization.

![]({{site.baseurl}}/assets/2015-12-21-ODL-DeSerialize-flow.png)


2.2.3.2	Single expandable property in entry

Simply input, we should add two states, for example:
```C#
public enum ODataReaderState
{
      …
     ExpandablePropertyStart,

     ExpandablePropertyEnd,
     …
}
```
We will only stop and return such state when we reading property with expanded entry in it. So, the server side can have the following structure to catch the state and figure out the expandable property.
```C#
while (reader.Read())
{
     switch (reader.State)
     {
         case ODataReaderState.EntryStart:
              break;

         ……

        case ODataReaderState.ExpandablePropertyStart:
              break;

        case ODataReaderState.ExpandablePropertyEnd:
              break;


        ……
        
        default:
             break;
     }
}
```
Based on this design, we should do as follows:
1.	Add the following APIs in ODataReaderCore to start and end reading the expandable property.
```C#
protected abstract bool ReadAtExpandablePropertyStartImplementation();
protected abstract bool ReadAtExpandablePropertyEndImplementation();
```

2.	Need a new Scope to identify the expandable property reading
```
private sealed class JsonLightExpandablePropertyScope : Scope
{
    …
}
```

#### 2.2.3.3	Collection expandable property in entry
   
For collection, it’s same as single expandable property, except that the embed property should have the collection value.

#### 2.2.3.4	Top level expandable property 

The API ODataMessageReader.ReadProperty() is used to read property without navigation property. To support navigation property in complex type, we can’t use it, because it cannot be used to reader expanded entry and feed. Similar to delta entry reader, we should have the following classes to support top level expandable property with navigation property:
```C#
public abstract class ODataExpandblePropertyReader
{
  public abstract bool Read();  
  …
}
```
And the implementation:
```C#
internal sealed class ODataJsonLightExpandblePropertyReader : ODataExpandblePropertyReader
{

  public override void Read ()
  {
    …
  }
}
```

#### 2.2.3.5	Top level collection of expandable property

Similar to ODataCollectionReader, we can provide ODataCollectionExpandablePropertyReader to writer the top level collection of expandable property.
```C#
public abstract class ODataCollectionExpandablePropertyReader
{

  public abstract void Read(…);
  …
}
```

## 2.3	OData Client
### 2.3.1 Client Support Operation on Complex type
Scenario: Suppose that we have type Location, Address, City. Location, City are Entity type, Address is Complex Type. City is the navigation of Address. 
Then supposedly customers can use following APIs on complex type with NP (navigation property) on client. 
1.	LINQ Expand
```C#
  context.Locations.Expand(a=>a.Address.City); 
  context.Locations.Bykey(1).Address.Expand(a=>a.City);   
  context.Locations.Bykey(1).Addresses.Expand(a=>a.City);
```
 
2.	LoadProperty
```C#
  var location = context.Locations.Where(l=>l.ID == 1).Single();
  var address = location.Address;
  context.LoadProperty(address, "City");
```
3.	AddRelatedObject, UpdateRelatedObject
```C#
  var city = new City();
  context.AddRelatedObject(address, “Cities”, city);
```
4.	AddLink, SetLink, DeleteLink
**NOTE**: 2,3,4 are not applicable to collection value complex type.

### 2.3.2 Materialization
Materialization happens by converting the response payload to client object. 
Client defines different materializers to materialize different kind of payload. As shown in following picture:
*	ODataEntitiesEntityMaterializer: Handle the response of SaveChanges()
*	ODataReaderEntityMaterializer: Handle response of querying Entry or Feed, and not queried through LoadProperty, e.g. GET ~/Customers
*	ODataLoadNavigationPropertyMaterializer: When LoadProperty is called
*	ODataCollectionMaterializer: Handle response of querying collection value property
*	ODataValueMaterializer: Handle response of querying a value, e.g. GET ~/Customers/$count
*	ODataPropertyMaterializer: Handle response of querying a property, e.g. GET ~/Customers(1)/Name
*	ODataLinksMaterializer: Handle response of querying the reference links e.g. GET ~/Customers(1)/Orders(0)/$ref

![]({{site.baseurl}}/assets/2015-12-21-Client-main-flow.png)

The common process of a query is:

![]({{site.baseurl}}/assets/2015-12-21-Client-common-query.png)

The Materialization (Part 2) is driven at the top level by an instance of MaterializeAtom, which implements the enumerable/enumerator. The materializer reads OData object from payload with ODataReader and materialize by calling different materialization policy and tracks materialization activity in an AtomMaterializerLog. Then MaterializeAtom instance applies AtomMaterializerLog onto the context (entityTracker) for each successful call to MoveNext().
During an entry materialization, MaterializerEntry/MaterializerFeed/MaterializerNavigationLink will be created to record the materializer state for a given ODataEntry, ODataFeed and NavigationLink respectively.

#### 2.3.2.1 Materialization class for complex type with navigation property

As complex type with navigation property will be read as an ODataExpandableProperty in ODataReader. To align with this: 
1.	Add ExpandablePropertyMaterializationPolicy to be responsible for materializing an ODataExpandableProperty.
```C#
public class ExpandableComplexPropertyMaterializationPolicy : StructuralValueMaterializationPolicy
{
  private readonly EntryValueMaterializationPolicy entryValueMaterializationPolicy;
  …
}
```
2.	Add MaterializerExpandableProperty to remember the materializer state of a given ODataExpandableProperty.
```C#
internal class MaterializerExpandableProperty
{
  /// <summary>The property.</summary>
  private readonly ODataExpandableProperty ;

  /// <summary>List of navigation links for this entry.</summary>
  private ICollection<ODataNavigationLink> navigationLinks = ODataMaterializer.EmptyLinks;

  …
}
```

#### 2.3.2.2 Materialize complex type property in an entry
When payload is an entry or a feed, ODataReaderEntityMaterializer will be created to materialize the response. So we need add logic in ODataReaderEntityMaterializer to handle the complex type with navigation property.
1.	Add ICollection<ODataExpandableProperty> complexProperties to MaterializerEntry. In Materializer.Read(), add state ODataReaderState.ExpandablePropertyStart/ ODataReaderState.ExpandablePropertyEnd to read complex type and its navigation property to complexProperties. And for each complex type having navigation property, create an instance of MaterializerExpandableProperty. Following is a sample:

```C#
do
                {
                    bool inComplexPropertyScope = false;
                    ODataExpandableProperty complexProperty;
                    ICollection<ODataNavigationLink> complexPropertyNavigationLinks = ODataMaterializer.EmptyLinks;

                    switch (this.reader.State)
                    {
                        case ODataReaderState.NavigationLinkStart:
                            if (!inExpandablePropertyScope)
                            {
                                navigationLinks.Add(this.ReadNavigationLink());
                            }
                            else
                            {
                                complexPropertyNavigationLinks.Add(this.ReadNavigationLink());
                            }
                            break;
                        case ODataReaderState.EntryEnd:
                            break;
                        case ODataReaderState.ExpandablePropertyStart:
                            complexProperty = (ODataExpandableProperty)this.reader.Item;
                            entry.AddComplexProperties(complexProperty);
                            inComplexPropertyScope = true;
                            break;
 
                        case ODataReaderState.ExpandablePropertyEnd:
                            inComplexPropertyScope = false; 
                            MaterializerExpandableProperty.CreateInstance(complexProperty, complexPropertyNavigationLinks);
                            complexPropertyNavigationLinks = ODataMaterializer.EmptyLinks;
                            break;
                        …. 

                    }
            }
```
Then the data flow would be like:

![]({{site.baseurl}}/assets/2015-12-21-Client-data-flow.png)

2.	Materialize
The materializer will call EntryValueMaterializationPolicy to materialize an entity, and in EntryValueMaterializationPolicy, we can call ExpandableComplexPropertyMaterializationPolicy to handle complex type having navigation property. 
```C#
foreach (var property in entry.complexProperties)
{
MaterializerExpandableProperty materializerProperty = property.GetAnnotation< MaterializerExpandableProperty>();             this.expandablePropertyMaterializationPolicy.MaterializeExpandableProperty(property, materializerProperty. navigationLinks);
object value = property.GetMaterializedValue();
    var prop = actualType.GetProperty(property.Name, this.MaterializerContext.IgnoreMissingProperties);
    prop.SetValue(entry.ResolvedObject, value, property.Name, true /* allowAdd? */);   
}
```
3.	ApplyLogToContext
Update the materialization info to DataServiceContext. Will explain more in tracking section. 

#### 2.3.2.3 Materialize top level complex type property
Currently top level single-value complex type is handled by ODataPropertyMaterializer, and collection-value complex type is handled by ODataCollectionMaterializer, and the readers are:

*	ODataPropertyMaterializer  ODataMessageReader.ReadProperty
*	ODataCollectionMaterializer  ODataMessageReader. CreateODataCollectionReader

For complex type has navigation property, we have separate reader for it. 

*	Single-value complex type  ODataExpandblePropertyReader
*	Collection-value complex type  ODataCollectionExpandablePropertyReader

So in ODataPropertyMaterializer/ODataCollectionMaterializer we need add logic to read with _ODataExpandblePropertyReader/ODataCollectionExpandablePropertyReader_ when we found the complex type has navigation property (by visiting the model). 

#### 2.3.2.4 Materialize LoadProperty under complex type property
When LoadProperty is called, ODataLoadNavigationPropertyMaterializer will be used as materializer. So we need add logic for complex type in this materializer:

1.	Get existing complex type instance from descriptor (Refer to the tracking part)
2.	Read the payload to ODataEntry or ODataFeed
3.	Materialize the ODataEntry/ODataFeed and set it as property of the complex type instance

### 2.3.3 Tracking complex type
#### 2.3.3.1 Existing Entity Tracking
In order to directly have operations on a materialized entity, we need store the needed info internally in order to generate the Url for a real http request.  For example, company has been materialized to a clr object, and we try to update a property by directly modifying the clr object.
```C#
company.TotalAssetsPlus = 100;                  
TestClientContext.UpdateObject(company);       
TestClientContext.SaveChanges();
```
In this case, we need try to get the company editlink in order to send PATCH against it. And info like editlink can be achieved during company materialization.  For this reason, client will create an EntityDescriptor when materializing an entity, and store the mapping of the materialized entity and its descriptor in EntityTracker. And the entitytracker can be accessed through DataServiceContext.

EntityDescriptor is defined as:
```C#
public sealed class EntityDescriptor : Descriptor
{
    private Uri identity;          // The id of the entity

        private object entity;    // The materialized value of the entity

        private Uri addToUri;    // uri of the resource set to add the entity to during save

        private Uri selfLink;      // uri to query the entity
 
        private Uri editLink;     // uri to edit the entity.

        private Dictionary<string, LinkInfo> relatedEntityLinks;    // Contains the LinkInfo (navigation and relationship links) for navigation properties
…
}
```
And in EntityTracker we have:

* Dictionary<object, EntityDescriptor> entityDescriptors:
The mapping of entity clr object and entity descriptor. With the entity clr object, we can search the dictionary to get its EntityDescriptor, then we can get the editlink/selflink of the entity, and send quest against it.

* Dictionary<Uri, EntityDescriptor> identityToDescriptor
The mapping of entity id and entity descriptor. When materializing an entity, we will firstly search this dictionary to check if the entity is already been tracked (materialized). If it is already been materialized, it will reuse existing clr object (EntityDescriptor-> entity), and apply new values to it. 

* Dictionary<LinkDescriptor, LinkDescriptor> bindings
The binding of entity and its navigation property which has been tracked (materialized). For example, if we request Get Customers(1)?$expand=Order, then during materialization, we will create a LinkDescriptor(customer(id=1), “Order”, order, this.model) to track the binding, customer and order is the materialized object. This dictionary can be used for AddLink, SetLink…

So when we try to query an entity, client will work as:

![]({{site.baseurl}}/assets/2015-12-21-Client-query-entity.png)

### 2.3.3.2 Complex type tracking
Like entity, in order to support LoadProperty, AddRelatedObject, AddLink… on complex type, we need track as well for those complex type having navigation property. But as we do not have an identity for complex type, so we can only track single-value complex type, and the complex type property must be queried inside an entry (Will explain why we has this restriction later). 

1.	Create ComplexTypeDescriptor
In order to reuse current logic of EntityDescriptor, we can add a base class ResourceDescriptor let EntityDescriptor and ComplexTypeDescriptor inherit from it. 

![]({{site.baseurl}}/assets/2015-12-21-Client-class-relation1.png)

2.	Then for EntiyTracker, it will be like:

![]({{site.baseurl}}/assets/2015-12-21-Client-class-relation2.png)

3.	Then track when complex type property is queried through entry
**Materializer.Read()**:
a.	Create ComplexTypeDescriptor while encountering complex type property which is needed to be tracked, complex type identity consists of entity id plus complex property name, for example, ~/Locations(1)/Address.
b.	Update relatedLinks in ComplexTypeDescriptor by reading navigation links in complex type property (Need ODL support: ODL reader should be able to compute the navigation links for navigation property under single-value complex type).
c.	Add ComplexTypeDescriptor to EntityDescriptor
d.	If the navigation link of complex type is expanded in the payload, read the expanded entry or feed, and annotate the navigationlink with MaterializerNavigationLink, same with reading the navigation property of entity.

**Materializer.Materialize:**
Materialize OData value to client clr object.  
a.	Update materialized value of complex type property to complexTypeDescritors in EntityDescriptor 
b.	Create a LinkDescriptor of the materialized complex value and its materialized navigation property value and add it to MaterializerLog. 

**Materializer.ApplyLogToContext():**
Update entity tracker based on previous materialization. 
a.	Add or merge EntityDescriptor to EntityTracker, including updating complexTypeDescriptors in EntityDescriptor
b.	Add or merge each complexTypeDescriptors in EntityDescriptor to Dictionary of complexDescriptors and identityToDescriptors in entityTracker.
c.	Update complex type navigation links in MaterializerLog to bindings of entityTracker.

4.	We cannot track collection-value complex type property, so following scenario does not work:
```C#
var location = TestClientContext.Locations.Where(lo=>lo.ID == 1).Single();
var addresses = location.Addresses;    //Addresses is collection-value complex type property
foreach (var addr in addresses) 
{
  TestClientContext.LoadProperty(addr, "City");  // Does not work
}
```
Reason:
We do not have an identity for a complex type instance in a collection, and there is no way for us to know if the incoming complex type has been materialized before. We probably can support this tracking if we allow indexing into collections.

Workaround: 
Expand city instead, for example, `context.Locations.Bykey(1).Addresses.Expand(a=>a.City)`;

5.	We cannot track if complex type property is queried as individual property
For example, in following scenario, we are not able to track complex type and use it in LoadProperty: 
```C#
var addr = TestClientContext.CreateQuery<Address>("Company/Address").Execute();
foreach (var d in addr)          // Will materialize Address here. 
{
  TestClientContext.LoadProperty(d, "City");   // Does not work
}
```
Reason:
When querying complex type property directly, we do not have the entity info. We only have the request Uri and the request Uri cannot be used to identify a complex type. For example, Get ~/Locations(1)/Address and Get ~/Company/Location/Address may actually get the same instance.

Workaround:
Query the entity which includes the complex type. 

### 2.3.4 Public API Implementation related to tracking
1. LoadProperty
For example, when customer call context.LoadProperty(address, "City"), client need add logic:
*	Remove the validation about address must be an entityType
*	Search dictionary of ResourceDescriptors with object address to get the ComplexTypeDescriptor
*	Get the navigationlink of City from ComplexTypeDescriptor-> relatedEntityLinks
*	Generate request with the navigationlink  of City

2. AddRelatedObject, UpdateRelatedObject
*	Remove the validation about source 
*	Update the parent Descriptor of EntityDescriptor to ResourceDescriptor so it can accept ComplexTypeDescriptor
*	For AddRelatedObject, add the LinkDescriptor to the bindings in entityTracker

3. AddLink, SetLink, DeleteLink
  Similar to Entity, add/set/delete bindings of entitytracker, and get source/target descriptor to generate request.
        
### 2.3.5 Serialization
When we try to add or update an entity from client, we need call ODataWriter to serialize the object to payload. So in class Serializer:
1.	WriteEntry
If we do not do any operation to the navigation property under complex type, then this function does not need any change. Meanwhile, if we want to support adding bindings to complex type (only for single-value complex type), like the following scenario:

```C#
var city = context.Cities.Bykey(1).GetValue();
Address address = new Address {Street = "Zixing"};
Location location = new Location {ID=11, Address=address};
context.AddToLocations(location);
context.SetLink(address, "City", city);
context.SaveChanges();
```
	Then the process would be like:
a)	AddToLocations would create an EntityDescriptor for location and add it to entityTracker.ResourceDescriptor
b)	SetLink would create a LinkDescriptor between address and city and add it to entityTracker.bindings
c)	In BaseSaveResult->RelatedLinks (EntityDescriptor entityDescriptor) which is try to enumerate the related Modified/Unchanged links for an added item, try to match link.source to the entity or property value in the descriptor. If the link.source is equal to a property value, create a ComplexTypeDescriptor and add it to entityDescriptor and entityTracker. 
d)	When creating OData objects, go through ComplexTypeDescriptor under EntityDescriptor and create ODataExpandableProperty from them
e)	Write ODataEntry, and for ODataExpandableProperty, call WriteStart(ODataExpandableProperty property), and call WriteEntityReferenceLink to write the binding.

2.	WriteEntityReferenceLink
When AddLink/SetLink is called on complex type, this function will be called to write the payload. So the function need update the logic from EntityDescriptor to ResourceDescriptor. 

### 2.3.6 CodeGen
In order to support .Expand on complex type, we need generate DataServiceQuery for Complex Type.
For scenario Locations->Address->City, Address is complex type, City is the navigation property of Address:
 
1.	Generate Class for Address: 
 
**If City is single-value navigation property:**
```C#
public partial class Address : global::System.ComponentModel.INotifyPropertyChanged
{
    public global::Microsoft.OData.Client.City City  { get; set; }
}
public partial class AddressSingle : global::Microsoft.OData.Client.DataServiceQuerySingle<Address>
{
    public global::Microsoft.OData.Client.CitySingle City { get; }
}
```

**If City is collection-value navigation property:**
```C#
public partial class Address : global::System.ComponentModel.INotifyPropertyChanged
{
    public global::Microsoft.OData.Client.DataServiceCollection<global::ODataClientSample.City> Cities  { get; set; }
}
public partial class AddressSingle : global::Microsoft.OData.Client.DataServiceQuerySingle<Address>
{
    public global::Microsoft.OData.Client.DataServiceQuery<global::ODataClientSample.City> Cities { get; }
}
```
2.	Add Address to Location:
**If complex type Address is single-value:**
```C#
public partial class Location: global::Microsoft.OData.Client.BaseEntityType, global::System.ComponentModel.INotifyPropertyChanged
{
public global::ODataClientSample.Address Address {get;set;}
}
```
Then we can support:
```C#
var location = context.Locations.Where(l=>l.ID == 1).Single();
var address = location.Address;
```

**If complex type Address is collection-value:**
```C#
public partial class Location: global::Microsoft.OData.Client.BaseEntityType, global::System.ComponentModel.INotifyPropertyChanged
{
    public global::System.Collections.ObjectModel.ObservableCollection<Address> Addresses {get;set;}
}
```
Then we can support:
```C#
var addresses = location.Addresses;
```

3.	Add Address to LocationSingle:
**If complex type Address is single-value:**
```C#
public partial class LocationSingle: global::Microsoft.OData.Client.DataServiceQuerySingle<Location>
{
  public global::ODataClientSample.AddressSingle Address {get;}
}
```
Then we can support:
```C#
var location = context.Location.ByKey(1).Address.Expand(a=>a.City);
```
 
**If complex type Address is collection-value:**
```C#
public partial class LocationSingle: global::Microsoft.OData.Client.DataServiceQuerySingle<Location>
{
public global::Microsoft.OData.Client.DataServiceQuery<global::ODataClientSample.Address> Addresses {get;}
}
```
Then we can support:
```C#
  var location = context.Location.ByKey(1).Addresses.Expand(a=>a.City);
```

## 2.4 Web API OData 

### 2.4.1 Model builder

Similar with the entity type and complex type structure, Web API OData has the same configuration class structure. Below picture shows the relationship between complex type configuration, entity type configuration and structural type configuration.

![]({{site.baseurl}}/assets/2015-12-21-WebApi-class-relation1.png)


Owing that _NavigationPropertyConfiguration_ is derived from _PropertyConfiguration_, and all properties for type (entity type or complex), either structural properties, or navigation properties are saved in the following dictionary in _StrucutralTypeCofiguration_:
```C#
protected internal IDictionary<PropertyInfo, PropertyConfiguration> ExplicitProperties { get; private set; }
```
So, form this point, complex type configuration can support navigation property. However, we need to do as follows to allow the customer to define navigation property on complex type:

1. Promote the following public/private APIs from _EntityTypeConfiguration_ to _StructuredTypeConfiguration_.
```C#
public virtual NavigationPropertyConfiguration AddNavigationProperty(PropertyInfo navigationProperty, EdmMultiplicity multiplicity)
public virtual NavigationPropertyConfiguration AddContainedNavigationProperty(PropertyInfo navigationProperty, EdmMultiplicity multiplicity)
private NavigationPropertyConfiguration AddNavigationProperty(PropertyInfo navigationProperty, EdmMultiplicity multiplicity, bool containsTarget)
```
2. Promote the following property from _EntityTypeConfiguration_ to _StructuredTypeConfiguration_
```C#
public virtual IEnumerable<NavigationPropertyConfiguration> NavigationProperties
```
3. Modify _NaviatonPropertyConfiguration_ class, for example
  * modify DeclaringEntityType property
  * add DeclaringComplexType property
  * modify the constructor
  * …

4. Modify _EdmTypeBuilder_ class to construct the complex type with navigation property.
```C#
         Change
private void CreateNavigationProperty(EntityTypeConfiguration config)
        To
private void CreateNavigationProperty(StructuralTypeConfiguration config)
```
5. Promote the following APIs from _EntityTypeConfigurationOfTEntityType_ to _StructuralTypeConfigurationOfTStrucuturalType_.
```C#
  * HasMany
  * HasRequired
  * HasOptional
```
Let’s have an example to illustrate how configure the navigation property in complex type:
a) We have the three types, **Customer** and **Region** as entity type, **Address** as complex type
```C#
public class Customer
{
   public int CustomerId { get; set; }
   public Address Location { get; set; }
}

public class Address
{
   public string Street { get; set; }
   public Region Region { get; set; }
}

public class Region
{
   public int RegionId { get; set; }
   public string Name { get; set; }
}
```
Then, we can configure the Edm type by non-convention model builder as:
```C#
var builder = new ODataModelBuilder();
builder.EntityType<Customer>().HasKey(c => c.CustomerId).ComplexProperty(c => c.Location);
builder.EntityType<Region>().HasKey(r => r.RegionId).Property(r => r.Name);
var address = builder.ComplexType<Address>();
address.Property(a => a.Street);
address.HasRequired(a => a.Region);
```

### 2.4.2	Convention model builder and conventions

In convention model builder, it is assumed that complex type can’t have navigation property. As a result, the properties type belong to complex type is built as complex type if it’s not enum type or primitive type. 
As navigation property is allowed in complex type, we should change the flow to assume the structural type of property in complex type as entity type. Once all types are buil, we should re-use the re-discover logic to change the assumed type.
So, we should do:

1. Modify MapComplexType(…) function, for any implicated added structural types from complex type, mark it as entity type and add them as navigation properties.

2. Re-configure the properties in complex type if the related entity types are re-configured as complex type.

For user codes, it should be same as previous, for example we re-use the CLR classes mentioned in previous section:
```C#
var builder = new ODataConventionModelBuilder();
builder.EntityType<Customer>();
builder.ComplexType<Address>();
```
Use the above codes, the **Region** type should be built as entity type automatically.

### 2.4.3 Navigation Source binding for navigation property in complex type

We should modify some codes in _NavigationSourceConfigurationOfEntityType_ to make navigation source binding to the navigation property in complex type:
For example:
```C#
builder.EntitySet<Customer>("Customers").HasRequiredBinding(c => c.Location.Region, "Regions");
```
Make sure the property path can be saved correctly.

### 2.4.4 Navigation property routing

* Path segment
The navigation path segment class _NavigationPathSegment_ can be re-used for navigation property in complex type. However, the **GetNavigationSource()** in _PropertyAccessPathSegment_ can’t return null directly. It maybe return the previous navigation source if the property is complex type.

* Routing convention

We can provide the one level routing convention for navigation property, but leave other for attribute routing. We can add the following path template in _NavigationRoutingConventions_:

  - ~/entityset/key/property/navigation

  - ~/entityset/key/property/cast/navigation

  - ~/singleton/property/navigation/$count

  - ~/singleton/property/cast/navigation/$count

The convention action name can be:   

**“RequestMethodName” + “NavigationPropertyName” + “In” + “ComplexPropertyName” + “From” + “DeclareTypeName”**

For example, GET ~/Customers(1)/Location/Region

The action name can be _“GetRegionInLocationFromCustomer”_

* Query option

Change the SelectExpandBinder to support expanding the navigation property in complex type. For example:

  - ~/…/Property?$expand=navigation
  - ~…?&expand=property/navigation
  
### 2.4.5 Serialization navigation property in complex type

#### 2.4.5.1 Expand complex property in Entry

1. SelectExpandNode
In SelectExpandNode, we have the following sets:

* SelectedStructuralProperties
* SelectedNavigationProperties
* ExpandedNavigationProperties

These sets are enough to expand a navigation property in entity, but it’s non-enough to expand a navigation property in complex type. Because, we should know which complex property is expanded. That’s, if we have the following request:
```C#
GET ~/Customers(1)?$expand=Location/Region
```
We should know **“Location”** is an expandable property and it’s expanded with **“Region”**.
So, for _SelectExpandNode_, we should at least a set to save the expanded structural property. Let’s say it can be:
```C#
public ISet<Tuple<IList<IEdmStructuralProperty>, IEdmNavigationProperty, SelectExpandClause>> ExpandedStructuralProperties { get; private set; }
```
And we should construct this property in constructor of _SelectExpandNode_ class.

2. Entit type serializer

For entity type serializer, we can use the **ExpandedStructuralProperties** defined in _SelectExpandNode_ to construct the expandable property.
So, we should do:
a) In CreateEntry function, before we call **CreateStructuralPropertyBag()** function, we should remove the expanded structure properties from **SelectedStructuralProperties**, and use the except set to build the properties for entry.
```C#
var expandableProperties = selectExpandNode.ExpandedStructuralProperties.Select(e => e.Item1.First());
var selectedStructuralProperties = selectExpandNode.SelectedStructuralProperties.Except(expandableProperties);
ODataEntry entry = new ODataEntry
{
   TypeName = typeName,
   Properties = CreateStructuralPropertyBag(selectedStructuralProperties, entityInstanceContext),
 };
 ```
   Then, the expanded structural properties exclude from the properties.

b) Provide a new private API to write the expanded structural properties:

```C#
private void WriteExpandedStructuralProperties(
            ISet<Tuple<IList<IEdmStructuralProperty>, IEdmNavigationProperty, SelectExpandClause>>
                structuralPropetiesToExpand,
            EntityInstanceContext entityInstanceContext,
            ODataWriter writer)
{
      Foreach( expanded structureal property)
      {
            Construct an expandable property (new ODataExpandableProperty())
            WriteStart(expandable property)
            WriteExpandedNavigationProperties(..)
            WriteEnd()
     }
}
```
c) Call WriteExpandedStructuralProperties after WriteStart(entry)
```C#
ODataEntry entry = CreateEntry(selectExpandNode, entityInstanceContext);
if (entry != null)
{
     writer.WriteStart(entry);
     WriteExpandedStructuralProperties(selectExpandNode.ExpandedStructuralProperties, entityInstanceContext, writer);
      WriteNavigationLinks(selectExpandNode.SelectedNavigationProperties, entityInstanceContext, writer);
      WriteExpandedNavigationProperties(selectExpandNode.ExpandedNavigationProperties, entityInstanceContext, writer);
      writer.WriteEnd();
 }

```


### 2.4.5.2 Top level expanded complex property

We can modify ODataComplexTypeSerializer to support expanded complex property. For example:
```C#
GET ~/Customers(1)/Location?$expand=Region
```
So, we can do as follows:
1. Modify _EntityInstanceContext_ to accept complex type instance, or create a new class named _ComplexInstanceContext_.

2. Use the _SelectExpandNode_ into _ODataComplexTypeSerializer_

3. If **ExpandedStructuralProperties** is empty, serialize the complex as normal, otherwise we will serialize an expandable property.

4. Add new function named **CreateExpandableProperty**
```C#
private static ODataExpandableProperty CreateExpandableProperty(EntityInstanceContext context)
{
        ….            
}
```
5. Create a new function to write the expandable property
```C#
private static void WriteExpandableProperty(ODataExpandableProperty property, EntityInstanceContext context, ODataSerializerContext writeContext);
{
        Create ODataExpandablePropertyWriter;
        Call WriteStart(…);
        WriteExpandedNavigationProperties(…);
        WriteEnd();
                  
 }
 ```
  Where, **WriteExpandedNavigationProperties** maybe same as the function in _ODataEntityTypeSerializer_.
For top level collection of expandable property, we can use the above same logic but create an _ODataCollectionExpandablePropertyWriter_ to write.


### 2.4.6 Deserialization navigation property in complex type

### 2.4.6.1 Expand complex property in Entry

As mentioned in OData Core for reader, we have two new reader state:
```C#
public enum ODataReaderState
{
      …
     ExpandablePropertyStart,

     ExpandablePropertyEnd,
     …
}
```
So, we can use them to read the expandable property. 
1. Create a new class named _ODataExpandablePropertyWithNavigationLinks_
```C#
public sealed class ODataExpandablePropertyWithNavigationLinks : ODataItemBase
{
       ……
        public ODataExpandableProperty Property
        {
            get { return Item as ODataExpandableProperty; }
        }

        public IList<ODataNavigationLinkWithItems> NavigationLinks { get; private set; }
 }
 ```
2. Modify **ReadEntryOrFeed()** function in _ODataEntityDeserializer_ by added two case statements into:
```C#
while (reader.Read())
{
       switch (reader.State)
        {
              ….
              case ODataReaderState.ExpandablePropertyStart:
                        ....
                       break;

               case ODataReaderState.ExpandablePropertyEnd:
                       break;
  
         }
}
```
   In **ExpandablePropertyStart**, we can create _ODataExpandablePropertyWithNavigationLinks_ object to push it into Stack. Make sure, we should modify the state transfer to make sure the **NavigationState** can follow up _ExpandablePropertyStart_.
3. Should modify ODataEntryWithNavigationLinks to accept the ODataExpandablePropertyWithNavigationLinks

4. Add new API named AddExpandedStrucutralProperties()
```C#
private void AddExpandedStrucutralProperties(IEdmNavigationProperty navigationProperty, object entityResource,
            ODataEntryWithNavigationLinks entry, ODataDeserializerContext readContext)
{
  …
}
```
 And call it brefore **ApplyNavigationProperties** in **ApplyEntityProperties()**.

#### 2.4.6.2 Top level expanded complex property

Web API doesn’t support to read a top level complex property. So, we shouldn’t support expanded complex property.

#### 2.4.6.3 Expanded complex property as action parameter

TBD.
