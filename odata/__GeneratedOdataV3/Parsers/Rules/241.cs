namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _identifierCharacterParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._identifierCharacter> Instance { get; } = (_ALPHAParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._identifierCharacter>(_ʺx5FʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._identifierCharacter>(_DIGITParser.Instance);
        
        public static class _ALPHAParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._identifierCharacter._ALPHA> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._identifierCharacter._ALPHA>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._identifierCharacter._ALPHA> Parse(IInput<char>? input)
                {
                    var _ALPHA_1 = __GeneratedOdataV3.Parsers.Rules._ALPHAParser.Instance.Parse(input);
if (!_ALPHA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._identifierCharacter._ALPHA)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._identifierCharacter._ALPHA(_ALPHA_1.Parsed), _ALPHA_1.Remainder);
                }
            }
        }
        
        public static class _ʺx5FʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._identifierCharacter._ʺx5Fʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._identifierCharacter._ʺx5Fʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._identifierCharacter._ʺx5Fʺ> Parse(IInput<char>? input)
                {
                    var _ʺx5Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx5FʺParser.Instance.Parse(input);
if (!_ʺx5Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._identifierCharacter._ʺx5Fʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._identifierCharacter._ʺx5Fʺ(_ʺx5Fʺ_1.Parsed), _ʺx5Fʺ_1.Remainder);
                }
            }
        }
        
        public static class _DIGITParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._identifierCharacter._DIGIT> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._identifierCharacter._DIGIT>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._identifierCharacter._DIGIT> Parse(IInput<char>? input)
                {
                    var _DIGIT_1 = __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Parse(input);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._identifierCharacter._DIGIT)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._identifierCharacter._DIGIT(_DIGIT_1.Parsed), _DIGIT_1.Remainder);
                }
            }
        }
    }
    
}
