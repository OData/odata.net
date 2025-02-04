namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx3Fʺ_batchOptionsParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_batchOptions> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_batchOptions>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_batchOptions> Parse(IInput<char>? input)
            {
                var _ʺx3Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3FʺParser.Instance.Parse(input);
if (!_ʺx3Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_batchOptions)!, input);
}

var _batchOptions_1 = __GeneratedOdataV3.Parsers.Rules._batchOptionsParser.Instance.Parse(_ʺx3Fʺ_1.Remainder);
if (!_batchOptions_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_batchOptions)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_batchOptions(_ʺx3Fʺ_1.Parsed, _batchOptions_1.Parsed), _batchOptions_1.Remainder);
            }
        }
    }
    
}
