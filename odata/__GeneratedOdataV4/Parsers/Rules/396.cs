namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _decⲻoctetParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._decⲻoctet> Instance { get; } = (_ʺx31ʺ_2DIGITParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._decⲻoctet>(_ʺx32ʺ_Ⰳx30ⲻ34_DIGITParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._decⲻoctet>(_ʺx32x35ʺ_Ⰳx30ⲻ35Parser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._decⲻoctet>(_Ⰳx31ⲻ39_DIGITParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._decⲻoctet>(_DIGITParser.Instance);
        
        public static class _ʺx31ʺ_2DIGITParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx31ʺ_2DIGIT> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx31ʺ_2DIGIT>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx31ʺ_2DIGIT> Parse(IInput<char>? input)
                {
                    var _ʺx31ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx31ʺParser.Instance.Parse(input);
if (!_ʺx31ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx31ʺ_2DIGIT)!, input);
}

var _DIGIT_1 = __GeneratedOdataV4.Parsers.Rules._DIGITParser.Instance.Repeat(2, 2).Parse(_ʺx31ʺ_1.Remainder);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx31ʺ_2DIGIT)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx31ʺ_2DIGIT(_ʺx31ʺ_1.Parsed, new __GeneratedOdataV4.CstNodes.Inners.HelperRangedExactly2<__GeneratedOdataV4.CstNodes.Rules._DIGIT>(_DIGIT_1.Parsed)), _DIGIT_1.Remainder);
                }
            }
        }
        
        public static class _ʺx32ʺ_Ⰳx30ⲻ34_DIGITParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx32ʺ_Ⰳx30ⲻ34_DIGIT> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx32ʺ_Ⰳx30ⲻ34_DIGIT>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx32ʺ_Ⰳx30ⲻ34_DIGIT> Parse(IInput<char>? input)
                {
                    var _ʺx32ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx32ʺParser.Instance.Parse(input);
if (!_ʺx32ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx32ʺ_Ⰳx30ⲻ34_DIGIT)!, input);
}

var _Ⰳx30ⲻ34_1 = __GeneratedOdataV4.Parsers.Inners._Ⰳx30ⲻ34Parser.Instance.Parse(_ʺx32ʺ_1.Remainder);
if (!_Ⰳx30ⲻ34_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx32ʺ_Ⰳx30ⲻ34_DIGIT)!, input);
}

var _DIGIT_1 = __GeneratedOdataV4.Parsers.Rules._DIGITParser.Instance.Parse(_Ⰳx30ⲻ34_1.Remainder);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx32ʺ_Ⰳx30ⲻ34_DIGIT)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx32ʺ_Ⰳx30ⲻ34_DIGIT(_ʺx32ʺ_1.Parsed, _Ⰳx30ⲻ34_1.Parsed, _DIGIT_1.Parsed), _DIGIT_1.Remainder);
                }
            }
        }
        
        public static class _ʺx32x35ʺ_Ⰳx30ⲻ35Parser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx32x35ʺ_Ⰳx30ⲻ35> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx32x35ʺ_Ⰳx30ⲻ35>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx32x35ʺ_Ⰳx30ⲻ35> Parse(IInput<char>? input)
                {
                    var _ʺx32x35ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx32x35ʺParser.Instance.Parse(input);
if (!_ʺx32x35ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx32x35ʺ_Ⰳx30ⲻ35)!, input);
}

var _Ⰳx30ⲻ35_1 = __GeneratedOdataV4.Parsers.Inners._Ⰳx30ⲻ35Parser.Instance.Parse(_ʺx32x35ʺ_1.Remainder);
if (!_Ⰳx30ⲻ35_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx32x35ʺ_Ⰳx30ⲻ35)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx32x35ʺ_Ⰳx30ⲻ35(_ʺx32x35ʺ_1.Parsed, _Ⰳx30ⲻ35_1.Parsed), _Ⰳx30ⲻ35_1.Remainder);
                }
            }
        }
        
        public static class _Ⰳx31ⲻ39_DIGITParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._decⲻoctet._Ⰳx31ⲻ39_DIGIT> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._decⲻoctet._Ⰳx31ⲻ39_DIGIT>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._decⲻoctet._Ⰳx31ⲻ39_DIGIT> Parse(IInput<char>? input)
                {
                    var _Ⰳx31ⲻ39_1 = __GeneratedOdataV4.Parsers.Inners._Ⰳx31ⲻ39Parser.Instance.Parse(input);
if (!_Ⰳx31ⲻ39_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._decⲻoctet._Ⰳx31ⲻ39_DIGIT)!, input);
}

var _DIGIT_1 = __GeneratedOdataV4.Parsers.Rules._DIGITParser.Instance.Parse(_Ⰳx31ⲻ39_1.Remainder);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._decⲻoctet._Ⰳx31ⲻ39_DIGIT)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._decⲻoctet._Ⰳx31ⲻ39_DIGIT(_Ⰳx31ⲻ39_1.Parsed, _DIGIT_1.Parsed), _DIGIT_1.Remainder);
                }
            }
        }
        
        public static class _DIGITParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._decⲻoctet._DIGIT> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._decⲻoctet._DIGIT>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._decⲻoctet._DIGIT> Parse(IInput<char>? input)
                {
                    var _DIGIT_1 = __GeneratedOdataV4.Parsers.Rules._DIGITParser.Instance.Parse(input);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._decⲻoctet._DIGIT)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._decⲻoctet._DIGIT(_DIGIT_1.Parsed), _DIGIT_1.Remainder);
                }
            }
        }
    }
    
}
