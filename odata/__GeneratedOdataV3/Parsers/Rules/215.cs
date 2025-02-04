namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _stringInJSONParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._stringInJSON> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._stringInJSON>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._stringInJSON> Parse(IInput<char>? input)
            {
                var _quotationⲻmark_1 = __GeneratedOdataV3.Parsers.Rules._quotationⲻmarkParser.Instance.Parse(input);
if (!_quotationⲻmark_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._stringInJSON)!, input);
}

var _charInJSON_1 = __GeneratedOdataV3.Parsers.Rules._charInJSONParser.Instance.Many().Parse(_quotationⲻmark_1.Remainder);
if (!_charInJSON_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._stringInJSON)!, input);
}

var _quotationⲻmark_2 = __GeneratedOdataV3.Parsers.Rules._quotationⲻmarkParser.Instance.Parse(_charInJSON_1.Remainder);
if (!_quotationⲻmark_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._stringInJSON)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._stringInJSON(_quotationⲻmark_1.Parsed, _charInJSON_1.Parsed, _quotationⲻmark_2.Parsed), _quotationⲻmark_2.Remainder);
            }
        }
    }
    
}
