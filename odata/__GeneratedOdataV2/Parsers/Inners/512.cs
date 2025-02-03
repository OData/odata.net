namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ> Instance { get; } = from _DIGIT_1 in __GeneratedOdataV2.Parsers.Rules._DIGITParser.Instance.Repeat(1, null)
from _ʺx2Eʺ_1ЖDIGIT_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2Eʺ_1ЖDIGITParser.Instance.Optional()
from _ʺx53ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx53ʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ(new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Rules._DIGIT>(_DIGIT_1), _ʺx2Eʺ_1ЖDIGIT_1.GetOrElse(null), _ʺx53ʺ_1);
    }
    
}
