---
layout: post
title: "Override type annotation in serialization"
description: ""
category: "5. OData Features"
---

As you may know, all the OData items (`ODataResource`, `ODataResourceSet`, etc.) read from the payload are inherited from `ODataAnnotatable`. In `ODataAnnotatable`, there is a property called `TypeAnnotation` whose type is `ODataTypeAnnotation`. It is used to store the `@odata.type` annotation read from or written to the payload. The reasons why we don't merge it into the instance annotations in `ODataAnnotatable` are that: 1) for performance improvement; 2) in ATOM format, we also have OData type annotation.

During deserialization, the `TypeAnnotation` property will be set by the OData readers into each OData item read from the payload. During serialization, the `TypeAnnotation` property itself and its `TypeName` property together will control how OData type annotation will be written to the payload.

The control policies are:

- If the `TypeAnnotation` property itself is `null`, then we will rely on the underlying logic in ODataLib and Web API OData to determine what OData type annotation to be written to the payload.
- If the `TypeAnnotation` property is not `null` but the `TypeName` property is `null`, then absolutely NO OData type annotation will be written to the payload.
- If the `TypeAnnotation` property is not `null and the `TypeName` property is not `null` as well, then the string value specified will be written to the payload.

That said, if you specify a value for the `TypeAnnotation` property, its `TypeName` property will override the underlying logic anyway. So please pay attention to the value of the `TypeName` property.