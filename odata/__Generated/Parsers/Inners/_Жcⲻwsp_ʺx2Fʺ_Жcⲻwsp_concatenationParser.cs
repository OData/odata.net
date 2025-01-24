namespace __Generated.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationParser
    {
        public static IParser<char, __Generated.CstNodes.Inners._Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenation> Instance { get; } = from _cⲻwsp_1 in __Generated.Parsers.Rules._cⲻwspParser.Instance.Many()
from _ʺx2Fʺ_1 in __Generated.Parsers.Inners._ʺx2FʺParser.Instance
from _cⲻwsp_2 in __Generated.Parsers.Rules._cⲻwspParser.Instance.Many()
from _concatenation_1 in __Generated.Parsers.Rules._concatenationParser.Instance
select new __Generated.CstNodes.Inners._Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenation(_cⲻwsp_1, _ʺx2Fʺ_1, _cⲻwsp_2, _concatenation_1);
    }
    
}
