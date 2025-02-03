namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx26ʺ_batchOptionParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_batchOption> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_batchOption>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_batchOption> Parse(IInput<char>? input)
            {
                var _ʺx26ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx26ʺParser.Instance.Parse(input);
if (!_ʺx26ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_batchOption)!, input);
}

var _batchOption_1 = __GeneratedOdataV3.Parsers.Rules._batchOptionParser.Instance.Parse(_ʺx26ʺ_1.Remainder);
if (!_batchOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_batchOption)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx26ʺ_batchOption(_ʺx26ʺ_1.Parsed,  _batchOption_1.Parsed), _batchOption_1.Remainder);
            }
        }
    }
    
}
