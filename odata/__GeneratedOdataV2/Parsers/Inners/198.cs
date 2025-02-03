namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx4Ex4Fx54ʺ_RWSParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx4Ex4Fx54ʺ_RWS> Instance { get; } = from _ʺx4Ex4Fx54ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx4Ex4Fx54ʺParser.Instance
from _RWS_1 in __GeneratedOdataV2.Parsers.Rules._RWSParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ʺx4Ex4Fx54ʺ_RWS(_ʺx4Ex4Fx54ʺ_1, _RWS_1);
    }
    
}
