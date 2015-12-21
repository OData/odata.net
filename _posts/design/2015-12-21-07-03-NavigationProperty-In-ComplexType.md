---
layout: post
title: "7.3 Navigation Property in Complex Type Design"
description: "Design doc for navigation property, complex type"
category: "7. Design"
---
<<Inital draft, Improvment frequently>>

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
