namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geometryLineStringParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._geometryLineString> Instance { get; } = from _geometryPrefix_1 in __GeneratedOdata.Parsers.Rules._geometryPrefixParser.Instance
from _SQUOTE_1 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
from _fullLineStringLiteral_1 in __GeneratedOdata.Parsers.Rules._fullLineStringLiteralParser.Instance
from _SQUOTE_2 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._geometryLineString(_geometryPrefix_1, _SQUOTE_1, _fullLineStringLiteral_1, _SQUOTE_2);
    }
    
}
