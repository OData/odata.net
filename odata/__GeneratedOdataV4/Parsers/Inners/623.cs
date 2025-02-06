namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx23ʺ_fragmentParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx23ʺ_fragment> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx23ʺ_fragment>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx23ʺ_fragment> Parse(IInput<char>? input)
            {
                var _ʺx23ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx23ʺParser.Instance.Parse(input);
if (!_ʺx23ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx23ʺ_fragment)!, input);
}

var _fragment_1 = __GeneratedOdataV4.Parsers.Rules._fragmentParser.Instance.Parse(_ʺx23ʺ_1.Remainder);
if (!_fragment_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx23ʺ_fragment)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ʺx23ʺ_fragment(_ʺx23ʺ_1.Parsed, _fragment_1.Parsed), _fragment_1.Remainder);
            }
        }
    }
    
}
