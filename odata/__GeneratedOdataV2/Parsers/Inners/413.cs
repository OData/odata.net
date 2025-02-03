namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _oneToNine_ЖDIGITParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._oneToNine_ЖDIGIT> Instance { get; } = from _oneToNine_1 in __GeneratedOdataV2.Parsers.Rules._oneToNineParser.Instance
from _DIGIT_1 in __GeneratedOdataV2.Parsers.Rules._DIGITParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Inners._oneToNine_ЖDIGIT(_oneToNine_1, _DIGIT_1);
    }
    
}
