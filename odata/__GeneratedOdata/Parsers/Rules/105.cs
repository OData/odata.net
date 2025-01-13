namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _contextParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._context> Instance { get; } = from _ʺx23ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx23ʺParser.Instance
from _contextFragment_1 in __GeneratedOdata.Parsers.Rules._contextFragmentParser.Instance
select new __GeneratedOdata.CstNodes.Rules._context(_ʺx23ʺ_1, _contextFragment_1);
    }
    
}
