namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _IPⲻliteralParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._IPⲻliteral> Instance { get; } = from _ʺx5Bʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx5BʺParser.Instance
from _ⲤIPv6addressⳆIPvFutureↃ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤIPv6addressⳆIPvFutureↃParser.Instance
from _ʺx5Dʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx5DʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._IPⲻliteral(_ʺx5Bʺ_1, _ⲤIPv6addressⳆIPvFutureↃ_1, _ʺx5Dʺ_1);
    }
    
}
