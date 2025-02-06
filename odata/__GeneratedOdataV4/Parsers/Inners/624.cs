namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fx2FʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fx2Fʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fx2Fʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fx2Fʺ> Parse(IInput<char>? input)
            {
                var _x2F_1 = __GeneratedOdataV4.Parsers.Inners._x2FParser.Instance.Parse(input);
if (!_x2F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fx2Fʺ)!, input);
}

var _x2F_2 = __GeneratedOdataV4.Parsers.Inners._x2FParser.Instance.Parse(_x2F_1.Remainder);
if (!_x2F_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fx2Fʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fx2Fʺ.Instance, _x2F_2.Remainder);
            }
        }
    }
    
}
