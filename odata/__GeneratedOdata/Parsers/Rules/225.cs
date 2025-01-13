namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _qualifiedEntityTypeNameParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._qualifiedEntityTypeName> Instance { get; } = from _namespace_1 in __GeneratedOdata.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _entityTypeName_1 in __GeneratedOdata.Parsers.Rules._entityTypeNameParser.Instance
select new __GeneratedOdata.CstNodes.Rules._qualifiedEntityTypeName(_namespace_1, _ʺx2Eʺ_1, _entityTypeName_1);
    }
    
}
