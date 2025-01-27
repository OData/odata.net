namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _allOperationsInSchemaParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._allOperationsInSchema> Instance { get; } = from _namespace_1 in __GeneratedOdata.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _STAR_1 in __GeneratedOdata.Parsers.Rules._STARParser.Instance
select new __GeneratedOdata.CstNodes.Rules._allOperationsInSchema(_namespace_1, _ʺx2Eʺ_1, _STAR_1);
    }
    
}
