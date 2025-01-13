namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _skiptokenParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._skiptoken> Instance { get; } = from _ʺx24x73x6Bx69x70x74x6Fx6Bx65x6Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx24x73x6Bx69x70x74x6Fx6Bx65x6EʺParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _ⲤqcharⲻnoⲻAMPↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤqcharⲻnoⲻAMPↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._skiptoken(_ʺx24x73x6Bx69x70x74x6Fx6Bx65x6Eʺ_1, _EQ_1, _ⲤqcharⲻnoⲻAMPↃ_1);
    }
    
}
