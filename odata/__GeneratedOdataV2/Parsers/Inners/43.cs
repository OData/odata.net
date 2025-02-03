namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_keyPathLiteralParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2FʺParser.Instance
from _keyPathLiteral_1 in __GeneratedOdataV2.Parsers.Rules._keyPathLiteralParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral(_ʺx2Fʺ_1, _keyPathLiteral_1);
    }
    
}
