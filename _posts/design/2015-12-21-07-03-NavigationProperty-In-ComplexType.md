---
layout: post
title: "7.3 Navigation Property in Complex Type Design"
description: "Design doc for navigation property, complex type"
category: "7. Design"
---

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



