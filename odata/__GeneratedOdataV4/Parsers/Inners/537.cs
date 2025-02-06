namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_geoLiteralParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_geoLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_geoLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_geoLiteral> Parse(IInput<char>? input)
            {
                var _COMMA_1 = __GeneratedOdataV4.Parsers.Rules._COMMAParser.Instance.Parse(input);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._COMMA_geoLiteral)!, input);
}

var _geoLiteral_1 = __GeneratedOdataV4.Parsers.Rules._geoLiteralParser.Instance.Parse(_COMMA_1.Remainder);
if (!_geoLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._COMMA_geoLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._COMMA_geoLiteral(_COMMA_1.Parsed, _geoLiteral_1.Parsed), _geoLiteral_1.Remainder);
            }
        }
    }
    
}
