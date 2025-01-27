namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _1ЖDIGIT_ʺx44ʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._1ЖDIGIT_ʺx44ʺ> Instance { get; } = from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Repeat(1, null)
from _ʺx44ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx44ʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._1ЖDIGIT_ʺx44ʺ(new __GeneratedOdata.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdata.CstNodes.Rules._DIGIT>(_DIGIT_1), _ʺx44ʺ_1);
    }
    
}
