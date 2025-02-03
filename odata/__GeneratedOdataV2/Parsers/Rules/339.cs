namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geometryMultiLineStringParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._geometryMultiLineString> Instance { get; } = from _geometryPrefix_1 in __GeneratedOdataV2.Parsers.Rules._geometryPrefixParser.Instance
from _SQUOTE_1 in __GeneratedOdataV2.Parsers.Rules._SQUOTEParser.Instance
from _fullMultiLineStringLiteral_1 in __GeneratedOdataV2.Parsers.Rules._fullMultiLineStringLiteralParser.Instance
from _SQUOTE_2 in __GeneratedOdataV2.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._geometryMultiLineString(_geometryPrefix_1, _SQUOTE_1, _fullMultiLineStringLiteral_1, _SQUOTE_2);
    }
    
}
