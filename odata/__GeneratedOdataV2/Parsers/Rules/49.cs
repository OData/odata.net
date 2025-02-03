namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _queryOptionsParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._queryOptions> Instance { get; } = from _queryOption_1 in __GeneratedOdataV2.Parsers.Rules._queryOptionParser.Instance
from _Ⲥʺx26ʺ_queryOptionↃ_1 in Inners._Ⲥʺx26ʺ_queryOptionↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Rules._queryOptions(_queryOption_1, _Ⲥʺx26ʺ_queryOptionↃ_1);
    }
    
}
