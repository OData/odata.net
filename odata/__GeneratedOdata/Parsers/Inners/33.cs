namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx3Fʺ_queryOptionsParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx3Fʺ_queryOptions> Instance { get; } = from _ʺx3Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3FʺParser.Instance
from _queryOptions_1 in __GeneratedOdata.Parsers.Rules._queryOptionsParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx3Fʺ_queryOptions(_ʺx3Fʺ_1, _queryOptions_1);
    }
    
}
