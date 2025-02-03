namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _metadataOptionsParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._metadataOptions> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._metadataOptions>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._metadataOptions> Parse(IInput<char>? input)
            {
                var _metadataOption_1 = __GeneratedOdataV3.Parsers.Rules._metadataOptionParser.Instance.Parse(input);
if (!_metadataOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._metadataOptions)!, input);
}

var _Ⲥʺx26ʺ_metadataOptionↃ_1 = Inners._Ⲥʺx26ʺ_metadataOptionↃParser.Instance.Many().Parse(_metadataOption_1.Remainder);
if (!_Ⲥʺx26ʺ_metadataOptionↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._metadataOptions)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._metadataOptions(_metadataOption_1.Parsed,  _Ⲥʺx26ʺ_metadataOptionↃ_1.Parsed), _Ⲥʺx26ʺ_metadataOptionↃ_1.Remainder);
            }
        }
    }
    
}
