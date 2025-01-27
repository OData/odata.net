namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geographyMultiLineStringParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._geographyMultiLineString> Instance { get; } = from _geographyPrefix_1 in __GeneratedOdata.Parsers.Rules._geographyPrefixParser.Instance
from _SQUOTE_1 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
from _fullMultiLineStringLiteral_1 in __GeneratedOdata.Parsers.Rules._fullMultiLineStringLiteralParser.Instance
from _SQUOTE_2 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._geographyMultiLineString(_geographyPrefix_1, _SQUOTE_1, _fullMultiLineStringLiteral_1, _SQUOTE_2);
    }
    
}
