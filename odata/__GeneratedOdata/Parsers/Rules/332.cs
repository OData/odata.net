namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geographyPolygonParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._geographyPolygon> Instance { get; } = from _geographyPrefix_1 in __GeneratedOdata.Parsers.Rules._geographyPrefixParser.Instance
from _SQUOTE_1 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
from _fullPolygonLiteral_1 in __GeneratedOdata.Parsers.Rules._fullPolygonLiteralParser.Instance
from _SQUOTE_2 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._geographyPolygon(_geographyPrefix_1, _SQUOTE_1, _fullPolygonLiteral_1, _SQUOTE_2);
    }
    
}
