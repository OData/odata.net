namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _deltatokenParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._deltatoken> Instance { get; } = from _ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6EʺParser.Instance
from _EQ_1 in __GeneratedOdataV2.Parsers.Rules._EQParser.Instance
from _ⲤqcharⲻnoⲻAMPↃ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤqcharⲻnoⲻAMPↃParser.Instance.Repeat(1, null)
select new __GeneratedOdataV2.CstNodes.Rules._deltatoken(_ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ_1, _EQ_1, new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Inners._ⲤqcharⲻnoⲻAMPↃ>(_ⲤqcharⲻnoⲻAMPↃ_1));
    }
    
}
