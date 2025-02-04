namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _batchOptionsParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._batchOptions> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._batchOptions>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._batchOptions> Parse(IInput<char>? input)
            {
                var _batchOption_1 = __GeneratedOdataV3.Parsers.Rules._batchOptionParser.Instance.Parse(input);
if (!_batchOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._batchOptions)!, input);
}

var _Ⲥʺx26ʺ_batchOptionↃ_1 = Inners._Ⲥʺx26ʺ_batchOptionↃParser.Instance.Many().Parse(_batchOption_1.Remainder);
if (!_Ⲥʺx26ʺ_batchOptionↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._batchOptions)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._batchOptions(_batchOption_1.Parsed, _Ⲥʺx26ʺ_batchOptionↃ_1.Parsed), _Ⲥʺx26ʺ_batchOptionↃ_1.Remainder);
            }
        }
    }
    
}
