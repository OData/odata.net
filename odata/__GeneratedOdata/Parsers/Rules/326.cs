namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geographyPointParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._geographyPoint> Instance { get; } = from _geographyPrefix_1 in __GeneratedOdata.Parsers.Rules._geographyPrefixParser.Instance
from _SQUOTE_1 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
from _fullPointLiteral_1 in __GeneratedOdata.Parsers.Rules._fullPointLiteralParser.Instance
from _SQUOTE_2 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._geographyPoint(_geographyPrefix_1, _SQUOTE_1, _fullPointLiteral_1, _SQUOTE_2);
    }
    
}
