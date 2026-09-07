---
layout: post
title: "Omit null values"
description: "Omit null values"
category: "5. OData Features"
---

# Introduction
Per [OData V4.01 spec](http://docs.oasis-open.org/odata/odata/v4.01/cs01/part1-protocol/odata-v4.01-cs01-part1-protocol.html#_Toc505771137), client request can specify null values to be omitted in the response payload by specifying Prefer header value "omit-values=nulls" in the comma-separated list of preferences, using the following syntax:
```
Prefer: omit-values=nulls[,other preference values]
```
For sparse structural objects in the response, this helps speeding up the serialization and significantly reducing the payload size on the wire and also the network latency.

When the response is received by the client, the client can use OData library for payload deserialization and restoration of null values omitted in wire to get the desired EDM object.

# Overview
When OData service serves the request with omit-values=nulls preference, ODataJsonLightWriter generates response by skipping serialization of null-value items for primitive properties, nested complex type properties and expanded entities. 

In the response, Preference-Applied header is automatically set by the ODataJsonLightWriter, corresponding to the Prefer header in the request. For example:
```
Preference-Applied: omit-values=nulls[,other preference values]
```

When the response message arrives at the client, in general payload is deserialized by ODataJsonLightReader based on metadata acquired through context url in the response and the model equipped in the reader. When the Preference-Applied header indicates null values were omitted in the response, these null value properties omitted during serialization are restored as part of deserialization process.

Note that navigation properties (links) should be irrelevant to omitting nulls in the serialization / deserialization processing. This is because navigation properties (not-expanded) are emitted in form of navigationlink annotations when OData metadata level is full. If OData metadata level is minimal, the response message omits the navigation links; and during client side response deserialization ODataJsonLightReader always materializes missing nested resource information containing the navigation links, based on selected properties information provided by context url of the response. 

# Sample
### Edm model

The Edm model has an entity set 'employees', whose elements are of entity type 'Employee', which is an open type containing some primitive properties, structural property 'Address' and two navigation properties 'Manager' and 'Friend', both of 'Employeee' type.
```
            model = new EdmModel();
            var addressType = model.AddComplexType("test", "Address");
            addressType.AddStructuralProperty("city", EdmPrimitiveTypeKind.String, true);
            employeeType = model.AddEntityType("test", "Employee", null, false, true);   // Open type
            var key = employeeType.AddStructuralProperty("Upn", EdmPrimitiveTypeKind.String, false);
            employeeType.AddKeys(key);
            employeeType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String, false);
            employeeType.AddStructuralProperty("Title", EdmPrimitiveTypeKind.String, true);
            employeeType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, true));

            employeeType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "Manager",
                    TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                    Target = employeeType
                }
            );
            employeeType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "Friend",
                    TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                    Target = employeeType
                }
            );

            var container = model.AddEntityContainer("test", "service");
            employeeSet = container.AddEntitySet("employees", employeeType);
```

### Entity objects
In our sample data, entity set "employees" contains employees "Fred" and "FriendOfFred", with the following properties:
```
    {
        Upn: "Fred",
        Name: "fd",
        Title: null,
        Address: null,
        Manager: null,
        Friend: {
            Upn: "FriendOfFred",
            Name: null,
            Title: null,
            Address: null,
            Manager: null,
            Friend: null,
        }
    }
```
and with the following annotations added by the service:
```
    {
        Title@test.annotation: "annotationValue",
        DynamicAnnotated@test.annotation: "annotationValue",
    }
```
    
### Request
Client issues a request to the service using following sample url and Prefer header:
```
GET https://test/employees("Fred")?$select=Upn,Title,Dynamic,Address&$expand=Manager,Friend($select=Upn,Title)
Content-Type: application/json;odata.metadata=minimal
Prefer: omit-values=nulls,odata.include-annotations="*"
```
In the request message above, note the followings:
- Indicating omitting null values in the response;
- Selecting declared property "Upn", "Title", Address, and dynamic property "Dynamic";
- Expanding navigation property Manager and Friend;

Client request Prefer header can be set as follows:
```
	requestMessage.PreferHeader().OmitValues = "nulls";
```

### Response Serialization
Using ODataJsonLightWriter, service generates response message, as described in the following snippet:
```
    Preference-Applied: omit-values=nulls,odata.include-annotations="*"
    {
        "@odata.context":"https://test/$metadata#employees(Upn,Title,Dynamic,Address,Manager(),Friend(Upn,Title))/$entity",
        "@odata.type": "#test.Employee",
        "@odata.id": "https://test/Employees('Fred')",
        "@odata.editLink": "https://test/Employees('Fred')",
        "Upn":"Fred",
        "Title@test.annotation":"annotationValue",
        "DynamicAnnotated@test.annotation":"annotationValue",
        "Friend": {
            "Upn":"FriendOfFred"
        }
    }
```
In the above response message:
- Preference-Applied header indicates null values are omitted in the payload.
- Context url in the response is constructed per the incoming request's select & expand clause, with expanded entity with applicable sub-selected clauses.
- Dynamic property "Dynamic" is allowed in open type entity, but its null value is omitted on the wire.
- Structural property "Address"'s null value is omitted on the wire.
- Annotation values are added to the payload.
- Expanded entity "Manager"'s null value is omitted on the wire.
- Expanded entity "Friend" only has its non-null property "Upn" on the wire, the rest null-value properties are omitted. See OData Spec V4.01 for [projected expanded entity in context url](http://docs.oasis-open.org/odata/odata/v4.01/cs01/part1-protocol/odata-v4.01-cs01-part1-protocol.html#sec_ExpandedEntity).
- Other non-null properties but not selected in the query clause of request are not included in the payload (as expected).

### Response Deserialization
Lastly, when client receives the response message above, it uses ODataJsonLightReader to deserialize the payload. Here is what the deserialized EDM object looks like:
```
    {
        Upn: "Fred",
        Title: null,     
        Dynamic: null,
        Address: null,
        Manager: null,
        Friend: {
            Upn: "FriendOfFred",
            Title: null,
        }
    }
with annotation values:
    "Title@test.annotation":"annotationValue",
    "DynamicAnnotated@test.annotation":"annotationValue",   
```
It also, according to the context url, materializes the nested resources information (navigation links) of employee 'Fred' as well:
- Expanded entities 'Manager'
- Expanded entities 'Friend'

which can be further utilized by the client.
