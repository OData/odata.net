namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡Parser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡> Instance { get; } = from _ʺx3Aʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3AʺParser.Instance
from _second_1 in __GeneratedOdataV2.Parsers.Rules._secondParser.Instance
from _ʺx2Eʺ_fractionalSeconds_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2Eʺ_fractionalSecondsParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Inners._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡(_ʺx3Aʺ_1, _second_1, _ʺx2Eʺ_fractionalSeconds_1.GetOrElse(null));
    }
    
}
