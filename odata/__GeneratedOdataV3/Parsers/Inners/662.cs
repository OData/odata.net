namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx21ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx21ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx21ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx21ʺ> Parse(IInput<char>? input)
            {
                var _x21_1 = __GeneratedOdataV3.Parsers.Inners._x21Parser.Instance.Parse(input);
if (!_x21_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx21ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx21ʺ.Instance, _x21_1.Remainder);
            }
        }
    }
    
}
