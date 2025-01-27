namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx3Fʺ_queryParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx3Fʺ_query> Instance { get; } = from _ʺx3Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3FʺParser.Instance
from _query_1 in __GeneratedOdata.Parsers.Rules._queryParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx3Fʺ_query(_ʺx3Fʺ_1, _query_1);
    }
    
}
