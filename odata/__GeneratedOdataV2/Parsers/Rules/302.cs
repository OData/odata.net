namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _minuteParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._minute> Instance { get; } = from _zeroToFiftyNine_1 in __GeneratedOdataV2.Parsers.Rules._zeroToFiftyNineParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._minute(_zeroToFiftyNine_1);
    }
    
}
