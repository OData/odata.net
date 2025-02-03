namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx3Fʺ_metadataOptionsParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_metadataOptions> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_metadataOptions>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_metadataOptions> Parse(IInput<char>? input)
            {
                var _ʺx3Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3FʺParser.Instance.Parse(input);
if (!_ʺx3Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_metadataOptions)!, input);
}

var _metadataOptions_1 = __GeneratedOdataV3.Parsers.Rules._metadataOptionsParser.Instance.Parse(_ʺx3Fʺ_1.Remainder);
if (!_metadataOptions_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_metadataOptions)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_metadataOptions(_ʺx3Fʺ_1.Parsed,  _metadataOptions_1.Parsed), _metadataOptions_1.Remainder);
            }
        }
    }
    
}
