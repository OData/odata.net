namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _identifierCharacterParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._identifierCharacter> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._identifierCharacter>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public __GeneratedOdataV4.CstNodes.Rules._identifierCharacter Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                var _ALPHA_1 = _ALPHAParser.Instance2.Parse(input, start, out newStart);

                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._identifierCharacter> Instance { get; } = (_ALPHAParser.Instance);
        
        public static class _ALPHAParser
        {
            public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA> Instance2 { get; } = new Parser2();

            private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA>
            {
                [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
                public __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
                {
                    var _ALPHA_1 = __GeneratedOdataV4.Parsers.Rules._ALPHAParser.Instance2.Parse(input, start, out newStart);

                    return default;
                }
            }

            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA> Parse(IInput<char>? input)
                {
                    var _ALPHA_1 = __GeneratedOdataV4.Parsers.Rules._ALPHAParser.Instance.Parse(input);
if (!_ALPHA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(_ALPHA_1.Parsed), _ALPHA_1.Remainder);
                }
            }
        }
        
        public static class _ʺx5FʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ʺx5Fʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ʺx5Fʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ʺx5Fʺ> Parse(IInput<char>? input)
                {
                    var _ʺx5Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx5FʺParser.Instance.Parse(input);
if (!_ʺx5Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ʺx5Fʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ʺx5Fʺ.Instance, _ʺx5Fʺ_1.Remainder);
                }
            }
        }
        
        public static class _DIGITParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._DIGIT> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._DIGIT>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._DIGIT> Parse(IInput<char>? input)
                {
                    var _DIGIT_1 = __GeneratedOdataV4.Parsers.Rules._DIGITParser.Instance.Parse(input);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._identifierCharacter._DIGIT)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._DIGIT(_DIGIT_1.Parsed), _DIGIT_1.Remainder);
                }
            }
        }
    }
    
}
