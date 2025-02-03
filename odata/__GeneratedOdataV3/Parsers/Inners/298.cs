namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx74x6Fx6Cx6Fx77x65x72ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx74x6Fx6Cx6Fx77x65x72ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx74x6Fx6Cx6Fx77x65x72ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx74x6Fx6Cx6Fx77x65x72ʺ> Parse(IInput<char>? input)
            {
                var _x74_1 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(input);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx74x6Fx6Cx6Fx77x65x72ʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(_x74_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx74x6Fx6Cx6Fx77x65x72ʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV3.Parsers.Inners._x6CParser.Instance.Parse(_x6F_1.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx74x6Fx6Cx6Fx77x65x72ʺ)!, input);
}

var _x6F_2 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(_x6C_1.Remainder);
if (!_x6F_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx74x6Fx6Cx6Fx77x65x72ʺ)!, input);
}

var _x77_1 = __GeneratedOdataV3.Parsers.Inners._x77Parser.Instance.Parse(_x6F_2.Remainder);
if (!_x77_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx74x6Fx6Cx6Fx77x65x72ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x77_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx74x6Fx6Cx6Fx77x65x72ʺ)!, input);
}

var _x72_1 = __GeneratedOdataV3.Parsers.Inners._x72Parser.Instance.Parse(_x65_1.Remainder);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx74x6Fx6Cx6Fx77x65x72ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx74x6Fx6Cx6Fx77x65x72ʺ.Instance, _x72_1.Remainder);
            }
        }
    }
    
}
