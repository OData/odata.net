namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _fullMultiPointLiteralParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._fullMultiPointLiteral> Instance { get; } = from _sridLiteral_1 in __GeneratedOdata.Parsers.Rules._sridLiteralParser.Instance
from _multiPointLiteral_1 in __GeneratedOdata.Parsers.Rules._multiPointLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._fullMultiPointLiteral(_sridLiteral_1, _multiPointLiteral_1);
    }
    
}
