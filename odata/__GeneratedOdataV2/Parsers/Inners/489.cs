namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Eʺ_1ЖDIGITParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx2Eʺ_1ЖDIGIT> Instance { get; } = from _ʺx2Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2EʺParser.Instance
from _DIGIT_1 in __GeneratedOdataV2.Parsers.Rules._DIGITParser.Instance.Repeat(1, null)
select new __GeneratedOdataV2.CstNodes.Inners._ʺx2Eʺ_1ЖDIGIT(_ʺx2Eʺ_1, new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Rules._DIGIT>(_DIGIT_1));
    }
    
}
