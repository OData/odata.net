namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx26ʺ_entityCastOptionParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx26ʺ_entityCastOption> Instance { get; } = from _ʺx26ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx26ʺParser.Instance
from _entityCastOption_1 in __GeneratedOdataV2.Parsers.Rules._entityCastOptionParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ʺx26ʺ_entityCastOption(_ʺx26ʺ_1, _entityCastOption_1);
    }
    
}
