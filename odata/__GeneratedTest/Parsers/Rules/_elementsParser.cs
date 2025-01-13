namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _elementsParser
    {
        public static Parser<__GeneratedTest.CstNodes.Rules._elements> Instance { get; } = from _alternation_1 in __GeneratedTest.Parsers.Rules._alternationParser.Instance
from _cⲻwsp_1 in __GeneratedTest.Parsers.Rules._cⲻwspParser.Instance.Many()
select new __GeneratedTest.CstNodes.Rules._elements(_alternation_1, _cⲻwsp_1);
    }
    
}
