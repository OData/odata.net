namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _skipParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._skip> Instance { get; } = from _Ⲥʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺↃParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._skip(_Ⲥʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺↃ_1, _EQ_1, _DIGIT_1);
    }
    
}
