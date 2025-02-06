namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx46ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx46ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx46ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx46ʺ> Parse(IInput<char>? input)
            {
                var _x46_1 = __GeneratedOdataV4.Parsers.Inners._x46Parser.Instance.Parse(input);
if (!_x46_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx46ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx46ʺ.Instance, _x46_1.Remainder);
            }
        }
    }
    
}
