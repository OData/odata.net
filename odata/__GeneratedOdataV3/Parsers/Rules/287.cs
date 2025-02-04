namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _int32ValueParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._int32Value> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._int32Value>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._int32Value> Parse(IInput<char>? input)
            {
                var _SIGN_1 = __GeneratedOdataV3.Parsers.Rules._SIGNParser.Instance.Optional().Parse(input);
if (!_SIGN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._int32Value)!, input);
}

var _DIGIT_1 = __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Repeat(1, 10).Parse(_SIGN_1.Remainder);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._int32Value)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._int32Value(_SIGN_1.Parsed.GetOrElse(null), new __GeneratedOdataV3.CstNodes.Inners.HelperRangedFrom1To10<__GeneratedOdataV3.CstNodes.Rules._DIGIT>(_DIGIT_1.Parsed)), _DIGIT_1.Remainder);
            }
        }
    }
    
}
