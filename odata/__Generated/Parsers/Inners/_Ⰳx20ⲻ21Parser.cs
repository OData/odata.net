namespace __Generated.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⰳx20ⲻ21Parser
    {
        public static IParser<char, __Generated.CstNodes.Inners._Ⰳx20ⲻ21> Instance { get; } = (_20Parser.Instance).Or<char, __Generated.CstNodes.Inners._Ⰳx20ⲻ21>(_21Parser.Instance);
        
        public static class _20Parser
        {
            public static IParser<char, __Generated.CstNodes.Inners._Ⰳx20ⲻ21._20> Instance { get; } = from _20 in Parse.Char((char)0x20) select new __Generated.CstNodes.Inners._Ⰳx20ⲻ21._20(__Generated.CstNodes.Inners._2.Instance, __Generated.CstNodes.Inners._0.Instance);
        }
        
        public static class _21Parser
        {
            public static IParser<char, __Generated.CstNodes.Inners._Ⰳx20ⲻ21._21> Instance { get; } = from _21 in Parse.Char((char)0x21) select new __Generated.CstNodes.Inners._Ⰳx20ⲻ21._21(__Generated.CstNodes.Inners._2.Instance, __Generated.CstNodes.Inners._1.Instance);
        }
    }
    
}
