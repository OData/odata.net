namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx26ʺ_metadataOptionↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx26ʺ_metadataOptionↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx26ʺ_metadataOptionↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx26ʺ_metadataOptionↃ> Parse(IInput<char>? input)
            {
                var _ʺx26ʺ_metadataOption_1 = __GeneratedOdataV4.Parsers.Inners._ʺx26ʺ_metadataOptionParser.Instance.Parse(input);
if (!_ʺx26ʺ_metadataOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._Ⲥʺx26ʺ_metadataOptionↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx26ʺ_metadataOptionↃ(_ʺx26ʺ_metadataOption_1.Parsed), _ʺx26ʺ_metadataOption_1.Remainder);
            }
        }
    }
    
}
