---
layout: post
title: "6.2 Design doc for $expand in context-url for OData V4.01"
description: "Design doc for $expand in context-url for OData V4.01"
category: "6. Design"
---

# $expand in Context Url

## Introduction
In form of metadata, context url in response from OData-compliant service provides a way to describe the response payload, and is used as control information to facilitate response processing by the client. 

Here is an example of a simple query request and resulting response containing context url:

Request url: http://services.odata.org/V4/TripPinService/People('russellwhyte')?$select=FirstName,LastName

Response:
```
{
    "@odata.context": "http://services.odata.org/V4/TripPinService/$metadata#People(FirstName,LastName)/$entity",
    "@odata.id": "http://services.odata.org/V4/TripPinService/People('russellwhyte')",
    "@odata.etag": "W/\"08D62407F53DE9ED\"",
    "@odata.editLink": "http://services.odata.org/V4/TripPinService/People('russellwhyte')",
    "FirstName": "Russell",
    "LastName": "Whyte"
}
```
As shown above, @odata.context annotation specifies the context url, and provides the following machine-readable description for response data:
- It is regarding an entity from the "People" entity set,
- It contains two property values "FirstName", "LastName".

## Expanded Entity Specification for OData V4.01
OData V4.0 specifies that expanded entities with nested selects are included in the context Url as the name of the navigation property suffixed with the comma separated list of selected properties, enclosed in parens. OData V4.01 [format](http://docs.oasis-open.org/odata/odata/v4.01/cs01/part1-protocol/odata-v4.01-cs01-part1-protocol.html#sec_ExpandedEntity) specifies that, in the absence of any nested selects, the expanded navigation property appears suffixed with empty parens. This is distinct, and may appear in addition to, the un-suffixed navigation property name, which indicates that the navigation property appears in the $select list (indicating that the navigationLink should be included in the response).
```
10.10 Expanded Entity
Context URL template:

{context-url}#{entity-set}{/type-name}{select-list}/$entity

{context-url}#{singleton}{select-list}

{context-url}#{type-name}{select-list}

For a 4.01 response, if a navigation property is explicitly expanded, then in addition to the non-suffixed names of any selected properties, navigation properties, functions or actions, the comma-separated list of properties MUST include the name of the expanded property, suffixed with the parenthesized comma-separated list of any properties of the expanded navigation property that are selected or expanded. If the expanded navigation property does not contain a nested $select or $expand, then the expanded property is suffixed with empty parentheses. [If the expansion is recursive for nested children, a plus sign (+) is infixed between the navigation property name and the opening parenthesis.]

For a 4.0 response, the expanded navigation property suffixed with parentheses MAY be omitted from the select-list if it does not contain a nested $select or $expand, but MUST still be present, without a suffix, if it is explicitly selected.
```

## Change Summary
According to the OData spec above, this change is for creating proper expand token in response's context url, corresponding to $expand clause in the request url. Context Url is used as response metadata to control data materialization when response is received by the client.
We need to make sure that:
- Context url in the response generated contains correct token for the $expand clause.
- Client can parse the context url correctly and uses the metadata for response processing.

### V4.01

As required, we are going to add to the context url the parenthesized (either empty or non-empty) navigation property when it is expanded. For example:

Request Url | Context Url in Response
----------- | -----------------------
 root/Cities('id')?$expand=TestModel.CapitalCity/Districts                       | root/$metadata#Cities(TestModel.CapitalCity/Districts())/$entity
 root/Cities('id')?$expand=TestModel.CapitalCity/Districts($select=DistrictName) | root/$metadata#Cities(TestModel.CapitalCity/Districts(DistrictName))/$entity
 root/Cities('id')?$select=Name&$expand=TestModel.CapitalCity/Districts($select=DistrictName) | root/$metadata#Cities(Name,TestModel.CapitalCity/Districts(DistrictName))/$entity
 root/Cities('id')?$select=Name,TestModel.CapitalCity/Districts&$expand=TestModel.CapitalCity/Districts($select=DistrictName) | root/$metadata#Cities(Name,TestModel.CapitalCity/Districts,TestModel.CapitalCity/Districts(DistrictName))/$entity


We also fix the known issue https://github.com/OData/odata.net/issues/1104 when a navigation link is both expanded and selected, the parenthesized navigation property will be present in the context-url, along with the navigation link explicitly selected:

Request Url | Context Url in Response
----------- | -----------------------
 root/Cities('id')?$select=**TestModel.CapitalCity/Districts**&$expand=**TestModel.CapitalCity/Districts** | root/$metadata#Cities(**TestModel.CapitalCity/Districts,TestModel.CapitalCity/Districts()**)/$entity
 root/Cities('id')?$select=Id,Name,**ExpandedNavProp**&$expand=**ExpandedNavProp**                         | root/$metadata#Cities(Id,Name,**ExpandedNavProp,ExpandedNavProp()**)/$entity

### V4.0
We are going to tweak the behavior to align with that of the 4.01. While this change does introduce slightly different semantics for explicitly selected navigation property, it is not considered a breaking change because the updated context-url form for expanded navigation link is legal, but just was not required. It has been confirmed through code examination and ODL tests that this change doesn't cause anomalies for libraries and client.

### Context Url Parsing Updates to Align with OData V4/V4.01 ABNF Spec
Notion of expanded entity in context url has been introduced since [OData V4 ABNF](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/abnf/odata-abnf-construction-rules.txt). Here is the excerpt of context url select clause in OData V4.01 ABNF:

```
;------------------------------------------------------------------------------
; 3. Context URL Fragments
;------------------------------------------------------------------------------

context         = "#" contextFragment
contextFragment = 'Collection($ref)'
                / '$ref'
                / 'Collection(Edm.EntityType)'
                / 'Collection(Edm.ComplexType)'
                / singletonEntity [ navigation *( containmentNavigation ) [ "/" qualifiedEntityTypeName ] ] [ selectList ]
                / qualifiedTypeName [ selectList ]
                / entitySet ( '/$deletedEntity' / '/$link' / '/$deletedLink' )
                / entitySet keyPredicate "/" contextPropertyPath [ selectList ]
                / entitySet [ selectList ] [ '/$entity' / '/$delta' ]
                
entitySet = entitySetName *( containmentNavigation ) [ "/" qualifiedEntityTypeName ]
            
containmentNavigation = keyPredicate [ "/" qualifiedEntityTypeName ] navigation
navigation            = *( "/" complexProperty [ "/" qualifiedComplexTypeName ] ) "/" navigationProperty   

selectList         = OPEN selectListItem *( COMMA selectListItem ) CLOSE
selectListItem     = STAR ; all structural properties
                   / allOperationsInSchema 
                   / [ qualifiedEntityTypeName "/" ] 
                     ( qualifiedActionName
                     / qualifiedFunctionName 
                     / selectListProperty
                     )
selectListProperty = primitiveProperty  
                   / primitiveColProperty 
                   / navigationProperty [ "+" ] [ selectList ]
                   / selectPath [ "/" selectListProperty ]

contextPropertyPath = primitiveProperty
                    / primitiveColProperty
                    / complexColProperty
                    / complexProperty [ [ "/" qualifiedComplexTypeName ] "/" contextPropertyPath ]
```

The expanded entity introduced nested list of comma-separated items for the projected expanded entity. While expanded entity is supported at high level per OData V4 Spec, some common cases such as projecting multiple properties in expanded entities, e.g. $expand=Account($select=UPN,DisplayName), are not supported by context Url parsing implementation, which currently is aligned with only ABNF V3.

This requires updating the context URL selected clause parsing implementation to align with ABNF V4.01. [Issue #1268](https://github.com/OData/odata.net/issues/1268) is added to the scope of this task.


## Design
1. In OData 6.x & 7.x implementation, while creation of the context url select clause is aligned with OData V4/V4.01 ABNF spec, its counterpart of lexical parsing is still using OData ABNF V3, which specifies context url select list as a flat list of comma-separated items. To align with OData ABNF V4.01, parser needs to adopt a recursive approach. In addition to the path segments representing simple properties originally, a notion of expanded entity needs to be introduced in SelectedPropertiesNode to accommodate requirements from OData ABNF V4.

2. During response de-serialization, the emitted expanded navigation property token (in the form of parenthesized navigation link) could cause incompatibility/ambiguity to the semantics of context-url's selected-list, where an empty list stands for select-all(entire subtree). This is actually an issue in current library which always emits "Projected Expanded Entity" into the selected list. It could cause inadvertent omission of navigation link information during materialization (see issue #1259) in current library, and needs to be addressed as a prerequisite of this task.
    . Check whether the select clause during SelectPropretiesNode instantiation only contains expanded navigation link, which contains balanced parentheses. The item should also be resolvable to navigation property, not other types of property.
    . Additional EDM type information needs to be passed in for navigation property resolution in the constructor of the SelectedPropertiesNode. The EDM type information is originated from the OData reader, which instantiates the SelectedPropertiesNode.

3. For both V4 and V4.01, always emit the parenthesized navigation link during response writing for context-url.

4. For internal logic of combining select list and expand list together, we should, according to select list's semantics, create a node of entire subtree when the selected list is empty.

5. Cleanup the callback function processSubResult signatures by removing ODataVersion argument and associated method, since same logic is used for both V4 and V4.01. The callback function is used for traversing the select&expand clause recursively and providing aggregated result.
```
private static string ProcessSubExpand(string expandNode, string subExpand, ODataVersion version)
```

```
private static SelectedPropertiesNode ProcessSubExpand(string nodeName, SelectedPropertiesNode subExpandNode, ODataVersion version)
```

## Work Items Breakdown:

 Task | Estimate
------| --------
    Research on OData spec for v4.01 related to context-url and projected expand entity	| Completed
    Research on OData implementation related to context-url serialization / de-serialization / metadata level | Completed
    Address prerequisite issue of navigation link materialization for expand-sub-select clause combined with minimal metadata | Completed (Code Review pending)
    Design documentation | Completed
    Implementation of $expand in context url for V4.01: based on PoC done during research | 3 days (WIP, pending on Design Review)
    Implementation of context url select clause parsing per OData V4.01 ABNF spec.
    Feature doc	| 1 day

Note: The above estimates include test pass for the initial implementation. Code review cycles are not included.

## Tracking:
- Prerequisite issue/PR: https://github.com/OData/odata.net/issues/1259; https://github.com/OData/odata.net/pull/1264
- "$expand for context-url" issue/PR: https://github.com/OData/odata.net/issues/1265;
