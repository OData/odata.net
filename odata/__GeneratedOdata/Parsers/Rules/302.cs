namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _minuteParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._minute> Instance { get; } = from _zeroToFiftyNine_1 in __GeneratedOdata.Parsers.Rules._zeroToFiftyNineParser.Instance
select new __GeneratedOdata.CstNodes.Rules._minute(_zeroToFiftyNine_1);
    }
    
}
