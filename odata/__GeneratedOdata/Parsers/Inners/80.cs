namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _entityIdOption_ʺx26ʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._entityIdOption_ʺx26ʺ> Instance { get; } = from _entityIdOption_1 in __GeneratedOdata.Parsers.Rules._entityIdOptionParser.Instance
from _ʺx26ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx26ʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._entityIdOption_ʺx26ʺ(_entityIdOption_1, _ʺx26ʺ_1);
    }
    
}
