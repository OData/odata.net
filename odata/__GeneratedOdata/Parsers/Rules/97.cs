namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _deltatokenParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._deltatoken> Instance { get; } = from _ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6EʺParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _ⲤqcharⲻnoⲻAMPↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤqcharⲻnoⲻAMPↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._deltatoken(_ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ_1, _EQ_1, _ⲤqcharⲻnoⲻAMPↃ_1);
    }
    
}
