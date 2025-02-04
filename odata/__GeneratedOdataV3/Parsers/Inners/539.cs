namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_positionLiteralParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_positionLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_positionLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_positionLiteral> Parse(IInput<char>? input)
            {
                var _COMMA_1 = __GeneratedOdataV3.Parsers.Rules._COMMAParser.Instance.Parse(input);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_positionLiteral)!, input);
}

var _positionLiteral_1 = __GeneratedOdataV3.Parsers.Rules._positionLiteralParser.Instance.Parse(_COMMA_1.Remainder);
if (!_positionLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_positionLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._COMMA_positionLiteral(_COMMA_1.Parsed, _positionLiteral_1.Parsed), _positionLiteral_1.Remainder);
            }
        }
    }
    
}
