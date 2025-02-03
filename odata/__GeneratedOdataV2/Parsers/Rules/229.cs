namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _namespaceParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._namespace> Instance { get; } = from _namespacePart_1 in __GeneratedOdataV2.Parsers.Rules._namespacePartParser.Instance
from _Ⲥʺx2Eʺ_namespacePartↃ_1 in Inners._Ⲥʺx2Eʺ_namespacePartↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Rules._namespace(_namespacePart_1, _Ⲥʺx2Eʺ_namespacePartↃ_1);
    }
    
}
