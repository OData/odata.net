namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _headerParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._header> Instance { get; } = (_contentⲻidParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._header>(_entityidParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._header>(_isolationParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._header>(_odataⲻmaxversionParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._header>(_odataⲻversionParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._header>(_preferParser.Instance);
        
        public static class _contentⲻidParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._header._contentⲻid> Instance { get; } = from _contentⲻid_1 in __GeneratedOdata.Parsers.Rules._contentⲻidParser.Instance
select new __GeneratedOdata.CstNodes.Rules._header._contentⲻid(_contentⲻid_1);
        }
        
        public static class _entityidParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._header._entityid> Instance { get; } = from _entityid_1 in __GeneratedOdata.Parsers.Rules._entityidParser.Instance
select new __GeneratedOdata.CstNodes.Rules._header._entityid(_entityid_1);
        }
        
        public static class _isolationParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._header._isolation> Instance { get; } = from _isolation_1 in __GeneratedOdata.Parsers.Rules._isolationParser.Instance
select new __GeneratedOdata.CstNodes.Rules._header._isolation(_isolation_1);
        }
        
        public static class _odataⲻmaxversionParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._header._odataⲻmaxversion> Instance { get; } = from _odataⲻmaxversion_1 in __GeneratedOdata.Parsers.Rules._odataⲻmaxversionParser.Instance
select new __GeneratedOdata.CstNodes.Rules._header._odataⲻmaxversion(_odataⲻmaxversion_1);
        }
        
        public static class _odataⲻversionParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._header._odataⲻversion> Instance { get; } = from _odataⲻversion_1 in __GeneratedOdata.Parsers.Rules._odataⲻversionParser.Instance
select new __GeneratedOdata.CstNodes.Rules._header._odataⲻversion(_odataⲻversion_1);
        }
        
        public static class _preferParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._header._prefer> Instance { get; } = from _prefer_1 in __GeneratedOdata.Parsers.Rules._preferParser.Instance
select new __GeneratedOdata.CstNodes.Rules._header._prefer(_prefer_1);
        }
    }
    
}