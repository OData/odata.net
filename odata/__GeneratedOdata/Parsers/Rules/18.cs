namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitivePathParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._primitivePath> Instance { get; } = (_valueParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._primitivePath>(_boundOperationParser.Instance);
        
        public static class _valueParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._primitivePath._value> Instance { get; } = from _value_1 in __GeneratedOdata.Parsers.Rules._valueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitivePath._value(_value_1);
        }
        
        public static class _boundOperationParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._primitivePath._boundOperation> Instance { get; } = from _boundOperation_1 in __GeneratedOdata.Parsers.Rules._boundOperationParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitivePath._boundOperation(_boundOperation_1);
        }
    }
    
}
