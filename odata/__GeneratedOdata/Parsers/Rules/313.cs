namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geographyLineStringParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._geographyLineString> Instance { get; } = from _geographyPrefix_1 in __GeneratedOdata.Parsers.Rules._geographyPrefixParser.Instance
from _SQUOTE_1 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
from _fullLineStringLiteral_1 in __GeneratedOdata.Parsers.Rules._fullLineStringLiteralParser.Instance
from _SQUOTE_2 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._geographyLineString(_geographyPrefix_1, _SQUOTE_1, _fullLineStringLiteral_1, _SQUOTE_2);
    }
    
}
