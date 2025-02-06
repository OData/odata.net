namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _EQParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._EQ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._EQ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._EQ> Parse(IInput<char>? input)
            {
                var _ʺx3Dʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3DʺParser.Instance.Parse(input);
if (!_ʺx3Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._EQ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._EQ.Instance, _ʺx3Dʺ_1.Remainder);
            }
        }
    }
    
}
