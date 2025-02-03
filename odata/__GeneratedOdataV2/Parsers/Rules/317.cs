namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geographyMultiLineStringParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._geographyMultiLineString> Instance { get; } = from _geographyPrefix_1 in __GeneratedOdataV2.Parsers.Rules._geographyPrefixParser.Instance
from _SQUOTE_1 in __GeneratedOdataV2.Parsers.Rules._SQUOTEParser.Instance
from _fullMultiLineStringLiteral_1 in __GeneratedOdataV2.Parsers.Rules._fullMultiLineStringLiteralParser.Instance
from _SQUOTE_2 in __GeneratedOdataV2.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._geographyMultiLineString(_geographyPrefix_1, _SQUOTE_1, _fullMultiLineStringLiteral_1, _SQUOTE_2);
    }
    
}
