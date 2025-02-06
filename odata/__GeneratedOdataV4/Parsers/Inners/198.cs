namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx4Ex4Fx54ʺ_RWSParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx4Ex4Fx54ʺ_RWS> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx4Ex4Fx54ʺ_RWS>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx4Ex4Fx54ʺ_RWS> Parse(IInput<char>? input)
            {
                var _ʺx4Ex4Fx54ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx4Ex4Fx54ʺParser.Instance.Parse(input);
if (!_ʺx4Ex4Fx54ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Ex4Fx54ʺ_RWS)!, input);
}

var _RWS_1 = __GeneratedOdataV4.Parsers.Rules._RWSParser.Instance.Parse(_ʺx4Ex4Fx54ʺ_1.Remainder);
if (!_RWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Ex4Fx54ʺ_RWS)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ʺx4Ex4Fx54ʺ_RWS(_ʺx4Ex4Fx54ʺ_1.Parsed, _RWS_1.Parsed), _RWS_1.Remainder);
            }
        }
    }
    
}
