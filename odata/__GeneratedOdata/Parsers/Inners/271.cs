namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _namespace_ʺx2EʺParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._namespace_ʺx2Eʺ> Instance { get; } = from _namespace_1 in __GeneratedOdata.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._namespace_ʺx2Eʺ(_namespace_1, _ʺx2Eʺ_1);
    }
    
}
