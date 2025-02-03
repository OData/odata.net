namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx68x74x74x70ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70ʺ> Parse(IInput<char>? input)
            {
                var _x68_1 = __GeneratedOdataV3.Parsers.Inners._x68Parser.Instance.Parse(input);
if (!_x68_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x68_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70ʺ)!, input);
}

var _x74_2 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x74_1.Remainder);
if (!_x74_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70ʺ)!, input);
}

var _x70_1 = __GeneratedOdataV3.Parsers.Inners._x70Parser.Instance.Parse(_x74_2.Remainder);
if (!_x70_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx68x74x74x70ʺ.Instance, _x70_1.Remainder);
            }
        }
    }
    
}
