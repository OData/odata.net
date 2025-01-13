namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _boundActionCallParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._boundActionCall> Instance { get; } = from _namespace_1 in __GeneratedOdata.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _action_1 in __GeneratedOdata.Parsers.Rules._actionParser.Instance
select new __GeneratedOdata.CstNodes.Rules._boundActionCall(_namespace_1, _ʺx2Eʺ_1, _action_1);
    }
    
}
