namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _fullPolygonLiteralParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._fullPolygonLiteral> Instance { get; } = from _sridLiteral_1 in __GeneratedOdata.Parsers.Rules._sridLiteralParser.Instance
from _polygonLiteral_1 in __GeneratedOdata.Parsers.Rules._polygonLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._fullPolygonLiteral(_sridLiteral_1, _polygonLiteral_1);
    }
    
}
