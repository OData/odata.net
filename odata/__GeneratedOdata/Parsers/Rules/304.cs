namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _fractionalSecondsParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._fractionalSeconds> Instance { get; } = from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._fractionalSeconds(_DIGIT_1);
    }
    
}
