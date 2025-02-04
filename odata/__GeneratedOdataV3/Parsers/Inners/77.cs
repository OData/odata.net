namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx26ʺ_batchOptionↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx26ʺ_batchOptionↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx26ʺ_batchOptionↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx26ʺ_batchOptionↃ> Parse(IInput<char>? input)
            {
                var _ʺx26ʺ_batchOption_1 = __GeneratedOdataV3.Parsers.Inners._ʺx26ʺ_batchOptionParser.Instance.Parse(input);
if (!_ʺx26ʺ_batchOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⲥʺx26ʺ_batchOptionↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx26ʺ_batchOptionↃ(_ʺx26ʺ_batchOption_1.Parsed), _ʺx26ʺ_batchOption_1.Remainder);
            }
        }
    }
    
}
