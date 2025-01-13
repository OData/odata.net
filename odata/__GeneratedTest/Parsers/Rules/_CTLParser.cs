namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _CTLParser
    {
        public static Parser<__GeneratedTest.CstNodes.Rules._CTL> Instance { get; } = (_Ⰳx00ⲻ1FParser.Instance).Or<__GeneratedTest.CstNodes.Rules._CTL>(_Ⰳx7FParser.Instance);
        
        public static class _Ⰳx00ⲻ1FParser
        {
            public static Parser<__GeneratedTest.CstNodes.Rules._CTL._Ⰳx00ⲻ1F> Instance { get; } = from _Ⰳx00ⲻ1F_1 in __GeneratedTest.Parsers.Inners._Ⰳx00ⲻ1FParser.Instance
select new __GeneratedTest.CstNodes.Rules._CTL._Ⰳx00ⲻ1F(_Ⰳx00ⲻ1F_1);
        }
        
        public static class _Ⰳx7FParser
        {
            public static Parser<__GeneratedTest.CstNodes.Rules._CTL._Ⰳx7F> Instance { get; } = from _Ⰳx7F_1 in __GeneratedTest.Parsers.Inners._Ⰳx7FParser.Instance
select new __GeneratedTest.CstNodes.Rules._CTL._Ⰳx7F(_Ⰳx7F_1);
        }
    }
    
}
