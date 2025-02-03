namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _expParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._exp> Instance { get; } = from _ʺx65ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx65ʺParser.Instance
from _ʺx2DʺⳆʺx2Bʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2DʺⳆʺx2BʺParser.Instance.Optional()
from _DIGIT_1 in __GeneratedOdataV2.Parsers.Rules._DIGITParser.Instance.Repeat(1, null)
select new __GeneratedOdataV2.CstNodes.Rules._exp(_ʺx65ʺ_1, _ʺx2DʺⳆʺx2Bʺ_1.GetOrElse(null), new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Rules._DIGIT>(_DIGIT_1));
    }
    
}
