namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _geographyCollectionParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._geographyCollection> Instance { get; } = from _geographyPrefix_1 in __GeneratedOdata.Parsers.Rules._geographyPrefixParser.Instance
from _SQUOTE_1 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
from _fullCollectionLiteral_1 in __GeneratedOdata.Parsers.Rules._fullCollectionLiteralParser.Instance
from _SQUOTE_2 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._geographyCollection(_geographyPrefix_1, _SQUOTE_1, _fullCollectionLiteral_1, _SQUOTE_2);
    }
    
}
