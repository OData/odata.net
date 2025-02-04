namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _requestⲻidParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._requestⲻid> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._requestⲻid>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._requestⲻid> Parse(IInput<char>? input)
            {
                var _unreserved_1 = __GeneratedOdataV3.Parsers.Rules._unreservedParser.Instance.Repeat(1, null).Parse(input);
if (!_unreserved_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._requestⲻid)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._requestⲻid(new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV3.CstNodes.Rules._unreserved>(_unreserved_1.Parsed)), _unreserved_1.Remainder);
            }
        }
    }
    
}
