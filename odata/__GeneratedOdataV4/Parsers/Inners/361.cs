namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ> Parse(IInput<char>? input)
            {
                var _primitiveLiteralInJSON_1 = __GeneratedOdataV4.Parsers.Rules._primitiveLiteralInJSONParser.Instance.Parse(input);
if (!_primitiveLiteralInJSON_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ)!, input);
}

var _Ⲥvalueⲻseparator_primitiveLiteralInJSONↃ_1 = Inners._Ⲥvalueⲻseparator_primitiveLiteralInJSONↃParser.Instance.Many().Parse(_primitiveLiteralInJSON_1.Remainder);
if (!_Ⲥvalueⲻseparator_primitiveLiteralInJSONↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ(_primitiveLiteralInJSON_1.Parsed, _Ⲥvalueⲻseparator_primitiveLiteralInJSONↃ_1.Parsed), _Ⲥvalueⲻseparator_primitiveLiteralInJSONↃ_1.Remainder);
            }
        }
    }
    
}
