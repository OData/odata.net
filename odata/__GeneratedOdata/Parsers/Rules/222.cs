namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _expParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._exp> Instance { get; } = from _ʺx65ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx65ʺParser.Instance
from _ʺx2DʺⳆʺx2Bʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2DʺⳆʺx2BʺParser.Instance.Optional()
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Repeat(1, null)
select new __GeneratedOdata.CstNodes.Rules._exp(_ʺx65ʺ_1, _ʺx2DʺⳆʺx2Bʺ_1.GetOrElse(null), new __GeneratedOdata.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdata.CstNodes.Rules._DIGIT>(_DIGIT_1));
    }
    
}
