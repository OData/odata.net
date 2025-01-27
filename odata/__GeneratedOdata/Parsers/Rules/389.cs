namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _portParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._port> Instance { get; } = from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._port(_DIGIT_1);
    }
    
}
