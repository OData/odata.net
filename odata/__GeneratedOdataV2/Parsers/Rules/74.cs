namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _skipParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._skip> Instance { get; } = from _Ⲥʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺↃ_1 in __GeneratedOdataV2.Parsers.Inners._Ⲥʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺↃParser.Instance
from _EQ_1 in __GeneratedOdataV2.Parsers.Rules._EQParser.Instance
from _DIGIT_1 in __GeneratedOdataV2.Parsers.Rules._DIGITParser.Instance.Repeat(1, null)
select new __GeneratedOdataV2.CstNodes.Rules._skip(_Ⲥʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺↃ_1, _EQ_1, new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Rules._DIGIT>(_DIGIT_1));
    }
    
}
