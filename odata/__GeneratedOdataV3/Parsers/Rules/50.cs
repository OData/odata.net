namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _queryOptionParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._queryOption> Instance { get; } = (_systemQueryOptionParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._queryOption>(_aliasAndValueParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._queryOption>(_nameAndValueParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._queryOption>(_customQueryOptionParser.Instance);
        
        public static class _systemQueryOptionParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._queryOption._systemQueryOption> Instance { get; } = from _systemQueryOption_1 in __GeneratedOdataV3.Parsers.Rules._systemQueryOptionParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._queryOption._systemQueryOption(_systemQueryOption_1);
        }
        
        public static class _aliasAndValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._queryOption._aliasAndValue> Instance { get; } = from _aliasAndValue_1 in __GeneratedOdataV3.Parsers.Rules._aliasAndValueParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._queryOption._aliasAndValue(_aliasAndValue_1);
        }
        
        public static class _nameAndValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._queryOption._nameAndValue> Instance { get; } = from _nameAndValue_1 in __GeneratedOdataV3.Parsers.Rules._nameAndValueParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._queryOption._nameAndValue(_nameAndValue_1);
        }
        
        public static class _customQueryOptionParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._queryOption._customQueryOption> Instance { get; } = from _customQueryOption_1 in __GeneratedOdataV3.Parsers.Rules._customQueryOptionParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._queryOption._customQueryOption(_customQueryOption_1);
        }
    }
    
}
