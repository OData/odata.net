namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _byteValueParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._byteValue> Instance { get; } = from _DIGIT_1 in __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Repeat(1, 3)
select new __GeneratedOdataV3.CstNodes.Rules._byteValue(new __GeneratedOdataV3.CstNodes.Inners.HelperRangedFrom1To3<__GeneratedOdataV3.CstNodes.Rules._DIGIT>(_DIGIT_1));
    }
    
}
