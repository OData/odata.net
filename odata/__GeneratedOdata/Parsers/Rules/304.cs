namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _fractionalSecondsParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._fractionalSeconds> Instance { get; } = from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Repeat(1, 12)
select new __GeneratedOdata.CstNodes.Rules._fractionalSeconds(new __GeneratedOdata.CstNodes.Inners.HelperRangedFrom1To12<__GeneratedOdata.CstNodes.Rules._DIGIT>(_DIGIT_1));
    }
    
}
