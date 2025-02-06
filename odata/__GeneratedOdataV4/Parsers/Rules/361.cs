namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _excludeOperatorParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._excludeOperator> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._excludeOperator>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._excludeOperator> Parse(IInput<char>? input)
            {
                var _ʺx2Dʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2DʺParser.Instance.Parse(input);
if (!_ʺx2Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._excludeOperator)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._excludeOperator.Instance, _ʺx2Dʺ_1.Remainder);
            }
        }
    }
    
}
