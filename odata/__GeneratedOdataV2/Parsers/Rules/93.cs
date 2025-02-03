namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _allOperationsInSchemaParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._allOperationsInSchema> Instance { get; } = from _namespace_1 in __GeneratedOdataV2.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2EʺParser.Instance
from _STAR_1 in __GeneratedOdataV2.Parsers.Rules._STARParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._allOperationsInSchema(_namespace_1, _ʺx2Eʺ_1, _STAR_1);
    }
    
}
