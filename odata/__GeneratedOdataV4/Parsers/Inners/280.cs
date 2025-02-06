namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_functionExprParameterParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_functionExprParameter> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_functionExprParameter>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_functionExprParameter> Parse(IInput<char>? input)
            {
                var _COMMA_1 = __GeneratedOdataV4.Parsers.Rules._COMMAParser.Instance.Parse(input);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._COMMA_functionExprParameter)!, input);
}

var _functionExprParameter_1 = __GeneratedOdataV4.Parsers.Rules._functionExprParameterParser.Instance.Parse(_COMMA_1.Remainder);
if (!_functionExprParameter_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._COMMA_functionExprParameter)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._COMMA_functionExprParameter(_COMMA_1.Parsed, _functionExprParameter_1.Parsed), _functionExprParameter_1.Remainder);
            }
        }
    }
    
}
