namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _geometryMultiLineStringParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._geometryMultiLineString> Instance { get; } = from _geometryPrefix_1 in __GeneratedOdata.Parsers.Rules._geometryPrefixParser.Instance
from _SQUOTE_1 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
from _fullMultiLineStringLiteral_1 in __GeneratedOdata.Parsers.Rules._fullMultiLineStringLiteralParser.Instance
from _SQUOTE_2 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._geometryMultiLineString(_geometryPrefix_1, _SQUOTE_1, _fullMultiLineStringLiteral_1, _SQUOTE_2);
    }
    
}
