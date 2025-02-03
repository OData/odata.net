namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qualifiedActionNameParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qualifiedActionName> Instance { get; } = from _namespace_1 in __GeneratedOdataV2.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2EʺParser.Instance
from _action_1 in __GeneratedOdataV2.Parsers.Rules._actionParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._qualifiedActionName(_namespace_1, _ʺx2Eʺ_1, _action_1);
    }
    
}
