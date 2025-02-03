namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx66x61x6Cx73x65ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx66x61x6Cx73x65ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx66x61x6Cx73x65ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx66x61x6Cx73x65ʺ> Parse(IInput<char>? input)
            {
                var _x66_1 = __GeneratedOdataV3.Parsers.Inners._x66Parser.Instance.Parse(input);
if (!_x66_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx66x61x6Cx73x65ʺ)!, input);
}

var _x61_1 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x66_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx66x61x6Cx73x65ʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV3.Parsers.Inners._x6CParser.Instance.Parse(_x61_1.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx66x61x6Cx73x65ʺ)!, input);
}

var _x73_1 = __GeneratedOdataV3.Parsers.Inners._x73Parser.Instance.Parse(_x6C_1.Remainder);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx66x61x6Cx73x65ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x73_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx66x61x6Cx73x65ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx66x61x6Cx73x65ʺ.Instance, _x65_1.Remainder);
            }
        }
    }
    
}
