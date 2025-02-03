namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx23ʺ_fragmentParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx23ʺ_fragment> Instance { get; } = from _ʺx23ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx23ʺParser.Instance
from _fragment_1 in __GeneratedOdataV2.Parsers.Rules._fragmentParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ʺx23ʺ_fragment(_ʺx23ʺ_1, _fragment_1);
    }
    
}
