namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _byteValueParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._byteValue> Instance { get; } = from _DIGIT_1 in __GeneratedOdataV2.Parsers.Rules._DIGITParser.Instance.Repeat(1, 3)
select new __GeneratedOdataV2.CstNodes.Rules._byteValue(new __GeneratedOdataV2.CstNodes.Inners.HelperRangedFrom1To3<__GeneratedOdataV2.CstNodes.Rules._DIGIT>(_DIGIT_1));
    }
    
}
