namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _IPⲻliteralParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._IPⲻliteral> Instance { get; } = from _ʺx5Bʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx5BʺParser.Instance
from _ⲤIPv6addressⳆIPvFutureↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤIPv6addressⳆIPvFutureↃParser.Instance
from _ʺx5Dʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx5DʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._IPⲻliteral(_ʺx5Bʺ_1, _ⲤIPv6addressⳆIPvFutureↃ_1, _ʺx5Dʺ_1);
    }
    
}
