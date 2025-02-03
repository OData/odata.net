namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _indexParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._index> Instance { get; } = from _Ⲥʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺↃ_1 in __GeneratedOdataV2.Parsers.Inners._Ⲥʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺↃParser.Instance
from _EQ_1 in __GeneratedOdataV2.Parsers.Rules._EQParser.Instance
from _DIGIT_1 in __GeneratedOdataV2.Parsers.Rules._DIGITParser.Instance.Repeat(1, null)
select new __GeneratedOdataV2.CstNodes.Rules._index(_Ⲥʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺↃ_1, _EQ_1, new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Rules._DIGIT>(_DIGIT_1));
    }
    
}
