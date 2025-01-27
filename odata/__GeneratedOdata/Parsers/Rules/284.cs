namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _byteValueParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._byteValue> Instance { get; } = from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Repeat(1, 3)
select new __GeneratedOdata.CstNodes.Rules._byteValue(new __GeneratedOdata.CstNodes.Inners.HelperRangedFrom1To3<__GeneratedOdata.CstNodes.Rules._DIGIT>(_DIGIT_1));
    }
    
}
