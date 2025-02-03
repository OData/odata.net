namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _fullPointLiteralParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._fullPointLiteral> Instance { get; } = from _sridLiteral_1 in __GeneratedOdataV2.Parsers.Rules._sridLiteralParser.Instance
from _pointLiteral_1 in __GeneratedOdataV2.Parsers.Rules._pointLiteralParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._fullPointLiteral(_sridLiteral_1, _pointLiteral_1);
    }
    
}
