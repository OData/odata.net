namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _groupParser
    {
        public static Parser<__GeneratedTest.CstNodes.Rules._group> Instance { get; } = from _ʺx28ʺ_1 in __GeneratedTest.Parsers.Inners._ʺx28ʺParser.Instance
from _cⲻwsp_1 in __GeneratedTest.Parsers.Rules._cⲻwspParser.Instance.Many()
from _alternation_1 in __GeneratedTest.Parsers.Rules._alternationParser.Instance
from _cⲻwsp_2 in __GeneratedTest.Parsers.Rules._cⲻwspParser.Instance.Many()
from _ʺx29ʺ_1 in __GeneratedTest.Parsers.Inners._ʺx29ʺParser.Instance
select new __GeneratedTest.CstNodes.Rules._group(_ʺx28ʺ_1, _cⲻwsp_1, _alternation_1, _cⲻwsp_2, _ʺx29ʺ_1);
    }
    
}
