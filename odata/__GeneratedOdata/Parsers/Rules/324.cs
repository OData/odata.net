namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _fullMultiPolygonLiteralParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._fullMultiPolygonLiteral> Instance { get; } = from _sridLiteral_1 in __GeneratedOdata.Parsers.Rules._sridLiteralParser.Instance
from _multiPolygonLiteral_1 in __GeneratedOdata.Parsers.Rules._multiPolygonLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._fullMultiPolygonLiteral(_sridLiteral_1, _multiPolygonLiteral_1);
    }
    
}
