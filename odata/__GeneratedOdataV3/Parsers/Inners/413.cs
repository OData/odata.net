namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _oneToNine_ЖDIGITParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._oneToNine_ЖDIGIT> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._oneToNine_ЖDIGIT>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._oneToNine_ЖDIGIT> Parse(IInput<char>? input)
            {
                var _oneToNine_1 = __GeneratedOdataV3.Parsers.Rules._oneToNineParser.Instance.Parse(input);
if (!_oneToNine_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._oneToNine_ЖDIGIT)!, input);
}

var _DIGIT_1 = __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Many().Parse(_oneToNine_1.Remainder);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._oneToNine_ЖDIGIT)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._oneToNine_ЖDIGIT(_oneToNine_1.Parsed,  _DIGIT_1.Parsed), _DIGIT_1.Remainder);
            }
        }
    }
    
}
