namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥvalueⲻseparator_primitiveLiteralInJSONↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥvalueⲻseparator_primitiveLiteralInJSONↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥvalueⲻseparator_primitiveLiteralInJSONↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥvalueⲻseparator_primitiveLiteralInJSONↃ> Parse(IInput<char>? input)
            {
                var _valueⲻseparator_primitiveLiteralInJSON_1 = __GeneratedOdataV3.Parsers.Inners._valueⲻseparator_primitiveLiteralInJSONParser.Instance.Parse(input);
if (!_valueⲻseparator_primitiveLiteralInJSON_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⲥvalueⲻseparator_primitiveLiteralInJSONↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._Ⲥvalueⲻseparator_primitiveLiteralInJSONↃ(_valueⲻseparator_primitiveLiteralInJSON_1.Parsed), _valueⲻseparator_primitiveLiteralInJSON_1.Remainder);
            }
        }
    }
    
}
