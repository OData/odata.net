namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _countParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._count> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._count>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._count> Parse(IInput<char>? input)
            {
                var _ʺx2Fx24x63x6Fx75x6Ex74ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fx24x63x6Fx75x6Ex74ʺParser.Instance.Parse(input);
if (!_ʺx2Fx24x63x6Fx75x6Ex74ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._count)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._count.Instance, _ʺx2Fx24x63x6Fx75x6Ex74ʺ_1.Remainder);
            }
        }
    }
    
}
