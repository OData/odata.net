namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ> Instance { get; } = (_DIGITParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ>(_ʺx41ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ>(_ʺx42ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ>(_ʺx44ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ>(_ʺx45ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ>(_ʺx46ʺParser.Instance);
        
        public static class _DIGITParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._DIGIT> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._DIGIT>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._DIGIT> Parse(IInput<char>? input)
                {
                    var _DIGIT_1 = __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Parse(input);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._DIGIT)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._DIGIT(_DIGIT_1.Parsed), _DIGIT_1.Remainder);
                }
            }
        }
        
        public static class _ʺx41ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx41ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx41ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx41ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx41ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx41ʺParser.Instance.Parse(input);
if (!_ʺx41ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx41ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx41ʺ.Instance, _ʺx41ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx42ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx42ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx42ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx42ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx42ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx42ʺParser.Instance.Parse(input);
if (!_ʺx42ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx42ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx42ʺ.Instance, _ʺx42ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx44ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx44ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx44ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx44ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx44ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx44ʺParser.Instance.Parse(input);
if (!_ʺx44ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx44ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx44ʺ.Instance, _ʺx44ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx45ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx45ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx45ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx45ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx45ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx45ʺParser.Instance.Parse(input);
if (!_ʺx45ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx45ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx45ʺ.Instance, _ʺx45ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx46ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx46ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx46ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx46ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx46ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx46ʺParser.Instance.Parse(input);
if (!_ʺx46ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx46ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx46ʺ.Instance, _ʺx46ʺ_1.Remainder);
                }
            }
        }
    }
    
}
