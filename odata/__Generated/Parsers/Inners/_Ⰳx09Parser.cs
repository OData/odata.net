namespace __Generated.Parsers.Inners
{
    using Sprache;
    
    public static class _Ⰳx09Parser
    {
        public static Parser<__Generated.CstNodes.Inners._Ⰳx09> Instance { get; } = from _0_1 in __Generated.Parsers.Inners._0Parser.Instance
from _9_1 in __Generated.Parsers.Inners._9Parser.Instance
select new __Generated.CstNodes.Inners._Ⰳx09(_0_1, _9_1);
    }
    
}
