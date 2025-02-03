namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx3Fʺ_queryOptionsParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx3Fʺ_queryOptions> Instance { get; } = from _ʺx3Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3FʺParser.Instance
from _queryOptions_1 in __GeneratedOdataV2.Parsers.Rules._queryOptionsParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ʺx3Fʺ_queryOptions(_ʺx3Fʺ_1, _queryOptions_1);
    }
    
}
