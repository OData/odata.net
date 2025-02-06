namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx34ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx34ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx34ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx34ʺ> Parse(IInput<char>? input)
            {
                var _x34_1 = __GeneratedOdataV4.Parsers.Inners._x34Parser.Instance.Parse(input);
if (!_x34_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx34ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx34ʺ.Instance, _x34_1.Remainder);
            }
        }
    }
    
}
