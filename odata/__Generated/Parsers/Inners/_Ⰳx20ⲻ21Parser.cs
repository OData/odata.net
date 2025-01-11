namespace __Generated.Parsers.Inners
{
    using Sprache;
    
    public static class _Ⰳx20ⲻ21Parser
    {
        public static Parser<__Generated.CstNodes.Inners._Ⰳx20ⲻ21> Instance { get; } = (_20Parser.Instance).Or<__Generated.CstNodes.Inners._Ⰳx20ⲻ21>(_21Parser.Instance);
        
        public static class _20Parser
        {
            public static Parser<__Generated.CstNodes.Inners._Ⰳx20ⲻ21._20> Instance { get; } = from _2_1 in __Generated.Parsers.Inners._2Parser.Instance
from _0_1 in __Generated.Parsers.Inners._0Parser.Instance
select new __Generated.CstNodes.Inners._Ⰳx20ⲻ21._20(_2_1, _0_1);
        }
        
        public static class _21Parser
        {
            public static Parser<__Generated.CstNodes.Inners._Ⰳx20ⲻ21._21> Instance { get; } = from _2_1 in __Generated.Parsers.Inners._2Parser.Instance
from _1_1 in __Generated.Parsers.Inners._1Parser.Instance
select new __Generated.CstNodes.Inners._Ⰳx20ⲻ21._21(_2_1, _1_1);
        }
    }
    
}
