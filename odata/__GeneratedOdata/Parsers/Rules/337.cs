namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _geometryCollectionParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._geometryCollection> Instance { get; } = from _geometryPrefix_1 in __GeneratedOdata.Parsers.Rules._geometryPrefixParser.Instance
from _SQUOTE_1 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
from _fullCollectionLiteral_1 in __GeneratedOdata.Parsers.Rules._fullCollectionLiteralParser.Instance
from _SQUOTE_2 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._geometryCollection(_geometryPrefix_1, _SQUOTE_1, _fullCollectionLiteral_1, _SQUOTE_2);
    }
    
}
