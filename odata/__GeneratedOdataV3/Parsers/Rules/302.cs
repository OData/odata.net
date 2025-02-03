namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _minuteParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._minute> Instance { get; } = from _zeroToFiftyNine_1 in __GeneratedOdataV3.Parsers.Rules._zeroToFiftyNineParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._minute(_zeroToFiftyNine_1);
    }
    
}
