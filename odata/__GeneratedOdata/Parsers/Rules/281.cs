namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _singleValueParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._singleValue> Instance { get; } = from _decimalValue_1 in __GeneratedOdata.Parsers.Rules._decimalValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._singleValue(_decimalValue_1);
    }
    
}
