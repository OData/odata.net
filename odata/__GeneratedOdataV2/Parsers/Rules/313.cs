namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geographyLineStringParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._geographyLineString> Instance { get; } = from _geographyPrefix_1 in __GeneratedOdataV2.Parsers.Rules._geographyPrefixParser.Instance
from _SQUOTE_1 in __GeneratedOdataV2.Parsers.Rules._SQUOTEParser.Instance
from _fullLineStringLiteral_1 in __GeneratedOdataV2.Parsers.Rules._fullLineStringLiteralParser.Instance
from _SQUOTE_2 in __GeneratedOdataV2.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._geographyLineString(_geographyPrefix_1, _SQUOTE_1, _fullLineStringLiteral_1, _SQUOTE_2);
    }
    
}
