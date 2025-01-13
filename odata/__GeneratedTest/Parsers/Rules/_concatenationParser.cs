namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _concatenationParser
    {
        public static Parser<__GeneratedTest.CstNodes.Rules._concatenation> Instance { get; } = from _repetition_1 in __GeneratedTest.Parsers.Rules._repetitionParser.Instance
from _Ⲥ1Жcⲻwsp_repetitionↃ_1 in __GeneratedTest.Parsers.Inners._Ⲥ1Жcⲻwsp_repetitionↃParser.Instance.Many()
select new __GeneratedTest.CstNodes.Rules._concatenation(_repetition_1, _Ⲥ1Жcⲻwsp_repetitionↃ_1);
    }
    
}
