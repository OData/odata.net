namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx62ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx62ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx62ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx62ʺ> Parse(IInput<char>? input)
            {
                var _x62_1 = __GeneratedOdataV4.Parsers.Inners._x62Parser.Instance.Parse(input);
if (!_x62_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx62ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx62ʺ.Instance, _x62_1.Remainder);
            }
        }
    }
    
}
