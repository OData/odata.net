namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitivePropertyInUriParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitivePropertyInUri> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitivePropertyInUri>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitivePropertyInUri> Parse(IInput<char>? input)
            {
                var _quotationⲻmark_1 = __GeneratedOdataV4.Parsers.Rules._quotationⲻmarkParser.Instance.Parse(input);
if (!_quotationⲻmark_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitivePropertyInUri)!, input);
}

var _primitiveProperty_1 = __GeneratedOdataV4.Parsers.Rules._primitivePropertyParser.Instance.Parse(_quotationⲻmark_1.Remainder);
if (!_primitiveProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitivePropertyInUri)!, input);
}

var _quotationⲻmark_2 = __GeneratedOdataV4.Parsers.Rules._quotationⲻmarkParser.Instance.Parse(_primitiveProperty_1.Remainder);
if (!_quotationⲻmark_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitivePropertyInUri)!, input);
}

var _nameⲻseparator_1 = __GeneratedOdataV4.Parsers.Rules._nameⲻseparatorParser.Instance.Parse(_quotationⲻmark_2.Remainder);
if (!_nameⲻseparator_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitivePropertyInUri)!, input);
}

var _primitiveLiteralInJSON_1 = __GeneratedOdataV4.Parsers.Rules._primitiveLiteralInJSONParser.Instance.Parse(_nameⲻseparator_1.Remainder);
if (!_primitiveLiteralInJSON_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitivePropertyInUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitivePropertyInUri(_quotationⲻmark_1.Parsed, _primitiveProperty_1.Parsed, _quotationⲻmark_2.Parsed, _nameⲻseparator_1.Parsed, _primitiveLiteralInJSON_1.Parsed), _primitiveLiteralInJSON_1.Remainder);
            }
        }
    }
    
}
