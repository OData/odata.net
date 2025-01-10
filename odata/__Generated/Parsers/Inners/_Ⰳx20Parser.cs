namespace __Generated.Parsers.Inners
{
    using Sprache;
    
    public static class _Ⰳx20Parser
    {
        public static Parser<__Generated.CstNodes.Inners._Ⰳx20> Instance { get; } = from _2_1 in __Generated.Parsers.Inners._2Parser.Instance
from _0_1 in __Generated.Parsers.Inners._0Parser.Instance
select new __Generated.CstNodes.Inners._Ⰳx20(_2_1, _0_1);
    }
    
}
