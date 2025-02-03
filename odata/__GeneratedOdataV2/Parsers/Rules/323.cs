namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geographyMultiPolygonParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._geographyMultiPolygon> Instance { get; } = from _geographyPrefix_1 in __GeneratedOdataV2.Parsers.Rules._geographyPrefixParser.Instance
from _SQUOTE_1 in __GeneratedOdataV2.Parsers.Rules._SQUOTEParser.Instance
from _fullMultiPolygonLiteral_1 in __GeneratedOdataV2.Parsers.Rules._fullMultiPolygonLiteralParser.Instance
from _SQUOTE_2 in __GeneratedOdataV2.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._geographyMultiPolygon(_geographyPrefix_1, _SQUOTE_1, _fullMultiPolygonLiteral_1, _SQUOTE_2);
    }
    
}
