namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _alternationParser
    {
        public static Parser<__Generated.CstNodes.Rules._alternation> Instance { get; } = from _concatenation_1 in __GeneratedTest.Parsers.Rules._concatenationParser.Instance
from _ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ_1 in __GeneratedTest.Parsers.Inners._ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃParser.Instance.Many()
select new __Generated.CstNodes.Rules._alternation(_concatenation_1, _ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ_1);
    }
    
}
