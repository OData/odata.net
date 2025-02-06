namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx42x79x74x65ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx42x79x74x65ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx42x79x74x65ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx42x79x74x65ʺ> Parse(IInput<char>? input)
            {
                var _x42_1 = __GeneratedOdataV4.Parsers.Inners._x42Parser.Instance.Parse(input);
if (!_x42_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx42x79x74x65ʺ)!, input);
}

var _x79_1 = __GeneratedOdataV4.Parsers.Inners._x79Parser.Instance.Parse(_x42_1.Remainder);
if (!_x79_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx42x79x74x65ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x79_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx42x79x74x65ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x74_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx42x79x74x65ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx42x79x74x65ʺ.Instance, _x65_1.Remainder);
            }
        }
    }
    
}
