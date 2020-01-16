using System;

namespace Microsoft.OData.UriParser.Parsers
{
    [Flags]
    internal enum AllowedSegments
    {
        None = 0x0,

        EntityReferenceSegment = 0x01,

        CountSegment = 0x02,

        FilterSegment = 0x04,

        EachSegment = 0x08,

        PropertySegment = 0x10,

        TypeNameSegment = 0x20,

        OperationSegment = 0x40,

        KeyAsSegment = 0x80,

        DynamicSegment = 0x100,

        NavigationSourceSegment = 0x200,

        OperationImportSegment = 0x400,

        BatchSegment = 0x800,

        MetadataSegment = 0x1000,

        ValueSegment = 0x2000,

        UriTemplateSegment = 0x4000,

        EscapeFunctionSegment = 0x8000,

        SegmentsSupportingEscapeFunction = PropertySegment | KeyAsSegment | NavigationSourceSegment | OperationImportSegment | TypeNameSegment,

        All = EntityReferenceSegment | CountSegment | FilterSegment | EachSegment
            | PropertySegment | TypeNameSegment | OperationSegment | KeyAsSegment | DynamicSegment
            | NavigationSourceSegment | OperationImportSegment | BatchSegment | MetadataSegment | ValueSegment | UriTemplateSegment | EscapeFunctionSegment, 
    }
}
