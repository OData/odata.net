namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx59ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx59ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx59ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx59ʺ> Parse(IInput<char>? input)
            {
                var _x59_1 = __GeneratedOdataV4.Parsers.Inners._x59Parser.Instance.Parse(input);
if (!_x59_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx59ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx59ʺ.Instance, _x59_1.Remainder);
            }
        }
    }
    
}
