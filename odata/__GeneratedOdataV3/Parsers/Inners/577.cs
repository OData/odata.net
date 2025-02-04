namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _EQⲻh_booleanValueParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._EQⲻh_booleanValue> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._EQⲻh_booleanValue>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._EQⲻh_booleanValue> Parse(IInput<char>? input)
            {
                var _EQⲻh_1 = __GeneratedOdataV3.Parsers.Rules._EQⲻhParser.Instance.Parse(input);
if (!_EQⲻh_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._EQⲻh_booleanValue)!, input);
}

var _booleanValue_1 = __GeneratedOdataV3.Parsers.Rules._booleanValueParser.Instance.Parse(_EQⲻh_1.Remainder);
if (!_booleanValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._EQⲻh_booleanValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._EQⲻh_booleanValue(_EQⲻh_1.Parsed, _booleanValue_1.Parsed), _booleanValue_1.Remainder);
            }
        }
    }
    
}
