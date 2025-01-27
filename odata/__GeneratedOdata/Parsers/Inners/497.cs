namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Eʺ_fractionalSecondsParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx2Eʺ_fractionalSeconds> Instance { get; } = from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _fractionalSeconds_1 in __GeneratedOdata.Parsers.Rules._fractionalSecondsParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx2Eʺ_fractionalSeconds(_ʺx2Eʺ_1, _fractionalSeconds_1);
    }
    
}
