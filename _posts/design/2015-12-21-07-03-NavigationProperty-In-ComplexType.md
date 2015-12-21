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
