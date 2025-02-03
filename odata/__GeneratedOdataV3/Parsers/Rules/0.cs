namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _dummyStartRuleParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._dummyStartRule> Instance { get; } = (_odataUriParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._dummyStartRule>(_headerParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._dummyStartRule>(_primitiveValueParser.Instance);
        
        public static class _odataUriParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._dummyStartRule._odataUri> Instance { get; } = from _odataUri_1 in __GeneratedOdataV3.Parsers.Rules._odataUriParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._dummyStartRule._odataUri(_odataUri_1);
        }
        
        public static class _headerParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._dummyStartRule._header> Instance { get; } = from _header_1 in __GeneratedOdataV3.Parsers.Rules._headerParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._dummyStartRule._header(_header_1);
        }
        
        public static class _primitiveValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._dummyStartRule._primitiveValue> Instance { get; } = from _primitiveValue_1 in __GeneratedOdataV3.Parsers.Rules._primitiveValueParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._dummyStartRule._primitiveValue(_primitiveValue_1);
        }
    }
    
}
