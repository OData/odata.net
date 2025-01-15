namespace __Generated.Parsers.Rules
{
    using _GeneratorV5.ManualParsers.Rules;
    using Sprache;
    
    public static class _concatenationParser
    {
        public static Parser<__Generated.CstNodes.Rules._concatenation> Instance { get; } = from _repetition_1 in __Generated.Parsers.Rules._repetitionParser.Instance
from _Ⲥ1Жcⲻwsp_repetitionↃ_1 in __Generated.Parsers.Inners._Ⲥ1Жcⲻwsp_repetitionↃParser.Instance.Many()
select new __Generated.CstNodes.Rules._concatenation(_repetition_1, _Ⲥ1Жcⲻwsp_repetitionↃ_1.Convert());
    }
    
}
