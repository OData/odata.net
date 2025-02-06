namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _headerParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._header> Instance { get; } = (_contentⲻidParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._header>(_entityidParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._header>(_isolationParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._header>(_odataⲻmaxversionParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._header>(_odataⲻversionParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._header>(_preferParser.Instance);
        
        public static class _contentⲻidParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._header._contentⲻid> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._header._contentⲻid>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._header._contentⲻid> Parse(IInput<char>? input)
                {
                    var _contentⲻid_1 = __GeneratedOdataV4.Parsers.Rules._contentⲻidParser.Instance.Parse(input);
if (!_contentⲻid_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._header._contentⲻid)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._header._contentⲻid(_contentⲻid_1.Parsed), _contentⲻid_1.Remainder);
                }
            }
        }
        
        public static class _entityidParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._header._entityid> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._header._entityid>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._header._entityid> Parse(IInput<char>? input)
                {
                    var _entityid_1 = __GeneratedOdataV4.Parsers.Rules._entityidParser.Instance.Parse(input);
if (!_entityid_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._header._entityid)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._header._entityid(_entityid_1.Parsed), _entityid_1.Remainder);
                }
            }
        }
        
        public static class _isolationParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._header._isolation> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._header._isolation>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._header._isolation> Parse(IInput<char>? input)
                {
                    var _isolation_1 = __GeneratedOdataV4.Parsers.Rules._isolationParser.Instance.Parse(input);
if (!_isolation_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._header._isolation)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._header._isolation(_isolation_1.Parsed), _isolation_1.Remainder);
                }
            }
        }
        
        public static class _odataⲻmaxversionParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._header._odataⲻmaxversion> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._header._odataⲻmaxversion>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._header._odataⲻmaxversion> Parse(IInput<char>? input)
                {
                    var _odataⲻmaxversion_1 = __GeneratedOdataV4.Parsers.Rules._odataⲻmaxversionParser.Instance.Parse(input);
if (!_odataⲻmaxversion_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._header._odataⲻmaxversion)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._header._odataⲻmaxversion(_odataⲻmaxversion_1.Parsed), _odataⲻmaxversion_1.Remainder);
                }
            }
        }
        
        public static class _odataⲻversionParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._header._odataⲻversion> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._header._odataⲻversion>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._header._odataⲻversion> Parse(IInput<char>? input)
                {
                    var _odataⲻversion_1 = __GeneratedOdataV4.Parsers.Rules._odataⲻversionParser.Instance.Parse(input);
if (!_odataⲻversion_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._header._odataⲻversion)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._header._odataⲻversion(_odataⲻversion_1.Parsed), _odataⲻversion_1.Remainder);
                }
            }
        }
        
        public static class _preferParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._header._prefer> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._header._prefer>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._header._prefer> Parse(IInput<char>? input)
                {
                    var _prefer_1 = __GeneratedOdataV4.Parsers.Rules._preferParser.Instance.Parse(input);
if (!_prefer_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._header._prefer)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._header._prefer(_prefer_1.Parsed), _prefer_1.Remainder);
                }
            }
        }
    }
    
}
