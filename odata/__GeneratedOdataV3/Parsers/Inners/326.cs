namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx6Fx72ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx6Fx72ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx6Fx72ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx6Fx72ʺ> Parse(IInput<char>? input)
            {
                var _x6F_1 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(input);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Fx72ʺ)!, input);
}

var _x72_1 = __GeneratedOdataV3.Parsers.Inners._x72Parser.Instance.Parse(_x6F_1.Remainder);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Fx72ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx6Fx72ʺ.Instance, _x72_1.Remainder);
            }
        }
    }
    
}
