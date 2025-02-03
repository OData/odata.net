namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_keyPathLiteralParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _keyPathLiteral_1 in __GeneratedOdata.Parsers.Rules._keyPathLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral(_ʺx2Fʺ_1, _keyPathLiteral_1);
    }
    
}
