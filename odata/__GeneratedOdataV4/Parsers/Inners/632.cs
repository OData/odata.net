namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx76ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx76ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx76ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx76ʺ> Parse(IInput<char>? input)
            {
                var _x76_1 = __GeneratedOdataV4.Parsers.Inners._x76Parser.Instance.Parse(input);
if (!_x76_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx76ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx76ʺ.Instance, _x76_1.Remainder);
            }
        }
    }
    
}
