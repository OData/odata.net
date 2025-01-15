namespace __Generated.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx2Eʺ_1ЖBITParser
    {
        public static Parser<__Generated.CstNodes.Inners._ʺx2Eʺ_1ЖBIT> Instance { get; } = from _ʺx2Eʺ_1 in __Generated.Parsers.Inners._ʺx2EʺParser.Instance
from _BIT_1 in __Generated.Parsers.Rules._BITParser.Instance.Many()
select new __Generated.CstNodes.Inners._ʺx2Eʺ_1ЖBIT(_ʺx2Eʺ_1, new __Generated.CstNodes.Inners.HelperRangedAtLeast1<__Generated.CstNodes.Rules._BIT>(_BIT_1));
    }
    
}
