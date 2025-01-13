namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _optionParser
    {
        public static Parser<__Generated.CstNodes.Rules._option> Instance { get; } = from _ʺx5Bʺ_1 in __GeneratedTest.Parsers.Inners._ʺx5BʺParser.Instance
from _cⲻwsp_1 in __GeneratedTest.Parsers.Rules._cⲻwspParser.Instance.Many()
from _alternation_1 in __GeneratedTest.Parsers.Rules._alternationParser.Instance
from _cⲻwsp_2 in __GeneratedTest.Parsers.Rules._cⲻwspParser.Instance.Many()
from _ʺx5Dʺ_1 in __GeneratedTest.Parsers.Inners._ʺx5DʺParser.Instance
select new __Generated.CstNodes.Rules._option(_ʺx5Bʺ_1, _cⲻwsp_1, _alternation_1, _cⲻwsp_2, _ʺx5Dʺ_1);
    }
    
}
