namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _ordinalIndexParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._ordinalIndex> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Repeat(1, null)
select new __GeneratedOdata.CstNodes.Rules._ordinalIndex(_ʺx2Fʺ_1, new __GeneratedOdata.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdata.CstNodes.Rules._DIGIT>(_DIGIT_1));
    }
    
}
