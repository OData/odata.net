namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx24x63x6Fx6Dx70x75x74x65ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺ> Parse(IInput<char>? input)
            {
                var _x24_1 = __GeneratedOdataV3.Parsers.Inners._x24Parser.Instance.Parse(input);
if (!_x24_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺ)!, input);
}

var _x63_1 = __GeneratedOdataV3.Parsers.Inners._x63Parser.Instance.Parse(_x24_1.Remainder);
if (!_x63_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(_x63_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺ)!, input);
}

var _x6D_1 = __GeneratedOdataV3.Parsers.Inners._x6DParser.Instance.Parse(_x6F_1.Remainder);
if (!_x6D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺ)!, input);
}

var _x70_1 = __GeneratedOdataV3.Parsers.Inners._x70Parser.Instance.Parse(_x6D_1.Remainder);
if (!_x70_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺ)!, input);
}

var _x75_1 = __GeneratedOdataV3.Parsers.Inners._x75Parser.Instance.Parse(_x70_1.Remainder);
if (!_x75_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x75_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x74_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx24x63x6Fx6Dx70x75x74x65ʺ.Instance, _x65_1.Remainder);
            }
        }
    }
    
}
