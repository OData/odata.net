namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _CRLFParser
    {
        public static Parser<__GeneratedTest.CstNodes.Rules._CRLF> Instance { get; } = from _CR_1 in __GeneratedTest.Parsers.Rules._CRParser.Instance
from _LF_1 in __GeneratedTest.Parsers.Rules._LFParser.Instance
select new __GeneratedTest.CstNodes.Rules._CRLF(_CR_1, _LF_1);
    }
    
}
