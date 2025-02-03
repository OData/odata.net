namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _fullMultiLineStringLiteralParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._fullMultiLineStringLiteral> Instance { get; } = from _sridLiteral_1 in __GeneratedOdataV2.Parsers.Rules._sridLiteralParser.Instance
from _multiLineStringLiteral_1 in __GeneratedOdataV2.Parsers.Rules._multiLineStringLiteralParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._fullMultiLineStringLiteral(_sridLiteral_1, _multiLineStringLiteral_1);
    }
    
}
