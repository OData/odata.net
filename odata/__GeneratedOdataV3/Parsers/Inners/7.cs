namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺↃ> Parse(IInput<char>? input)
            {
                var _ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺParser.Instance.Parse(input);
if (!_ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⲥʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺↃ(_ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ_1.Parsed), _ʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺ_1.Remainder);
            }
        }
    }
    
}
