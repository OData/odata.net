namespace __Generated.Parsers.Inners
{
    using Sprache;
    
    public static class _Ⰳx22Parser
    {
        public static Parser<__Generated.CstNodes.Inners._Ⰳx22> Instance { get; } = from _2_1 in __Generated.Parsers.Inners._2Parser.Instance
from _2_2 in __Generated.Parsers.Inners._2Parser.Instance
select new __Generated.CstNodes.Inners._Ⰳx22(_2_1, _2_2);
    }
    
}
