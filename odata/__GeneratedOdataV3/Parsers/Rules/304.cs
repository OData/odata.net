namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _fractionalSecondsParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._fractionalSeconds> Instance { get; } = from _DIGIT_1 in __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Repeat(1, 12)
select new __GeneratedOdataV3.CstNodes.Rules._fractionalSeconds(new __GeneratedOdataV3.CstNodes.Inners.HelperRangedFrom1To12<__GeneratedOdataV3.CstNodes.Rules._DIGIT>(_DIGIT_1));
    }
    
}
