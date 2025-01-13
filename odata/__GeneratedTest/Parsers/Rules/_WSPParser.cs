namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _WSPParser
    {
        public static Parser<__GeneratedTest.CstNodes.Rules._WSP> Instance { get; } = (_SPParser.Instance).Or<__GeneratedTest.CstNodes.Rules._WSP>(_HTABParser.Instance);
        
        public static class _SPParser
        {
            public static Parser<__GeneratedTest.CstNodes.Rules._WSP._SP> Instance { get; } = from _SP_1 in __GeneratedTest.Parsers.Rules._SPParser.Instance
select new __GeneratedTest.CstNodes.Rules._WSP._SP(_SP_1);
        }
        
        public static class _HTABParser
        {
            public static Parser<__GeneratedTest.CstNodes.Rules._WSP._HTAB> Instance { get; } = from _HTAB_1 in __GeneratedTest.Parsers.Rules._HTABParser.Instance
select new __GeneratedTest.CstNodes.Rules._WSP._HTAB(_HTAB_1);
        }
    }
    
}
