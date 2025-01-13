namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _geometryPolygonParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._geometryPolygon> Instance { get; } = from _geometryPrefix_1 in __GeneratedOdata.Parsers.Rules._geometryPrefixParser.Instance
from _SQUOTE_1 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
from _fullPolygonLiteral_1 in __GeneratedOdata.Parsers.Rules._fullPolygonLiteralParser.Instance
from _SQUOTE_2 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._geometryPolygon(_geometryPrefix_1, _SQUOTE_1, _fullPolygonLiteral_1, _SQUOTE_2);
    }
    
}
