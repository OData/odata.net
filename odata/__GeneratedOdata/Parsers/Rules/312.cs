namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geoLiteralParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._geoLiteral> Instance { get; } = (_collectionLiteralParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._geoLiteral>(_lineStringLiteralParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._geoLiteral>(_multiPointLiteralParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._geoLiteral>(_multiLineStringLiteralParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._geoLiteral>(_multiPolygonLiteralParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._geoLiteral>(_pointLiteralParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._geoLiteral>(_polygonLiteralParser.Instance);
        
        public static class _collectionLiteralParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._geoLiteral._collectionLiteral> Instance { get; } = from _collectionLiteral_1 in __GeneratedOdata.Parsers.Rules._collectionLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._geoLiteral._collectionLiteral(_collectionLiteral_1);
        }
        
        public static class _lineStringLiteralParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._geoLiteral._lineStringLiteral> Instance { get; } = from _lineStringLiteral_1 in __GeneratedOdata.Parsers.Rules._lineStringLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._geoLiteral._lineStringLiteral(_lineStringLiteral_1);
        }
        
        public static class _multiPointLiteralParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._geoLiteral._multiPointLiteral> Instance { get; } = from _multiPointLiteral_1 in __GeneratedOdata.Parsers.Rules._multiPointLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._geoLiteral._multiPointLiteral(_multiPointLiteral_1);
        }
        
        public static class _multiLineStringLiteralParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._geoLiteral._multiLineStringLiteral> Instance { get; } = from _multiLineStringLiteral_1 in __GeneratedOdata.Parsers.Rules._multiLineStringLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._geoLiteral._multiLineStringLiteral(_multiLineStringLiteral_1);
        }
        
        public static class _multiPolygonLiteralParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._geoLiteral._multiPolygonLiteral> Instance { get; } = from _multiPolygonLiteral_1 in __GeneratedOdata.Parsers.Rules._multiPolygonLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._geoLiteral._multiPolygonLiteral(_multiPolygonLiteral_1);
        }
        
        public static class _pointLiteralParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._geoLiteral._pointLiteral> Instance { get; } = from _pointLiteral_1 in __GeneratedOdata.Parsers.Rules._pointLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._geoLiteral._pointLiteral(_pointLiteral_1);
        }
        
        public static class _polygonLiteralParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._geoLiteral._polygonLiteral> Instance { get; } = from _polygonLiteral_1 in __GeneratedOdata.Parsers.Rules._polygonLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._geoLiteral._polygonLiteral(_polygonLiteral_1);
        }
    }
    
}
