namespace __GeneratedTest.Parsers.Inners
{
    using Sprache;
    
    public static class _WSPⳆCRLF_WSPParser
    {
        public static Parser<__GeneratedTest.CstNodes.Inners._WSPⳆCRLF_WSP> Instance { get; } = (_WSPParser.Instance).Or<__GeneratedTest.CstNodes.Inners._WSPⳆCRLF_WSP>(_CRLF_WSPParser.Instance);
        
        public static class _WSPParser
        {
            public static Parser<__GeneratedTest.CstNodes.Inners._WSPⳆCRLF_WSP._WSP> Instance { get; } = from _WSP_1 in __GeneratedTest.Parsers.Rules._WSPParser.Instance
select new __GeneratedTest.CstNodes.Inners._WSPⳆCRLF_WSP._WSP(_WSP_1);
        }
        
        public static class _CRLF_WSPParser
        {
            public static Parser<__GeneratedTest.CstNodes.Inners._WSPⳆCRLF_WSP._CRLF_WSP> Instance { get; } = from _CRLF_1 in __GeneratedTest.Parsers.Rules._CRLFParser.Instance
from _WSP_1 in __GeneratedTest.Parsers.Rules._WSPParser.Instance
select new __GeneratedTest.CstNodes.Inners._WSPⳆCRLF_WSP._CRLF_WSP(_CRLF_1, _WSP_1);
        }
    }
    
}
