namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx73x6Ex61x70x73x68x6Fx74ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx73x6Ex61x70x73x68x6Fx74ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx73x6Ex61x70x73x68x6Fx74ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx73x6Ex61x70x73x68x6Fx74ʺ> Parse(IInput<char>? input)
            {
                var _x73_1 = __GeneratedOdataV3.Parsers.Inners._x73Parser.Instance.Parse(input);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x6Ex61x70x73x68x6Fx74ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x73_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x6Ex61x70x73x68x6Fx74ʺ)!, input);
}

var _x61_1 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x6Ex61x70x73x68x6Fx74ʺ)!, input);
}

var _x70_1 = __GeneratedOdataV3.Parsers.Inners._x70Parser.Instance.Parse(_x61_1.Remainder);
if (!_x70_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x6Ex61x70x73x68x6Fx74ʺ)!, input);
}

var _x73_2 = __GeneratedOdataV3.Parsers.Inners._x73Parser.Instance.Parse(_x70_1.Remainder);
if (!_x73_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x6Ex61x70x73x68x6Fx74ʺ)!, input);
}

var _x68_1 = __GeneratedOdataV3.Parsers.Inners._x68Parser.Instance.Parse(_x73_2.Remainder);
if (!_x68_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x6Ex61x70x73x68x6Fx74ʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(_x68_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x6Ex61x70x73x68x6Fx74ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x6F_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x6Ex61x70x73x68x6Fx74ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx73x6Ex61x70x73x68x6Fx74ʺ.Instance, _x74_1.Remainder);
            }
        }
    }
    
}
