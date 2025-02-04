namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _fragmentParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._fragment> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._fragment>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._fragment> Parse(IInput<char>? input)
            {
                var _ⲤpcharⳆʺx2FʺⳆʺx3FʺↃ_1 = Inners._ⲤpcharⳆʺx2FʺⳆʺx3FʺↃParser.Instance.Many().Parse(input);
if (!_ⲤpcharⳆʺx2FʺⳆʺx3FʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._fragment)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._fragment(_ⲤpcharⳆʺx2FʺⳆʺx3FʺↃ_1.Parsed), _ⲤpcharⳆʺx2FʺⳆʺx3FʺↃ_1.Remainder);
            }
        }
    }
    
}
