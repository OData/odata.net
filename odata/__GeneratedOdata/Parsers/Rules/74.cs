namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _skipParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._skip> Instance { get; } = from _Ⲥʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺↃParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Repeat(1, null)
select new __GeneratedOdata.CstNodes.Rules._skip(_Ⲥʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺↃ_1, _EQ_1, new __GeneratedOdata.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdata.CstNodes.Rules._DIGIT>(_DIGIT_1));
    }
    
}
