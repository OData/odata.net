namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx41x4Ex44ʺ_RWSParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41x4Ex44ʺ_RWS> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41x4Ex44ʺ_RWS>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41x4Ex44ʺ_RWS> Parse(IInput<char>? input)
            {
                var _ʺx41x4Ex44ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx41x4Ex44ʺParser.Instance.Parse(input);
if (!_ʺx41x4Ex44ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx41x4Ex44ʺ_RWS)!, input);
}

var _RWS_1 = __GeneratedOdataV4.Parsers.Rules._RWSParser.Instance.Parse(_ʺx41x4Ex44ʺ_1.Remainder);
if (!_RWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx41x4Ex44ʺ_RWS)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ʺx41x4Ex44ʺ_RWS(_ʺx41x4Ex44ʺ_1.Parsed, _RWS_1.Parsed), _RWS_1.Remainder);
            }
        }
    }
    
}
