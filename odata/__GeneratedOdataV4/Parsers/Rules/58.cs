namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _entityCastOptionParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._entityCastOption> Instance { get; } = (_entityIdOptionParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._entityCastOption>(_expandParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._entityCastOption>(_selectParser.Instance);
        
        public static class _entityIdOptionParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._entityCastOption._entityIdOption> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._entityCastOption._entityIdOption>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._entityCastOption._entityIdOption> Parse(IInput<char>? input)
                {
                    var _entityIdOption_1 = __GeneratedOdataV4.Parsers.Rules._entityIdOptionParser.Instance.Parse(input);
if (!_entityIdOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._entityCastOption._entityIdOption)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._entityCastOption._entityIdOption(_entityIdOption_1.Parsed), _entityIdOption_1.Remainder);
                }
            }
        }
        
        public static class _expandParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._entityCastOption._expand> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._entityCastOption._expand>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._entityCastOption._expand> Parse(IInput<char>? input)
                {
                    var _expand_1 = __GeneratedOdataV4.Parsers.Rules._expandParser.Instance.Parse(input);
if (!_expand_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._entityCastOption._expand)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._entityCastOption._expand(_expand_1.Parsed), _expand_1.Remainder);
                }
            }
        }
        
        public static class _selectParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._entityCastOption._select> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._entityCastOption._select>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._entityCastOption._select> Parse(IInput<char>? input)
                {
                    var _select_1 = __GeneratedOdataV4.Parsers.Rules._selectParser.Instance.Parse(input);
if (!_select_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._entityCastOption._select)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._entityCastOption._select(_select_1.Parsed), _select_1.Remainder);
                }
            }
        }
    }
    
}
