namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _skiptokenParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._skiptoken> Instance { get; } = from _ʺx24x73x6Bx69x70x74x6Fx6Bx65x6Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx24x73x6Bx69x70x74x6Fx6Bx65x6EʺParser.Instance
from _EQ_1 in __GeneratedOdataV2.Parsers.Rules._EQParser.Instance
from _ⲤqcharⲻnoⲻAMPↃ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤqcharⲻnoⲻAMPↃParser.Instance.Repeat(1, null)
select new __GeneratedOdataV2.CstNodes.Rules._skiptoken(_ʺx24x73x6Bx69x70x74x6Fx6Bx65x6Eʺ_1, _EQ_1, new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Inners._ⲤqcharⲻnoⲻAMPↃ>(_ⲤqcharⲻnoⲻAMPↃ_1));
    }
    
}
