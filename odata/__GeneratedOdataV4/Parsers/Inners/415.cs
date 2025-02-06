namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx65ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx65ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx65ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx65ʺ> Parse(IInput<char>? input)
            {
                var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(input);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx65ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx65ʺ.Instance, _x65_1.Remainder);
            }
        }
    }
    
}
