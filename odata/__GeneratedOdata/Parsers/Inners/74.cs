namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx26ʺ_queryOptionParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx26ʺ_queryOption> Instance { get; } = from _ʺx26ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx26ʺParser.Instance
from _queryOption_1 in __GeneratedOdata.Parsers.Rules._queryOptionParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx26ʺ_queryOption(_ʺx26ʺ_1, _queryOption_1);
    }
    
}
