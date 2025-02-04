namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _selectOptionParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOption> Instance { get; } = (_selectOptionPCParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectOption>(_computeParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectOption>(_selectParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectOption>(_expandParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectOption>(_aliasAndValueParser.Instance);
        
        public static class _selectOptionPCParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOption._selectOptionPC> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOption._selectOptionPC>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._selectOption._selectOptionPC> Parse(IInput<char>? input)
                {
                    var _selectOptionPC_1 = __GeneratedOdataV3.Parsers.Rules._selectOptionPCParser.Instance.Parse(input);
if (!_selectOptionPC_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectOption._selectOptionPC)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._selectOption._selectOptionPC(_selectOptionPC_1.Parsed), _selectOptionPC_1.Remainder);
                }
            }
        }
        
        public static class _computeParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOption._compute> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOption._compute>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._selectOption._compute> Parse(IInput<char>? input)
                {
                    var _compute_1 = __GeneratedOdataV3.Parsers.Rules._computeParser.Instance.Parse(input);
if (!_compute_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectOption._compute)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._selectOption._compute(_compute_1.Parsed), _compute_1.Remainder);
                }
            }
        }
        
        public static class _selectParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOption._select> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOption._select>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._selectOption._select> Parse(IInput<char>? input)
                {
                    var _select_1 = __GeneratedOdataV3.Parsers.Rules._selectParser.Instance.Parse(input);
if (!_select_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectOption._select)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._selectOption._select(_select_1.Parsed), _select_1.Remainder);
                }
            }
        }
        
        public static class _expandParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOption._expand> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOption._expand>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._selectOption._expand> Parse(IInput<char>? input)
                {
                    var _expand_1 = __GeneratedOdataV3.Parsers.Rules._expandParser.Instance.Parse(input);
if (!_expand_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectOption._expand)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._selectOption._expand(_expand_1.Parsed), _expand_1.Remainder);
                }
            }
        }
        
        public static class _aliasAndValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOption._aliasAndValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOption._aliasAndValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._selectOption._aliasAndValue> Parse(IInput<char>? input)
                {
                    var _aliasAndValue_1 = __GeneratedOdataV3.Parsers.Rules._aliasAndValueParser.Instance.Parse(input);
if (!_aliasAndValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectOption._aliasAndValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._selectOption._aliasAndValue(_aliasAndValue_1.Parsed), _aliasAndValue_1.Remainder);
                }
            }
        }
    }
    
}
