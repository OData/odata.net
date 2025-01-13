namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _Ж5Ⲥh16_ʺx3AʺↃ_h16Parser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._Ж5Ⲥh16_ʺx3AʺↃ_h16> Instance { get; } = from _Ⲥh16_ʺx3AʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥh16_ʺx3AʺↃParser.Instance.Many()
from _h16_1 in __GeneratedOdata.Parsers.Rules._h16Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._Ж5Ⲥh16_ʺx3AʺↃ_h16(_Ⲥh16_ʺx3AʺↃ_1, _h16_1);
    }
    
}
