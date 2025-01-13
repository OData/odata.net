namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _byteValueParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._byteValue> Instance { get; } = from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._byteValue(_DIGIT_1);
    }
    
}
