namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _secondParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._second> Instance { get; } = from _zeroToFiftyNine_1 in __GeneratedOdataV3.Parsers.Rules._zeroToFiftyNineParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._second(_zeroToFiftyNine_1);
    }
    
}
