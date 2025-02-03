namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _fullMultiPointLiteralParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._fullMultiPointLiteral> Instance { get; } = from _sridLiteral_1 in __GeneratedOdataV2.Parsers.Rules._sridLiteralParser.Instance
from _multiPointLiteral_1 in __GeneratedOdataV2.Parsers.Rules._multiPointLiteralParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._fullMultiPointLiteral(_sridLiteral_1, _multiPointLiteral_1);
    }
    
}
