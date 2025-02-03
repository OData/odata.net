namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx26ʺ_metadataOptionParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_metadataOption> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_metadataOption>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_metadataOption> Parse(IInput<char>? input)
            {
                var _ʺx26ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx26ʺParser.Instance.Parse(input);
if (!_ʺx26ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_metadataOption)!, input);
}

var _metadataOption_1 = __GeneratedOdataV3.Parsers.Rules._metadataOptionParser.Instance.Parse(_ʺx26ʺ_1.Remainder);
if (!_metadataOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_metadataOption)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_metadataOption(_ʺx26ʺ_1.Parsed,  _metadataOption_1.Parsed), _metadataOption_1.Remainder);
            }
        }
    }
    
}
