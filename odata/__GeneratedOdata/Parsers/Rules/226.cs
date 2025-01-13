namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _qualifiedComplexTypeNameParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._qualifiedComplexTypeName> Instance { get; } = from _namespace_1 in __GeneratedOdata.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _complexTypeName_1 in __GeneratedOdata.Parsers.Rules._complexTypeNameParser.Instance
select new __GeneratedOdata.CstNodes.Rules._qualifiedComplexTypeName(_namespace_1, _ʺx2Eʺ_1, _complexTypeName_1);
    }
    
}
