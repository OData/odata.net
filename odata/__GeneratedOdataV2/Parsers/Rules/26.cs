namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _ordinalIndexParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._ordinalIndex> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2FʺParser.Instance
from _DIGIT_1 in __GeneratedOdataV2.Parsers.Rules._DIGITParser.Instance.Repeat(1, null)
select new __GeneratedOdataV2.CstNodes.Rules._ordinalIndex(_ʺx2Fʺ_1, new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Rules._DIGIT>(_DIGIT_1));
    }
    
}
