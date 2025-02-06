namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx53ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx53ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx53ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx53ʺ> Parse(IInput<char>? input)
            {
                var _x53_1 = __GeneratedOdataV4.Parsers.Inners._x53Parser.Instance.Parse(input);
if (!_x53_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx53ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx53ʺ.Instance, _x53_1.Remainder);
            }
        }
    }
    
}
