namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _eachParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._each> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._each>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._each> Parse(IInput<char>? input)
            {
                var _ʺx2Fx24x65x61x63x68ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2Fx24x65x61x63x68ʺParser.Instance.Parse(input);
if (!_ʺx2Fx24x65x61x63x68ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._each)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._each.Instance, _ʺx2Fx24x65x61x63x68ʺ_1.Remainder);
            }
        }
    }
    
}
