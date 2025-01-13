namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _geometryMultiPointParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._geometryMultiPoint> Instance { get; } = from _geometryPrefix_1 in __GeneratedOdata.Parsers.Rules._geometryPrefixParser.Instance
from _SQUOTE_1 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
from _fullMultiPointLiteral_1 in __GeneratedOdata.Parsers.Rules._fullMultiPointLiteralParser.Instance
from _SQUOTE_2 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._geometryMultiPoint(_geometryPrefix_1, _SQUOTE_1, _fullMultiPointLiteral_1, _SQUOTE_2);
    }
    
}
