namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _base64b16Ⳇbase64b8Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._base64b16Ⳇbase64b8> Instance { get; } = (_base64b16Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._base64b16Ⳇbase64b8>(_base64b8Parser.Instance);
        
        public static class _base64b16Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._base64b16Ⳇbase64b8._base64b16> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._base64b16Ⳇbase64b8._base64b16>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._base64b16Ⳇbase64b8._base64b16> Parse(IInput<char>? input)
                {
                    var _base64b16_1 = __GeneratedOdataV3.Parsers.Rules._base64b16Parser.Instance.Parse(input);
if (!_base64b16_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._base64b16Ⳇbase64b8._base64b16)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._base64b16Ⳇbase64b8._base64b16(_base64b16_1.Parsed), _base64b16_1.Remainder);
                }
            }
        }
        
        public static class _base64b8Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._base64b16Ⳇbase64b8._base64b8> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._base64b16Ⳇbase64b8._base64b8>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._base64b16Ⳇbase64b8._base64b8> Parse(IInput<char>? input)
                {
                    var _base64b8_1 = __GeneratedOdataV3.Parsers.Rules._base64b8Parser.Instance.Parse(input);
if (!_base64b8_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._base64b16Ⳇbase64b8._base64b8)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._base64b16Ⳇbase64b8._base64b8(_base64b8_1.Parsed), _base64b8_1.Remainder);
                }
            }
        }
    }
    
}
