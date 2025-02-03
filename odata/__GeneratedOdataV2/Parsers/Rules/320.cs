namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geographyMultiPointParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._geographyMultiPoint> Instance { get; } = from _geographyPrefix_1 in __GeneratedOdataV2.Parsers.Rules._geographyPrefixParser.Instance
from _SQUOTE_1 in __GeneratedOdataV2.Parsers.Rules._SQUOTEParser.Instance
from _fullMultiPointLiteral_1 in __GeneratedOdataV2.Parsers.Rules._fullMultiPointLiteralParser.Instance
from _SQUOTE_2 in __GeneratedOdataV2.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._geographyMultiPoint(_geographyPrefix_1, _SQUOTE_1, _fullMultiPointLiteral_1, _SQUOTE_2);
    }
    
}
