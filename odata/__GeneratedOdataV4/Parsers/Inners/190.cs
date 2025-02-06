namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx4Fx52ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx4Fx52ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx4Fx52ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx4Fx52ʺ> Parse(IInput<char>? input)
            {
                var _x4F_1 = __GeneratedOdataV4.Parsers.Inners._x4FParser.Instance.Parse(input);
if (!_x4F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx52ʺ)!, input);
}

var _x52_1 = __GeneratedOdataV4.Parsers.Inners._x52Parser.Instance.Parse(_x4F_1.Remainder);
if (!_x52_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx52ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx4Fx52ʺ.Instance, _x52_1.Remainder);
            }
        }
    }
    
}
