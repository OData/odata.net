namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx48ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx48ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx48ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx48ʺ> Parse(IInput<char>? input)
            {
                var _x48_1 = __GeneratedOdataV4.Parsers.Inners._x48Parser.Instance.Parse(input);
if (!_x48_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx48ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx48ʺ.Instance, _x48_1.Remainder);
            }
        }
    }
    
}
