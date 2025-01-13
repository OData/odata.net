namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _geometryMultiPolygonParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._geometryMultiPolygon> Instance { get; } = from _geometryPrefix_1 in __GeneratedOdata.Parsers.Rules._geometryPrefixParser.Instance
from _SQUOTE_1 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
from _fullMultiPolygonLiteral_1 in __GeneratedOdata.Parsers.Rules._fullMultiPolygonLiteralParser.Instance
from _SQUOTE_2 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._geometryMultiPolygon(_geometryPrefix_1, _SQUOTE_1, _fullMultiPolygonLiteral_1, _SQUOTE_2);
    }
    
}
