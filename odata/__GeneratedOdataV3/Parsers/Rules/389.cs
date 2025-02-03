namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _portParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._port> Instance { get; } = from _DIGIT_1 in __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Many()
select new __GeneratedOdataV3.CstNodes.Rules._port(_DIGIT_1);
    }
    
}
