namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx26ʺ_entityIdOptionParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx26ʺ_entityIdOption> Instance { get; } = from _ʺx26ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx26ʺParser.Instance
from _entityIdOption_1 in __GeneratedOdataV2.Parsers.Rules._entityIdOptionParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ʺx26ʺ_entityIdOption(_ʺx26ʺ_1, _entityIdOption_1);
    }
    
}
