namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _indexParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._index> Instance { get; } = from _Ⲥʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺↃParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Repeat(1, null)
select new __GeneratedOdata.CstNodes.Rules._index(_Ⲥʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺↃ_1, _EQ_1, new __GeneratedOdata.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdata.CstNodes.Rules._DIGIT>(_DIGIT_1));
    }
    
}
