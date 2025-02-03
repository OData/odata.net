namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qualifiedEntityTypeNameParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qualifiedEntityTypeName> Instance { get; } = from _namespace_1 in __GeneratedOdataV2.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2EʺParser.Instance
from _entityTypeName_1 in __GeneratedOdataV2.Parsers.Rules._entityTypeNameParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._qualifiedEntityTypeName(_namespace_1, _ʺx2Eʺ_1, _entityTypeName_1);
    }
    
}
