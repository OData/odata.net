namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _namespace_ʺx2EʺParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._namespace_ʺx2Eʺ> Instance { get; } = from _namespace_1 in __GeneratedOdataV2.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2EʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._namespace_ʺx2Eʺ(_namespace_1, _ʺx2Eʺ_1);
    }
    
}
