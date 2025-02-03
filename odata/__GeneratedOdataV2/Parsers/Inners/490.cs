namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx65ʺ_꘡SIGN꘡_1ЖDIGITParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT> Instance { get; } = from _ʺx65ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx65ʺParser.Instance
from _SIGN_1 in __GeneratedOdataV2.Parsers.Rules._SIGNParser.Instance.Optional()
from _DIGIT_1 in __GeneratedOdataV2.Parsers.Rules._DIGITParser.Instance.Repeat(1, null)
select new __GeneratedOdataV2.CstNodes.Inners._ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT(_ʺx65ʺ_1, _SIGN_1.GetOrElse(null), new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Rules._DIGIT>(_DIGIT_1));
    }
    
}
