namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Eʺ_fractionalSecondsParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ_fractionalSeconds> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ_fractionalSeconds>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ_fractionalSeconds> Parse(IInput<char>? input)
            {
                var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(input);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ_fractionalSeconds)!, input);
}

var _fractionalSeconds_1 = __GeneratedOdataV3.Parsers.Rules._fractionalSecondsParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_fractionalSeconds_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ_fractionalSeconds)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ_fractionalSeconds(_ʺx2Eʺ_1.Parsed,  _fractionalSeconds_1.Parsed), _fractionalSeconds_1.Remainder);
            }
        }
    }
    
}
