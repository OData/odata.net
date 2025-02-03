namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _headerParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._header> Instance { get; } = (_contentⲻidParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._header>(_entityidParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._header>(_isolationParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._header>(_odataⲻmaxversionParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._header>(_odataⲻversionParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._header>(_preferParser.Instance);
        
        public static class _contentⲻidParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._header._contentⲻid> Instance { get; } = from _contentⲻid_1 in __GeneratedOdataV3.Parsers.Rules._contentⲻidParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._header._contentⲻid(_contentⲻid_1);
        }
        
        public static class _entityidParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._header._entityid> Instance { get; } = from _entityid_1 in __GeneratedOdataV3.Parsers.Rules._entityidParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._header._entityid(_entityid_1);
        }
        
        public static class _isolationParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._header._isolation> Instance { get; } = from _isolation_1 in __GeneratedOdataV3.Parsers.Rules._isolationParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._header._isolation(_isolation_1);
        }
        
        public static class _odataⲻmaxversionParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._header._odataⲻmaxversion> Instance { get; } = from _odataⲻmaxversion_1 in __GeneratedOdataV3.Parsers.Rules._odataⲻmaxversionParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._header._odataⲻmaxversion(_odataⲻmaxversion_1);
        }
        
        public static class _odataⲻversionParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._header._odataⲻversion> Instance { get; } = from _odataⲻversion_1 in __GeneratedOdataV3.Parsers.Rules._odataⲻversionParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._header._odataⲻversion(_odataⲻversion_1);
        }
        
        public static class _preferParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._header._prefer> Instance { get; } = from _prefer_1 in __GeneratedOdataV3.Parsers.Rules._preferParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._header._prefer(_prefer_1);
        }
    }
    
}
