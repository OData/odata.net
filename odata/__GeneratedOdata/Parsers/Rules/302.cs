namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _minuteParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._minute> Instance { get; } = from _zeroToFiftyNine_1 in __GeneratedOdata.Parsers.Rules._zeroToFiftyNineParser.Instance
select new __GeneratedOdata.CstNodes.Rules._minute(_zeroToFiftyNine_1);
    }
    
}
