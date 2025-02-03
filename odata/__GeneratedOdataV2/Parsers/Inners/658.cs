namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ж5Ⲥh16_ʺx3AʺↃ_h16Parser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._Ж5Ⲥh16_ʺx3AʺↃ_h16> Instance { get; } = from _Ⲥh16_ʺx3AʺↃ_1 in __GeneratedOdataV2.Parsers.Inners._Ⲥh16_ʺx3AʺↃParser.Instance.Repeat(0, 5)
from _h16_1 in __GeneratedOdataV2.Parsers.Rules._h16Parser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._Ж5Ⲥh16_ʺx3AʺↃ_h16(new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtMost5<__GeneratedOdataV2.CstNodes.Inners._Ⲥh16_ʺx3AʺↃ>(_Ⲥh16_ʺx3AʺↃ_1), _h16_1);
    }
    
}
