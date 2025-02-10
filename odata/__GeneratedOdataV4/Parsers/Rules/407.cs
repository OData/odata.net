namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _unreservedParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._unreserved> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._unreserved>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public __GeneratedOdataV4.CstNodes.Rules._unreserved Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                _ALPHAParser.Instance2.Parse(input, start, out newStart);

                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._unreserved> Instance { get; } = (_ALPHAParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._unreserved>(_DIGITParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._unreserved>(_ʺx2DʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._unreserved>(_ʺx2EʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._unreserved>(_ʺx5FʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._unreserved>(_ʺx7EʺParser.Instance);
        
        public static class _ALPHAParser
        {
            public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA> Instance2 { get; } = new Parser2();

            private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA>
            {
                [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
                public __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
                {
                    var _ALPHA_1 = __GeneratedOdataV4.Parsers.Rules._ALPHAParser.Instance2.Parse(input, start, out newStart);

                    return default;
                }
            }

            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA> Parse(IInput<char>? input)
                {
                    var _ALPHA_1 = __GeneratedOdataV4.Parsers.Rules._ALPHAParser.Instance.Parse(input);
if (!_ALPHA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA(_ALPHA_1.Parsed), _ALPHA_1.Remainder);
                }
            }
        }
        
        public static class _DIGITParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._unreserved._DIGIT> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._unreserved._DIGIT>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._unreserved._DIGIT> Parse(IInput<char>? input)
                {
                    var _DIGIT_1 = __GeneratedOdataV4.Parsers.Rules._DIGITParser.Instance.Parse(input);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._unreserved._DIGIT)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._unreserved._DIGIT(_DIGIT_1.Parsed), _DIGIT_1.Remainder);
                }
            }
        }
        
        public static class _ʺx2DʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._unreserved._ʺx2Dʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._unreserved._ʺx2Dʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._unreserved._ʺx2Dʺ> Parse(IInput<char>? input)
                {
                    var _ʺx2Dʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2DʺParser.Instance.Parse(input);
if (!_ʺx2Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._unreserved._ʺx2Dʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._unreserved._ʺx2Dʺ.Instance, _ʺx2Dʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx2EʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._unreserved._ʺx2Eʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._unreserved._ʺx2Eʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._unreserved._ʺx2Eʺ> Parse(IInput<char>? input)
                {
                    var _ʺx2Eʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2EʺParser.Instance.Parse(input);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._unreserved._ʺx2Eʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._unreserved._ʺx2Eʺ.Instance, _ʺx2Eʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx5FʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._unreserved._ʺx5Fʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._unreserved._ʺx5Fʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._unreserved._ʺx5Fʺ> Parse(IInput<char>? input)
                {
                    var _ʺx5Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx5FʺParser.Instance.Parse(input);
if (!_ʺx5Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._unreserved._ʺx5Fʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._unreserved._ʺx5Fʺ.Instance, _ʺx5Fʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx7EʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._unreserved._ʺx7Eʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._unreserved._ʺx7Eʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._unreserved._ʺx7Eʺ> Parse(IInput<char>? input)
                {
                    var _ʺx7Eʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx7EʺParser.Instance.Parse(input);
if (!_ʺx7Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._unreserved._ʺx7Eʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._unreserved._ʺx7Eʺ.Instance, _ʺx7Eʺ_1.Remainder);
                }
            }
        }
    }
    
}
