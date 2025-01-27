namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Eʺ_namespacePartParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx2Eʺ_namespacePart> Instance { get; } = from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _namespacePart_1 in __GeneratedOdata.Parsers.Rules._namespacePartParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx2Eʺ_namespacePart(_ʺx2Eʺ_1, _namespacePart_1);
    }
    
}
