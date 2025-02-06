namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _expandOptionParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._expandOption> Instance { get; } = (_expandRefOptionParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._expandOption>(_selectParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._expandOption>(_expandParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._expandOption>(_computeParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._expandOption>(_levelsParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._expandOption>(_aliasAndValueParser.Instance);
        
        public static class _expandRefOptionParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._expandOption._expandRefOption> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._expandOption._expandRefOption>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._expandOption._expandRefOption> Parse(IInput<char>? input)
                {
                    var _expandRefOption_1 = __GeneratedOdataV4.Parsers.Rules._expandRefOptionParser.Instance.Parse(input);
if (!_expandRefOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._expandOption._expandRefOption)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._expandOption._expandRefOption(_expandRefOption_1.Parsed), _expandRefOption_1.Remainder);
                }
            }
        }
        
        public static class _selectParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._expandOption._select> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._expandOption._select>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._expandOption._select> Parse(IInput<char>? input)
                {
                    var _select_1 = __GeneratedOdataV4.Parsers.Rules._selectParser.Instance.Parse(input);
if (!_select_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._expandOption._select)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._expandOption._select(_select_1.Parsed), _select_1.Remainder);
                }
            }
        }
        
        public static class _expandParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._expandOption._expand> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._expandOption._expand>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._expandOption._expand> Parse(IInput<char>? input)
                {
                    var _expand_1 = __GeneratedOdataV4.Parsers.Rules._expandParser.Instance.Parse(input);
if (!_expand_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._expandOption._expand)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._expandOption._expand(_expand_1.Parsed), _expand_1.Remainder);
                }
            }
        }
        
        public static class _computeParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._expandOption._compute> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._expandOption._compute>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._expandOption._compute> Parse(IInput<char>? input)
                {
                    var _compute_1 = __GeneratedOdataV4.Parsers.Rules._computeParser.Instance.Parse(input);
if (!_compute_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._expandOption._compute)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._expandOption._compute(_compute_1.Parsed), _compute_1.Remainder);
                }
            }
        }
        
        public static class _levelsParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._expandOption._levels> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._expandOption._levels>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._expandOption._levels> Parse(IInput<char>? input)
                {
                    var _levels_1 = __GeneratedOdataV4.Parsers.Rules._levelsParser.Instance.Parse(input);
if (!_levels_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._expandOption._levels)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._expandOption._levels(_levels_1.Parsed), _levels_1.Remainder);
                }
            }
        }
        
        public static class _aliasAndValueParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._expandOption._aliasAndValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._expandOption._aliasAndValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._expandOption._aliasAndValue> Parse(IInput<char>? input)
                {
                    var _aliasAndValue_1 = __GeneratedOdataV4.Parsers.Rules._aliasAndValueParser.Instance.Parse(input);
if (!_aliasAndValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._expandOption._aliasAndValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._expandOption._aliasAndValue(_aliasAndValue_1.Parsed), _aliasAndValue_1.Remainder);
                }
            }
        }
    }
    
}
