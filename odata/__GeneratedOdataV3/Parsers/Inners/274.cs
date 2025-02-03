namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx24x74x68x69x73ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x74x68x69x73ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x74x68x69x73ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x74x68x69x73ʺ> Parse(IInput<char>? input)
            {
                var _x24_1 = __GeneratedOdataV3.Parsers.Inners._x24Parser.Instance.Parse(input);
if (!_x24_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x74x68x69x73ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x24_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x74x68x69x73ʺ)!, input);
}

var _x68_1 = __GeneratedOdataV3.Parsers.Inners._x68Parser.Instance.Parse(_x74_1.Remainder);
if (!_x68_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x74x68x69x73ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV3.Parsers.Inners._x69Parser.Instance.Parse(_x68_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x74x68x69x73ʺ)!, input);
}

var _x73_1 = __GeneratedOdataV3.Parsers.Inners._x73Parser.Instance.Parse(_x69_1.Remainder);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x74x68x69x73ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx24x74x68x69x73ʺ.Instance, _x73_1.Remainder);
            }
        }
    }
    
}
