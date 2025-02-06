namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _int16ValueParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._int16Value> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._int16Value>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._int16Value> Parse(IInput<char>? input)
            {
                var _SIGN_1 = __GeneratedOdataV4.Parsers.Rules._SIGNParser.Instance.Optional().Parse(input);
if (!_SIGN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._int16Value)!, input);
}

var _DIGIT_1 = __GeneratedOdataV4.Parsers.Rules._DIGITParser.Instance.Repeat(1, 5).Parse(_SIGN_1.Remainder);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._int16Value)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._int16Value(_SIGN_1.Parsed.GetOrElse(null), new __GeneratedOdataV4.CstNodes.Inners.HelperRangedFrom1To5<__GeneratedOdataV4.CstNodes.Rules._DIGIT>(_DIGIT_1.Parsed)), _DIGIT_1.Remainder);
            }
        }
    }
    
}
