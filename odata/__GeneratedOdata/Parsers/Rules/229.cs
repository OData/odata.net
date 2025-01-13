namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _namespaceParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._namespace> Instance { get; } = from _namespacePart_1 in __GeneratedOdata.Parsers.Rules._namespacePartParser.Instance
from _Ⲥʺx2Eʺ_namespacePartↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx2Eʺ_namespacePartↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._namespace(_namespacePart_1, _Ⲥʺx2Eʺ_namespacePartↃ_1);
    }
    
}
