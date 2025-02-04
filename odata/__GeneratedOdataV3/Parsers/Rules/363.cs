namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _respondAsyncPreferenceParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._respondAsyncPreference> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._respondAsyncPreference>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._respondAsyncPreference> Parse(IInput<char>? input)
            {
                var _ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺParser.Instance.Parse(input);
if (!_ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._respondAsyncPreference)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._respondAsyncPreference.Instance, _ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺ_1.Remainder);
            }
        }
    }
    
}
