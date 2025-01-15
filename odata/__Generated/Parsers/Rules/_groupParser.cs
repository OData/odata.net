namespace __Generated.Parsers.Rules
{
    using _GeneratorV5.ManualParsers.Rules;
    using Sprache;
    
    public static class _groupParser
    {
        public static Parser<__Generated.CstNodes.Rules._group> Instance { get; } = from _ʺx28ʺ_1 in __Generated.Parsers.Inners._ʺx28ʺParser.Instance
from _cⲻwsp_1 in __Generated.Parsers.Rules._cⲻwspParser.Instance.Many()
from _alternation_1 in __Generated.Parsers.Rules._alternationParser.Instance
from _cⲻwsp_2 in __Generated.Parsers.Rules._cⲻwspParser.Instance.Many()
from _ʺx29ʺ_1 in __Generated.Parsers.Inners._ʺx29ʺParser.Instance
select new __Generated.CstNodes.Rules._group(_ʺx28ʺ_1, _cⲻwsp_1.Convert(), _alternation_1, _cⲻwsp_2.Convert(), _ʺx29ʺ_1);
    }
    
}
