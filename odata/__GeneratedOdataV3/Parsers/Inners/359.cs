namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _valueⲻseparator_primitiveLiteralInJSONParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._valueⲻseparator_primitiveLiteralInJSON> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._valueⲻseparator_primitiveLiteralInJSON>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._valueⲻseparator_primitiveLiteralInJSON> Parse(IInput<char>? input)
            {
                var _valueⲻseparator_1 = __GeneratedOdataV3.Parsers.Rules._valueⲻseparatorParser.Instance.Parse(input);
if (!_valueⲻseparator_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._valueⲻseparator_primitiveLiteralInJSON)!, input);
}

var _primitiveLiteralInJSON_1 = __GeneratedOdataV3.Parsers.Rules._primitiveLiteralInJSONParser.Instance.Parse(_valueⲻseparator_1.Remainder);
if (!_primitiveLiteralInJSON_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._valueⲻseparator_primitiveLiteralInJSON)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._valueⲻseparator_primitiveLiteralInJSON(_valueⲻseparator_1.Parsed, _primitiveLiteralInJSON_1.Parsed), _primitiveLiteralInJSON_1.Remainder);
            }
        }
    }
    
}
