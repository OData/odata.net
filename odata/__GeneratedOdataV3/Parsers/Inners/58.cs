namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fx24x76x61x6Cx75x65ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x76x61x6Cx75x65ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x76x61x6Cx75x65ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x76x61x6Cx75x65ʺ> Parse(IInput<char>? input)
            {
                var _x2F_1 = __GeneratedOdataV3.Parsers.Inners._x2FParser.Instance.Parse(input);
if (!_x2F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x76x61x6Cx75x65ʺ)!, input);
}

var _x24_1 = __GeneratedOdataV3.Parsers.Inners._x24Parser.Instance.Parse(_x2F_1.Remainder);
if (!_x24_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x76x61x6Cx75x65ʺ)!, input);
}

var _x76_1 = __GeneratedOdataV3.Parsers.Inners._x76Parser.Instance.Parse(_x24_1.Remainder);
if (!_x76_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x76x61x6Cx75x65ʺ)!, input);
}

var _x61_1 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x76_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x76x61x6Cx75x65ʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV3.Parsers.Inners._x6CParser.Instance.Parse(_x61_1.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x76x61x6Cx75x65ʺ)!, input);
}

var _x75_1 = __GeneratedOdataV3.Parsers.Inners._x75Parser.Instance.Parse(_x6C_1.Remainder);
if (!_x75_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x76x61x6Cx75x65ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x75_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x76x61x6Cx75x65ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x76x61x6Cx75x65ʺ.Instance, _x65_1.Remainder);
            }
        }
    }
    
}
