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

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._queryOptions(_queryOption_1.Parsed, System.Linq.Enumerable.Empty<__GeneratedOdataV3.CstNodes.Inners._Ⲥʺx26ʺ_queryOptionↃ>()), _queryOption_1.Remainder);
            }
        }
    }
    
}
