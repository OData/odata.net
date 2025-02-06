namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx74x6Fx70ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx70ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx70ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx70ʺ> Parse(IInput<char>? input)
            {
                var _x74_1 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(input);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx70ʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV4.Parsers.Inners._x6FParser.Instance.Parse(_x74_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx70ʺ)!, input);
}

var _x70_1 = __GeneratedOdataV4.Parsers.Inners._x70Parser.Instance.Parse(_x6F_1.Remainder);
if (!_x70_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx70ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx74x6Fx70ʺ.Instance, _x70_1.Remainder);
            }
        }
    }
    
}
