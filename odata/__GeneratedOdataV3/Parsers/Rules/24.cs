namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _refParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._ref> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._ref>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._ref> Parse(IInput<char>? input)
            {
                var _ʺx2Fx24x72x65x66ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fx24x72x65x66ʺParser.Instance.Parse(input);
if (!_ʺx2Fx24x72x65x66ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._ref)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._ref.Instance, _ʺx2Fx24x72x65x66ʺ_1.Remainder);
            }
        }
    }
    
}
