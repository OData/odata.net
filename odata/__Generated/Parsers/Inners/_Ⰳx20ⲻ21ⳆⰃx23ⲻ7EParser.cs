namespace __Generated.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⰳx20ⲻ21ⳆⰃx23ⲻ7EParser
    {
        public static IParser<char, __Generated.CstNodes.Inners._Ⰳx20ⲻ21ⳆⰃx23ⲻ7E> Instance { get; } = (_Ⰳx20ⲻ21Parser.Instance).Or<char, __Generated.CstNodes.Inners._Ⰳx20ⲻ21ⳆⰃx23ⲻ7E>(_Ⰳx23ⲻ7EParser.Instance);
        
        public static class _Ⰳx20ⲻ21Parser
        {
            public static IParser<char, __Generated.CstNodes.Inners._Ⰳx20ⲻ21ⳆⰃx23ⲻ7E._Ⰳx20ⲻ21> Instance { get; } = from _Ⰳx20ⲻ21_1 in __Generated.Parsers.Inners._Ⰳx20ⲻ21Parser.Instance
select new __Generated.CstNodes.Inners._Ⰳx20ⲻ21ⳆⰃx23ⲻ7E._Ⰳx20ⲻ21(_Ⰳx20ⲻ21_1);
        }
        
        public static class _Ⰳx23ⲻ7EParser
        {
            public static IParser<char, __Generated.CstNodes.Inners._Ⰳx20ⲻ21ⳆⰃx23ⲻ7E._Ⰳx23ⲻ7E> Instance { get; } = from _Ⰳx23ⲻ7E_1 in __Generated.Parsers.Inners._Ⰳx23ⲻ7EParser.Instance
select new __Generated.CstNodes.Inners._Ⰳx20ⲻ21ⳆⰃx23ⲻ7E._Ⰳx23ⲻ7E(_Ⰳx23ⲻ7E_1);
        }
    }
    
}
