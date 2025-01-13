namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _ALPHAParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._ALPHA> Instance { get; } = (_Ⰳx41ⲻ5AParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._ALPHA>(_Ⰳx61ⲻ7AParser.Instance);
        
        public static class _Ⰳx41ⲻ5AParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._ALPHA._Ⰳx41ⲻ5A> Instance { get; } = from _Ⰳx41ⲻ5A_1 in __GeneratedOdata.Parsers.Inners._Ⰳx41ⲻ5AParser.Instance
select new __GeneratedOdata.CstNodes.Rules._ALPHA._Ⰳx41ⲻ5A(_Ⰳx41ⲻ5A_1);
        }
        
        public static class _Ⰳx61ⲻ7AParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A> Instance { get; } = from _Ⰳx61ⲻ7A_1 in __GeneratedOdata.Parsers.Inners._Ⰳx61ⲻ7AParser.Instance
select new __GeneratedOdata.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(_Ⰳx61ⲻ7A_1);
        }
    }
    
}
