namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _queryOptionsParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._queryOptions> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._queryOptions>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._queryOptions> Parse(IInput<char>? input)
            {
                var _queryOption_1 = __GeneratedOdataV3.Parsers.Rules._queryOptionParser.Instance.Parse(input);
if (!_queryOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._queryOptions)!, input);
}

var _Ⲥʺx26ʺ_queryOptionↃ_1 = Inners._Ⲥʺx26ʺ_queryOptionↃParser.Instance.Many().Parse(_queryOption_1.Remainder);
if (!_Ⲥʺx26ʺ_queryOptionↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._queryOptions)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._queryOptions(_queryOption_1.Parsed, _Ⲥʺx26ʺ_queryOptionↃ_1.Parsed), _Ⲥʺx26ʺ_queryOptionↃ_1.Remainder);
            }
        }
    }
    
}
