namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_ringLiteralParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_ringLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_ringLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_ringLiteral> Parse(IInput<char>? input)
            {
                var _COMMA_1 = __GeneratedOdataV3.Parsers.Rules._COMMAParser.Instance.Parse(input);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_ringLiteral)!, input);
}

var _ringLiteral_1 = __GeneratedOdataV3.Parsers.Rules._ringLiteralParser.Instance.Parse(_COMMA_1.Remainder);
if (!_ringLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_ringLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._COMMA_ringLiteral(_COMMA_1.Parsed, _ringLiteral_1.Parsed), _ringLiteral_1.Remainder);
            }
        }
    }
    
}
