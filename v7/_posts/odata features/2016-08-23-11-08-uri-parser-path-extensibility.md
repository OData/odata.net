---
layout: post
title: "OData Uri Path Parser Extensibility"
description: "This feature enable developers to customize Uri path parser"
category: "5. OData Features"
---

In order to support more comprehensive OData Uri path, from ODataLib 7.0, we support Uri path parser customization in two parts:

-   Allow developers to customize how to separate a Uri into segments in string.
-   Allow developers to customize how to bind those raw string segments unrecognized by `ODataUriParser` to the model and create `ODataPathSegments`.

For example, we have a Uri http://localhost/odata/drives('C')/root:/OData/Docs/Features/Uri%20Parser%20Path%20Extensibility.doc:/size which is used to get the size of a local file "C:\OData\Docs\Features\Uri Parser Path Extensible.doc". But `ODataUriParser.ParsePath()` doesn't know how to bind this path to the edm model. So we want to provide a way to let developers define how to parse it.

Following sections will provide some sample codes to support this feature.


# Model #

Given a model as following:

```
<?xml version="1.0" encoding="utf-8"?>
<edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx">
  <edmx:DataServices>
    <Schema Namespace="SampleService" xmlns="http://docs.oasis-open.org/odata/ns/edm">
      <EntityType Name="drive">
        <Key>
          <PropertyRef Name="id" />
        </Key>
        <Property Name="id" Type="Edm.String" Nullable="false" />
        <NavigationProperty Name="items" Type="Collection(SampleService.item)" ContainsTarget="true" />
      </EntityType>
      <EntityType Name="item" OpenType="true">
        <Key>
          <PropertyRef Name="id" />
        </Key>
        <Property Name="id" Type="Edm.String" Nullable="false" />
        <Property Name="size" Type="Edm.Int64" />
      </EntityType>
      <EntityContainer Name="SampleService">
        <EntitySet Name="drives" EntityType="SampleService.drive" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>
```

# Separate a Uri into Segments #

`UriPathParser` provides a public virtual API `ParsePathIntoSegments(Uri fullUri, Uri serviceBaseUri)` to customize how to separate a Uri into segments in raw string.

Developers can define their own `UriPathParser`, register this class by DI (Please refer to  [Dependency Injection support](#01-04-di-support)) and override the `ParsePathIntoSegments`. Then, ODataLib will use this API to separate the Uri into several segments.

    public class UriPathParser
    {
        public virtual ICollection<string> ParsePathIntoSegments(Uri fullUri, Uri serviceBaseUri)
    }

## Customized `UriPathParser` #
 
    public class CustomizedUriPathParser : UriPathParser
    {
        public UriPathParserType UriPathParserType { get; set; }
        public CustomizedUriPathParser(ODataUriParserSettings settings)
            : base(settings)
        { }

        public override ICollection<string> ParsePathIntoSegments(Uri fullUri, Uri serviceBaseUri)
        {
            Uri uri = fullUri;
            int numberOfSegmentsToSkip = 0;

            numberOfSegmentsToSkip = serviceBaseUri.AbsolutePath.Split('/').Length - 1;
            string[] uriSegments = uri.AbsolutePath.Split('/');

            List<string> segments = new List<string>();
            List<string> tmpSegments = new List<string>();
            bool end = true;
            for (int i = numberOfSegmentsToSkip; i < uriSegments.Length; i++)
            {
                string segment = uriSegments[i];
                if (!segment.StartsWith("root:") && segment.EndsWith(":"))
                {
                    tmpSegments.Add(segment);
                    segments.Add(Uri.UnescapeDataString(string.Join("/", tmpSegments)));
                    end = true;
                }
                else if (segment.StartsWith("root:") || !end)
                {
                    end = false;
                    tmpSegments.Add(segment);
                    continue;
                }
                else
                {
                    segments.Add(segment);
                }
            }

            return segments.ToArray();
        }
    }

This class defines its own `ParsePathIntoSegments` to separate the Uri into segments. Developers can register this class by `builder.AddService<UriPathParser, CustomizedUriPathParser>(ServiceLifetime.Scoped)`. 

`ParsePathIntoSegments` considers "root:/OData/Docs/Features/Uri%20Parser%20Path%20Extensibility.doc:" as one segment. It parsed the whole Uri into three segments in string.

    [0]: "drives('C')"
    [1]: "root:/OData/Docs/Features/Uri Parser Path Extensibility.doc:"
    [2]: "size"

# Bind Unknown Raw-string Segments to Model #

After the above step, developers now can define how to bind the  "root:/OData/Docs/Features/Uri Parser Path Extensibility.doc:" which is unknown for `ODataUriParser`.

For all unknown segments, ODataLib provides a `DynamicPathSegment` class to represent the meaning of them in model. `DynamicPathSegment` could be used for both open property segments or other dynamic path segments.

`ODataUriParser` also provides a Property `ParseDynamicPathSegmentFunc` for developers to bind those unknown raw-string segments to model.

    public delegate ICollection<ODataPathSegment> ParseDynamicPathSegment(ODataPathSegment previous, string identifier, string parenthesisExpression);

    public ParseDynamicPathSegment ParseDynamicPathSegmentFunc
    {
        get { return this.configuration.ParseDynamicPathSegmentFunc; }
        set { this.configuration.ParseDynamicPathSegmentFunc = value; }
    }

By default, if developers do not set the `ParseDynamicPathSegmentFunc` , ODataLib will consider the unknown segment as an open property. Or, ODataLib will use this function to bind the segment and return a collection of `ODataPathSegment`. Then, ODataLib will parse the following segment according to this binding result.

1. Developers can set `ParseDynamicPathSegmentFunc` as following which parses the second segment into a `DynamicPathSegment` with `IEdmEntityType` "Item". Then `ODataUriParser` will parse the last segment "size" into a `PropertySegment`.

	    uriParser.ParseDynamicPathSegmentFunc = (previous, identifier, parenthesisExpression) =>
	    {
	        switch (identifier)
	        {
	            case "root:/OData/Docs/Features/Uri Parser Path Extensibility.doc:":
	                return new List<ODataPathSegment> { new DynamicPathSegment(identifier, itemType, true) };
	            default:
	                throw new Exception("Not supported Type");
	        }
	    };

    Then, the `ODataPath` should be:

	    [0]: EntitySetSegment : "drives"
	    [1]: KeySegment : "C"
	    [2]: DynamicPathSegment: "root:/OData/Docs/Features/Uri Parser Path Extensibility.doc:"
	    [3]: PropertySegment: "size"

2. Developers can also set `ParseDynamicPathSegmentFunc` as following which will translate the unknown string segment into a `NavigationPropertySegment` and `KeySegment`:

	    uriParser.ParseDynamicPathSegmentFunc = (previous, identifier, parenthesisExpression) =>
	    {
	        switch (identifier)
	        {
	            case "root:/OData/Docs/Features/Uri Parser Path Extensibility.doc":
	                return new List<ODataPathSegment>
	                {
	                    new NavigationPropertySegment(navProp, navSource);,
	                    new KeySegment(new Dictionary<string, object>() { { "id", "01VL3Q7L36JOJUAPXGDNAZ4FVIGCTMLL46" } }, itemType, navSource);
	                };
	            default:
	                throw new Exception("Not supported Type");
	        }
	    };

    Then, the `ODataPath` should be:

	    [0]: EntitySetSegment : "drives"
	    [1]: KeySegment : "C"
	    [2]: NavigationPropertySegment: "items"
	    [3]: KeySegment: "01VL3Q7L36JOJUAPXGDNAZ4FVIGCTMLL46"
	    [4]: PropertySegment: "size"


On server side, developers can implement their own behavior of CRUD according to the parse result.
