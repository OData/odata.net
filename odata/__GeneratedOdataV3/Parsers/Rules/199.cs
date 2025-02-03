namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitiveColInUriParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveColInUri> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveColInUri>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveColInUri> Parse(IInput<char>? input)
            {
                var _beginⲻarray_1 = __GeneratedOdataV3.Parsers.Rules._beginⲻarrayParser.Instance.Parse(input);
if (!_beginⲻarray_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveColInUri)!, input);
}

var _primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ_1 = __GeneratedOdataV3.Parsers.Inners._primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃParser.Instance.Optional().Parse(_beginⲻarray_1.Remainder);
if (!_primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveColInUri)!, input);
}

var _endⲻarray_1 = __GeneratedOdataV3.Parsers.Rules._endⲻarrayParser.Instance.Parse(_primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ_1.Remainder);
if (!_endⲻarray_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveColInUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveColInUri(_beginⲻarray_1.Parsed, _primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ_1.Parsed.GetOrElse(null),  _endⲻarray_1.Parsed), _endⲻarray_1.Remainder);
            }
        }
    }
    
}
