namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ж3Ⲥh16_ʺx3AʺↃ_h16Parser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._Ж3Ⲥh16_ʺx3AʺↃ_h16> Instance { get; } = from _Ⲥh16_ʺx3AʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥh16_ʺx3AʺↃParser.Instance.Repeat(0, 3)
from _h16_1 in __GeneratedOdata.Parsers.Rules._h16Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._Ж3Ⲥh16_ʺx3AʺↃ_h16(new __GeneratedOdata.CstNodes.Inners.HelperRangedAtMost3<__GeneratedOdata.CstNodes.Inners._Ⲥh16_ʺx3AʺↃ>(_Ⲥh16_ʺx3AʺↃ_1), _h16_1);
    }
    
}
