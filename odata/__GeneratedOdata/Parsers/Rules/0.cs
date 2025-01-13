namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _dummyStartRuleParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._dummyStartRule> Instance { get; } = (_odataUriParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._dummyStartRule>(_headerParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._dummyStartRule>(_primitiveValueParser.Instance);
        
        public static class _odataUriParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._dummyStartRule._odataUri> Instance { get; } = from _odataUri_1 in __GeneratedOdata.Parsers.Rules._odataUriParser.Instance
select new __GeneratedOdata.CstNodes.Rules._dummyStartRule._odataUri(_odataUri_1);
        }
        
        public static class _headerParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._dummyStartRule._header> Instance { get; } = from _header_1 in __GeneratedOdata.Parsers.Rules._headerParser.Instance
select new __GeneratedOdata.CstNodes.Rules._dummyStartRule._header(_header_1);
        }
        
        public static class _primitiveValueParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._dummyStartRule._primitiveValue> Instance { get; } = from _primitiveValue_1 in __GeneratedOdata.Parsers.Rules._primitiveValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._dummyStartRule._primitiveValue(_primitiveValue_1);
        }
    }
    
}
