namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _binaryValueParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._binaryValue> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._binaryValue>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._binaryValue> Parse(IInput<char>? input)
            {
                var _Ⲥ4base64charↃ_1 = Inners._Ⲥ4base64charↃParser.Instance.Many().Parse(input);
if (!_Ⲥ4base64charↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._binaryValue)!, input);
}

var _base64b16Ⳇbase64b8_1 = __GeneratedOdataV4.Parsers.Inners._base64b16Ⳇbase64b8Parser.Instance.Optional().Parse(_Ⲥ4base64charↃ_1.Remainder);
if (!_base64b16Ⳇbase64b8_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._binaryValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._binaryValue(_Ⲥ4base64charↃ_1.Parsed, _base64b16Ⳇbase64b8_1.Parsed.GetOrElse(null)), _base64b16Ⳇbase64b8_1.Remainder);
            }
        }
    }
    
}
