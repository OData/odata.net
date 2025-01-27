namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qualifiedTypeDefinitionNameParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._qualifiedTypeDefinitionName> Instance { get; } = from _namespace_1 in __GeneratedOdata.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _typeDefinitionName_1 in __GeneratedOdata.Parsers.Rules._typeDefinitionNameParser.Instance
select new __GeneratedOdata.CstNodes.Rules._qualifiedTypeDefinitionName(_namespace_1, _ʺx2Eʺ_1, _typeDefinitionName_1);
    }
    
}
