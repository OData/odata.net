namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_functionParameterParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_functionParameter> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_functionParameter>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_functionParameter> Parse(IInput<char>? input)
            {
                var _COMMA_1 = __GeneratedOdataV4.Parsers.Rules._COMMAParser.Instance.Parse(input);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._COMMA_functionParameter)!, input);
}

var _functionParameter_1 = __GeneratedOdataV4.Parsers.Rules._functionParameterParser.Instance.Parse(_COMMA_1.Remainder);
if (!_functionParameter_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._COMMA_functionParameter)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._COMMA_functionParameter(_COMMA_1.Parsed, _functionParameter_1.Parsed), _functionParameter_1.Remainder);
            }
        }
    }
    
}
