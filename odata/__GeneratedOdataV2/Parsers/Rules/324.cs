namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _fullMultiPolygonLiteralParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._fullMultiPolygonLiteral> Instance { get; } = from _sridLiteral_1 in __GeneratedOdataV2.Parsers.Rules._sridLiteralParser.Instance
from _multiPolygonLiteral_1 in __GeneratedOdataV2.Parsers.Rules._multiPolygonLiteralParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._fullMultiPolygonLiteral(_sridLiteral_1, _multiPolygonLiteral_1);
    }
    
}
