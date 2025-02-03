namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _h16_ʺx3Aʺ_h16Parser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._h16_ʺx3Aʺ_h16> Instance { get; } = from _h16_1 in __GeneratedOdataV2.Parsers.Rules._h16Parser.Instance
from _ʺx3Aʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3AʺParser.Instance
from _h16_2 in __GeneratedOdataV2.Parsers.Rules._h16Parser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._h16_ʺx3Aʺ_h16(_h16_1, _ʺx3Aʺ_1, _h16_2);
    }
    
}
