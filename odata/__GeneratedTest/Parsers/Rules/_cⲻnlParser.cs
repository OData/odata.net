namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _cⲻnlParser
    {
        public static Parser<__GeneratedTest.CstNodes.Rules._cⲻnl> Instance { get; } = (_commentParser.Instance).Or<__GeneratedTest.CstNodes.Rules._cⲻnl>(_CRLFParser.Instance);
        
        public static class _commentParser
        {
            public static Parser<__GeneratedTest.CstNodes.Rules._cⲻnl._comment> Instance { get; } = from _comment_1 in __GeneratedTest.Parsers.Rules._commentParser.Instance
select new __GeneratedTest.CstNodes.Rules._cⲻnl._comment(_comment_1);
        }
        
        public static class _CRLFParser
        {
            public static Parser<__GeneratedTest.CstNodes.Rules._cⲻnl._CRLF> Instance { get; } = from _CRLF_1 in __GeneratedTest.Parsers.Rules._CRLFParser.Instance
select new __GeneratedTest.CstNodes.Rules._cⲻnl._CRLF(_CRLF_1);
        }
    }
    
}
