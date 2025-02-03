namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx6Fx72x64x65x72x62x79ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx6Fx72x64x65x72x62x79ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx6Fx72x64x65x72x62x79ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx6Fx72x64x65x72x62x79ʺ> Parse(IInput<char>? input)
            {
                var _x6F_1 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(input);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Fx72x64x65x72x62x79ʺ)!, input);
}

var _x72_1 = __GeneratedOdataV3.Parsers.Inners._x72Parser.Instance.Parse(_x6F_1.Remainder);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Fx72x64x65x72x62x79ʺ)!, input);
}

var _x64_1 = __GeneratedOdataV3.Parsers.Inners._x64Parser.Instance.Parse(_x72_1.Remainder);
if (!_x64_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Fx72x64x65x72x62x79ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x64_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Fx72x64x65x72x62x79ʺ)!, input);
}

var _x72_2 = __GeneratedOdataV3.Parsers.Inners._x72Parser.Instance.Parse(_x65_1.Remainder);
if (!_x72_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Fx72x64x65x72x62x79ʺ)!, input);
}

var _x62_1 = __GeneratedOdataV3.Parsers.Inners._x62Parser.Instance.Parse(_x72_2.Remainder);
if (!_x62_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Fx72x64x65x72x62x79ʺ)!, input);
}

var _x79_1 = __GeneratedOdataV3.Parsers.Inners._x79Parser.Instance.Parse(_x62_1.Remainder);
if (!_x79_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Fx72x64x65x72x62x79ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx6Fx72x64x65x72x62x79ʺ.Instance, _x79_1.Remainder);
            }
        }
    }
    
}
