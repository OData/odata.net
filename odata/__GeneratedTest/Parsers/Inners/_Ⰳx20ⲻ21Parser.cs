namespace __GeneratedTest.Parsers.Inners
{
    using Sprache;
    
    public static class _Ⰳx20ⲻ21Parser
    {
        public static Parser<__GeneratedTest.CstNodes.Inners._Ⰳx20ⲻ21> Instance { get; } = (_20Parser.Instance).Or<__GeneratedTest.CstNodes.Inners._Ⰳx20ⲻ21>(_21Parser.Instance);
        
        public static class _20Parser
        {
            public static Parser<__GeneratedTest.CstNodes.Inners._Ⰳx20ⲻ21._20> Instance { get; } = from _20 in Parse.Char((char)0x20) select new __GeneratedTest.CstNodes.Inners._Ⰳx20ⲻ21._20(__GeneratedTest.CstNodes.Inners._2.Instance, __GeneratedTest.CstNodes.Inners._0.Instance);
        }
        
        public static class _21Parser
        {
            public static Parser<__GeneratedTest.CstNodes.Inners._Ⰳx20ⲻ21._21> Instance { get; } = from _21 in Parse.Char((char)0x21) select new __GeneratedTest.CstNodes.Inners._Ⰳx20ⲻ21._21(__GeneratedTest.CstNodes.Inners._2.Instance, __GeneratedTest.CstNodes.Inners._1.Instance);
        }
    }
    
}
