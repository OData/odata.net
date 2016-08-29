---
layout: post
title: "Breaking changes about merge entity and complex"
description: ""
category: "4. Release Notes"
---

This page will describes the Public API changes for "Merge entity and complex". The basic idea is that we named both an entity and a complex instance as an `ODataResource`, and named a collection of entity or a collection of complex as an `ODataResourceSet`. 

## API Changes ##

Following is difference of public Apis between ODataLib 7.0 and ODataLib 6.15.

||ODataLib 6.15|ODataLib 7.0|
|---|---|---|
||ODataEntry|ODataResource|
||ODataComplexValue|ODataResource|
||ODataFeed|ODataResourceSet|
||ODataCollectionValue for Complex|ODataResourceSet|
||ODataNavigationLink|ODataNestedResourceInfo|
||||
|ODataPayloadKind|Entry|Resource|
||Feed|ResourceSet|
||||
|ODataReaderState|EntryStart|ResourceStart|
||EntryEnd|ResourceEnd
||FeedStart|ResourceSetStart|
||FeedEnd|ResourceSetEnd|
||NavigationLinkStart|NestedResourceInfoStart|
||NavigationLinkEnd|NestedResourceInfoEnd|
||||
|ODataParameterReaderState  |Entry|Resource|
||Feed|ResourceSet|
||||
|ODataInputContext|ODataReader CreateEntryReader (IEdmNavigationSource navigationSource, IEdmEntityType expectedEntityType)|ODataReader CreateResourceReader(IEdmNavigationSource navigationSource, IEdmStructuredType expectedResourceType)|
||ODataReader CreateFeedReader (IEdmEntitySetBase entitySet, IEdmEntityType expectedBaseEntityType)|ODataReader CreateResourceSetReader(IEdmEntitySetBase entitySet, IEdmStructuredType expectedResourceType)|
||||
|ODataOutputContext|ODataWriter CreateODataEntryWriter (IEdmNavigationSource navigationSource, IEdmEntityType entityType)|ODataWriter CreateODataResourceWriter(IEdmNavigationSource navigationSource, IEdmStructuredType resourceType)|
||ODataWriter CreateODataFeedWriter (IEdmEntitySetBase entitySet, IEdmEntityType entityType)|ODataWriter CreateODataResourceSetWriter(IEdmEntitySetBase entitySet, IEdmStructuredType resourceType)|
||||
|ODataParameterReader|ODataReader CreateEntryReader ()|ODataReader CreateResourceReader ()|
||ODataReader CreateFeedReader ()|ODataReader CreateResourceSetReader ()|
||||
|ODataParameterWriter|ODataWriter CreateEntryWriter (string parameterName)|ODataWriter CreateResourceWriter (string parameterName)|
||ODataWriter CreateFeedWriter (string parameterName)|ODataWriter CreateResourceSetWriter (string parameterName)|
||||
|ODataMessageReader|public Microsoft.OData.Core.ODataReader CreateODataEntryReader ()|public Microsoft.OData.ODataReader CreateODataResourceReader ()|
||public Microsoft.OData.Core.ODataReader CreateODataEntryReader (IEdmEntityType entityType)|public Microsoft.OData.ODataReader CreateODataResourceReader (IEdmStructuredType resourceType)|
||public Microsoft.OData.Core.ODataReader CreateODataEntryReader (IEdmNavigationSource navigationSource, IEdmEntityType entityType)|public Microsoft.OData.ODataReader CreateODataResourceReader (IEdmNavigationSource navigationSource, IEdmStructuredType resourceType)|
||public Microsoft.OData.Core.ODataReader CreateODataFeedReader ()|public Microsoft.OData.ODataReader CreateODataResourceSetReader ()|
||public Microsoft.OData.Core.ODataReader CreateODataFeedReader (IEdmEntityType expectedBaseEntityType)|public Microsoft.OData.ODataReader CreateODataResourceSetReader (IEdmStructuredType expectedResourceType)|
||public Microsoft.OData.Core.ODataReader CreateODataFeedReader (IEdmEntitySetBase entitySet, IEdmEntityType expectedBaseEntityType|public Microsoft.OData.ODataReader CreateODataResourceSetReader (IEdmEntitySetBase entitySet, IEdmStructuredType expectedResourceType)|
|||public ODataReader CreateODataUriParameterResourceSetReader(IEdmEntitySetBase entitySet, IEdmStructuredType expectedResourceType)|
|||public ODataReader CreateODataUriParameterResourceReader(IEdmNavigationSource navigationSource, IEdmStructuredType expectedResourceType)|
||||
|ODataMessageWriter|public Microsoft.OData.Core.ODataWriter CreateODataEntryWriter ()|public Microsoft.OData.ODataWriter CreateODataResourceSetWriter ()|
||public Microsoft.OData.Core.ODataWriter CreateODataEntryWriter (IEdmNavigationSource navigationSource)|public Microsoft.OData.ODataWriter CreateODataResourceSetWriter (IEdmEntitySetBase entitySet)|
||public Microsoft.OData.Core.ODataWriter CreateODataEntryWriter (IEdmNavigationSource navigationSource, IEdmEntityType entityType)|public Microsoft.OData.ODataWriter CreateODataResourceSetWriter (IEdmEntitySetBase entitySet, IEdmStructuredType resourceType)|
||public Microsoft.OData.Core.ODataWriter CreateODataFeedWriter ()|public Microsoft.OData.ODataWriter CreateODataResourceWriter ()|
||public Microsoft.OData.Core.ODataWriter CreateODataFeedWriter (IEdmEntitySetBase entitySet)|public Microsoft.OData.ODataWriter CreateODataResourceWriter (IEdmNavigationSource navigationSource)|
||public Microsoft.OData.Core.ODataWriter CreateODataFeedWriter (IEdmEntitySetBase entitySet, IEdmEntityType entityType)|public Microsoft.OData.ODataWriter CreateODataResourceWriter (IEdmNavigationSource navigationSource, IEdmStructuredType resourceType)|
|||public ODataWriter CreateODataUriParameterResourceWriter(IEdmNavigationSource navigationSource, IEdmStructuredType resourceType)|
|||public ODataWriter CreateODataUriParameterResourceSetWriter(IEdmEntitySetBase entitySetBase, IEdmStructuredType resourceType)|
